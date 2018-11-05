#if !DEBUG // Only redirect when running a release build
#define CONSOLE_OUTPUT_TO_FILE
#endif

using Be.Timvw.Framework.ComponentModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Omtime
{
    public partial class ScheduleForm : Form
    {
        public List<Practice> mPractices = new List<Practice>();

        private SplashForm mSplashForm = new SplashForm();

        private string mStudioPicSource = null;
        private List<string> m_picList = new List<string>();
        private int m_picIndex = 0;

        private CookieContainer mCookieContainer = null;
        private TimeSpan mValidCookieTime;
        private DateTime mCookieContainerExpires = new DateTime(0);

        private const string DEFAULT_STATE_FILE = "Omtime.XML.DefaultXmlState.xml";
        private const string DEFAULT_STATE_FILE_GENRES = "Omtime.XML.DefaultXmlStateClassGenres.xml";
        private const string STATE_FILE = "Omtime.xml";
        private const string STATE_FILE_GENRES = "Omgenres.xml";
        private const string APPLICATION_BACKGROUND_IMAGE_LIBRARY = "Img\\";

        private Timer mClassGenreAssimilationModeTimer = new Timer();
        private bool mClassGenreAssimilationMode = false;
        private bool mClassButtonDown = false;
        private string mClassGenreAssimilationSource;

        // TODO: Find a cleaner way to implement one-time things like this w/o needing a member variable
        private bool mOneTimeSetSizeComplete = false;

        private Image mOrigBackground = null;

        XmlSerializer mClassGenreSerializer = new XmlSerializer(typeof(SerializableDictionary<List<string>, string>));
        SerializableDictionary<List<string>, string> mClassGenreDict;
        XmlWriterSettings mXmlWriterSettings = new XmlWriterSettings();

        private XmlDocument mXmlDocumentState;
        private int mWebRequestTimeout;

        public ScheduleForm()
        {
            InitializeComponent();

#if CONSOLE_OUTPUT_TO_FILE
            RedirectConsoleOutput(); // Save all output to a file everytime; nice for live error assessment!
#endif

            // This height seems like a good starting point;
            // The grid view is perfectly fit to the window with no h-scroll bar
            //     UPDATE: This does not work because the width of things are dynamic.
            //             For example, the width of the instructors panel is based on
            //             the width of the longest instructor's name (this changes)
            //             Keep this here, but do a "one time set width" after we
            //             get the web data and know exactly what things look like
            this.ClientSize = new System.Drawing.Size(1150, 955); // 100% arbitrary

            mClassGenreAssimilationModeTimer.Interval = 2000;
            mClassGenreAssimilationModeTimer.Tick += new EventHandler(OnClassGenreAssimilationModeTimer);

            // Keep the class genre XML file looking good. Hot stuff!
            mXmlWriterSettings.Indent = true;
            mXmlWriterSettings.NewLineOnAttributes = true;

            // Set a default policy level for the "http:" and "https" schemes
            // TODO: This might be a solution to the issue we have where changing studio ID from settings menu does not result
            //       in a subsequent successful schedule fetch- it hangs! Why does it hang!??
            HttpWebRequest.DefaultCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);
        }

        private StreamWriter mConsoleWriter;
        private FileStream mConsoleFileStream;
        private void RedirectConsoleOutput()
        {
            const string CONSOLE_REDIRECT_FILENAME = "./ConsoleOutput.txt";
            try
            {
                mConsoleFileStream = new FileStream(CONSOLE_REDIRECT_FILENAME, FileMode.Create, FileAccess.Write);
                mConsoleWriter = new TimestampStreamWriter(mConsoleFileStream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open {0} for writing", CONSOLE_REDIRECT_FILENAME);
                Console.WriteLine(e.Message);
                return;
            }

            Console.SetOut(mConsoleWriter);
            Console.SetError(mConsoleWriter);
        }

        private void ScheduleForm_Load(object sender, EventArgs e)
        {
            InitGrid();
            GetXmlState();

#if true // This is for testing. Turn this off to omit the (lengthy) web query
            DoWebRequest();
#endif
        }

        private void ScheduleForm_Shown(object sender, EventArgs e)
        {

        }

        private void DoWebRequest()
        {
            workerWebReq.RunWorkerAsync();
            workerStudioPic.RunWorkerAsync();
            if (mSplashForm.IsDisposed)
            {
                mSplashForm = new SplashForm();
            }
            mSplashForm.ShowDialog();
        }

        private void DoWebRequestInBackgroudStudioPic(object sender, DoWorkEventArgs e)
        {
            // Enumerate the images in this directory: APPLICATION_BACKGROUND_IMAGE_LIBRARY
            DirectoryInfo diTop = new DirectoryInfo(@APPLICATION_BACKGROUND_IMAGE_LIBRARY);

            if (!diTop.Exists) return;
            if (diTop.GetFiles().Length == 0) return;

            // We enumerate the filesystem once and keep a library in the member variable: m_picList
            // Subsequent access to this list is quick and easy
            if (m_picList.Count > 0)
            {
                m_picIndex++;
                if (m_picIndex >= m_picList.Count) m_picIndex = 0;
                mStudioPicSource = m_picList[m_picIndex];
                return;
            }

            // We only enumerate the filesystem once
            foreach (var fi in diTop.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                try
                {
                    //Console.WriteLine("{0}\t\t{1}", fi.FullName, fi.Length.ToString("N0"));
                    m_picList.Add(fi.FullName);
                }
                catch (UnauthorizedAccessException UnAuthFile)
                {
                    Console.WriteLine("UnAuthFile: {0}", UnAuthFile.Message);
                }
            }

            // Start with something new everytime
            m_picIndex = new Random().Next(0, m_picList.Count);
            mStudioPicSource = m_picList[m_picIndex];
        }

        private void DoWebRequestInBackgroudCompletedStudioPic(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                Console.WriteLine("DoWebRequestInBackgroudCompletedStudioPic was canceled");
            }
            else if (!(e.Error == null))
            {
                Console.WriteLine("DoWebRequestInBackgroudCompletedStudioPic had error " + e.Error.Message);
            }
            else
            {
                Console.WriteLine("DoWebRequestInBackgroudCompletedStudioPic");

                // Either load the image or hide the control if the image is not availabe
                if (mStudioPicSource != null)
                {
                    pictureBoxStudio.LoadAsync(mStudioPicSource);
                }
                else
                {
                    // No big deal if there is no picture, just hide the control
                    pictureBoxStudio.Visible = false;
                }
            }
        }

        private void DoWebRequestInBackgroud(object sender, DoWorkEventArgs e)
        {
            mPractices = GetListPractices();
        }

        private void ItDidNotGoWell()
        {
            const string MESSAGE = "Mind Body Online is not available right now!";
            const string CAPTION = "Bummer";
            var result = MessageBox.Show(MESSAGE, CAPTION,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Information);
        }

        private void DoWebRequestInBackgroudCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                Console.WriteLine("DoWebRequestInBackgroudCompleted was canceled");
            }
            else if (!(e.Error == null))
            {
                Console.WriteLine("DoWebRequestInBackgroudCompleted had error " + e.Error.Message);
                ItDidNotGoWell();
            }
            else
            {
                Console.WriteLine("DoWebRequestInBackgroudCompleted");

                if (null == mPractices)
                {
                    // For some reason, mindbodyonline did not service my request :(
                    mSplashForm.Close();
                    ItDidNotGoWell();
                    return;
                }

                PopulateLists();
                GetFavoriteInstructors();
                GetFavoriteStudios();
                GetFavoriteClasses();
                RefreshGridSchedule();
                SetInstructorPanelWidth();
                SetSize();

                if (!mOneTimeSetSizeComplete)
                {
                    OneTimeSetWidth();
                    mOneTimeSetSizeComplete = true;
                }

                // Set numericUpDownScheduleDays accordingly
                if (mPractices.Count > 1)
                {
                    // Check the date on the last practice; that's how many we got
                    Decimal availableScheduleDays = (decimal)(mPractices[mPractices.Count - 1].when.Date - DateTime.Now.Date).Days;
                    Decimal availableScheduleDaysPast = (decimal)(mPractices[0].when.Date - DateTime.Now.Date).Days;

                    // Add 1 to account for inclusively (include both today and the last day)
                    availableScheduleDays += 1;

                    Console.WriteLine("availableScheduleDays=" + availableScheduleDays + " availableScheduleDaysPast=" + availableScheduleDaysPast);
                    numericUpDownScheduleDays.Minimum = availableScheduleDaysPast;
                    numericUpDownScheduleDays.Maximum = availableScheduleDays;
                }

                // Tooltip for refresh button shows last refresh date/time
                string lastRefresh = String.Format("Last Refresh: {0}", DateTime.Now.ToLocalTime().ToString());
                toolTipRefreshButton.SetToolTip(RefreshBtn, lastRefresh);
            }

            mSplashForm.Close();
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            if (workerWebReq.IsBusy) return; // Do not allow another update when it's already running
            DoWebRequest();
        }

        private int mStateAll = 0;
        private void labelSchedule_Click(object sender, EventArgs e)
        {
            mStateAll++;
            switch (mStateAll % 3)
            {
                case 0:
                    buttonInstructors.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                    buttonStudios.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                    buttonClasses.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                    GetFavoriteInstructors();
                    GetFavoriteStudios();
                    GetFavoriteClasses();
                    break;
                case 1:
                    buttonInstructors.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    buttonStudios.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    buttonClasses.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    panelQuickInstructor.CheckAll();
                    panelQuickStudio.CheckAll();
                    panelQuickClass.CheckAll();
                    break;
                case 2:
                    buttonInstructors.Image = global::Omtime.Properties.Resources.RedX_26x26;
                    buttonStudios.Image = global::Omtime.Properties.Resources.RedX_26x26;
                    buttonClasses.Image = global::Omtime.Properties.Resources.RedX_26x26;
                    panelQuickInstructor.CheckNone();
                    panelQuickStudio.CheckNone();
                    panelQuickClass.CheckNone();
                    break;
            }

            RefreshGridSchedule();
        }

        private int mStateInstructors = 0;
        private void buttonInstructors_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    mStateInstructors++;
                    switch (mStateInstructors % 3)
                    {
                        case 0:
                            buttonInstructors.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                            GetFavoriteInstructors();
                            break;
                        case 1:
                            buttonInstructors.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                            panelQuickInstructor.CheckAll();
                            break;
                        case 2:
                            buttonInstructors.Image = global::Omtime.Properties.Resources.RedX_26x26;
                            panelQuickInstructor.CheckNone();
                            break;
                    }

                    RefreshGridSchedule();
                    break;

                case MouseButtons.Right:
                    XmlNode xmlNodeNewFavorites = mXmlDocumentState.CreateElement("FavoriteInstructors");

                    string toolTip = "Favorites:";
                    foreach (CheckBox cb in panelQuickInstructor.Controls)
                    {
                        if (cb.Checked)
                        {
                            XmlNode xmlNode = mXmlDocumentState.CreateElement("Instructor");
                            xmlNode.InnerText = cb.Text;
                            xmlNodeNewFavorites.AppendChild(xmlNode);
                            toolTip += "\n" + cb.Text;
                        }
                    }

                    XmlNode xmlNodeOldFavorites = mXmlDocumentState.SelectSingleNode("/Omtime/FavoriteInstructors");
                    XmlNode xmlNodeRoot = mXmlDocumentState.SelectSingleNode("/Omtime");
                    xmlNodeRoot.ReplaceChild(xmlNodeNewFavorites, xmlNodeOldFavorites);

                    toolTip = (xmlNodeNewFavorites.ChildNodes.Count > 0) ? toolTip : "None";
                    //toolTipFavoriteInstructors.SetToolTip(buttonInstructors, toolTip);
                    break;
            }
        }

        private int mStateStudios = 0;
        private void buttonStudios_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    mStateStudios++;
                    switch (mStateStudios % 3)
                    {
                        case 0:
                            buttonStudios.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                            GetFavoriteStudios();
                            break;
                        case 1:
                            buttonStudios.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                            panelQuickStudio.CheckAll();
                            break;
                        case 2:
                            buttonStudios.Image = global::Omtime.Properties.Resources.RedX_26x26;
                            panelQuickStudio.CheckNone();
                            break;
                    }

                    RefreshGridSchedule();
                    break;

                case MouseButtons.Right:
                    XmlNode xmlNodeNewFavorites = mXmlDocumentState.CreateElement("FavoriteStudios");

                    string toolTip = "Favorites:";
                    foreach (CheckBox cb in panelQuickStudio.Controls)
                    {
                        if (cb.Checked)
                        {
                            XmlNode xmlNode = mXmlDocumentState.CreateElement("Studio");
                            xmlNode.InnerText = cb.Text;
                            xmlNodeNewFavorites.AppendChild(xmlNode);
                            toolTip += "\n" + cb.Text;
                        }
                    }

                    XmlNode xmlNodeOldFavorites = mXmlDocumentState.SelectSingleNode("/Omtime/FavoriteStudios");
                    XmlNode xmlNodeRoot = mXmlDocumentState.SelectSingleNode("/Omtime");
                    xmlNodeRoot.ReplaceChild(xmlNodeNewFavorites, xmlNodeOldFavorites);

                    toolTip = (xmlNodeNewFavorites.ChildNodes.Count > 0) ? toolTip : "None";
                    //toolTipFavoriteStudios.SetToolTip(buttonStudios, toolTip);
                    break;
            }
        }

        private int mStateClasses = 0;
        private void buttonClasses_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    mStateClasses++;
                    switch (mStateClasses % 3)
                    {
                        case 0:
                            buttonClasses.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                            GetFavoriteClasses();
                            break;
                        case 1:
                            buttonClasses.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                            panelQuickClass.CheckAll();
                            break;
                        case 2:
                            buttonClasses.Image = global::Omtime.Properties.Resources.RedX_26x26;
                            panelQuickClass.CheckNone();
                            break;
                    }

                    RefreshGridSchedule();
                    break;

                case MouseButtons.Right:
                    XmlNode xmlNodeNewFavorites = mXmlDocumentState.CreateElement("FavoriteClasses");

                    string toolTip = "Favorites:";
                    foreach (CheckBox cb in panelQuickClass.Controls)
                    {
                        if (cb.Checked)
                        {
                            XmlNode xmlNode = mXmlDocumentState.CreateElement("Class");
                            xmlNode.InnerText = cb.Text;
                            xmlNodeNewFavorites.AppendChild(xmlNode);
                            toolTip += "\n" + cb.Text;
                        }
                    }

                    XmlNode xmlNodeOldFavorites = mXmlDocumentState.SelectSingleNode("/Omtime/FavoriteClasses");
                    XmlNode xmlNodeRoot = mXmlDocumentState.SelectSingleNode("/Omtime");
                    xmlNodeRoot.ReplaceChild(xmlNodeNewFavorites, xmlNodeOldFavorites);

                    toolTip = (xmlNodeNewFavorites.ChildNodes.Count > 0) ? toolTip : "None";
                    //toolTipFavoriteClasses.SetToolTip(buttonClasses, toolTip);
                    break;
            }
        }

        private void checkBoxMorning_CheckedChanged(object sender, EventArgs e)
        {
            RefreshGridSchedule();
        }

        private void checkBoxAfternoon_CheckedChanged(object sender, EventArgs e)
        {
            RefreshGridSchedule();
        }

        private void checkBoxEvening_CheckedChanged(object sender, EventArgs e)
        {
            RefreshGridSchedule();
        }

        private void numericUpDownScheduleDays_ValueChanged(object sender, EventArgs e)
        {
            RefreshGridSchedule();
        }

        private void ScheduleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveXmlState();
#if CONSOLE_OUTPUT_TO_FILE
            if (mConsoleWriter != null) mConsoleWriter.Close();
            if (mConsoleFileStream != null) mConsoleFileStream.Close();
#endif
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Let's see if a double clicks feels better?
        }

        private void dataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Let's see if a single clicks feels better?
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;  // Don't do anything for header

            Practice practice;
            object value = dataGridView.Rows[e.RowIndex].DataBoundItem;
            if (value is DBNull) { return; }
            practice = (Practice)value;
            Console.WriteLine("Show details for " + practice.instructor + "'s class");

            PracticeDetailsForm practiceDetails = new PracticeDetailsForm(practice, GetCookieContainer());
            practiceDetails.Show();
        }

        private void dataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) return;  // Don't do anything for header
            if (e.Button != System.Windows.Forms.MouseButtons.Right) return;

            Practice practice;
            object value = dataGridView.Rows[e.RowIndex].DataBoundItem;
            if (value is DBNull) { return; }
            practice = (Practice)value;
            Console.WriteLine("Copy details for " + practice.instructor + "'s class to clipboard");
            String clipContent = String.Format("{0} || {1} || {2} {3} || {4} || {5}",
                practice.instructor,
                practice.studio,
                practice.when.DayOfWeek,
                practice.when,
                practice.duration,
                practice.className);
            Clipboard.SetText(clipContent);
        }

        private void pictureBoxStudio_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            mOrigBackground = pictureBoxStudio.Image;
            SetSize();
        }

        private void ScheduleForm_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        private void dataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            SetSize();
        }

        private void panelQuickInstructor_MouseEnter(object sender, EventArgs e)
        {
            panelQuickInstructor.Focus();
        }

        private void panelQuickInstructor_Click(object sender, EventArgs e)
        {
            panelQuickInstructor.Focus();
        }

        private void panelQuickInstructor_MouseLeave(object sender, EventArgs e)
        {
            // Experimental; keep the focus on the flp unless mouse actually leaves the area.
            // C# is firing this event when the mouse is moved over buttons within the flp and
            // in that case, we wish to maintain focus for continued scrollability
            bool panelQuickInstructorContainsMouse = panelQuickInstructor.ClientRectangle.Contains(PointToClient(Control.MousePosition));
            if (!panelQuickInstructorContainsMouse)
            {
                dataGridView.Focus();
            }
        }

        private void SetInstructorPanelWidth()
        {
            int width = 0;
            foreach (Control c in panelQuickInstructor.Controls)
            {
                width = (width < c.Width ? c.Width : width);
            }
#if false
            // Experimental; set all instructor button widths to be the same
            foreach (CheckBox cb in panelQuickInstructor.Controls)
            {
                cb.AutoSize = false;
                cb.Width = width;
            }
#endif
            // Account for vertical scrollbar
            width += System.Windows.Forms.SystemInformation.VerticalScrollBarWidth + panelQuickInstructor.Margin.Horizontal;

            panelQuickInstructor.HorizontalScroll.Visible = false;
            panelQuickInstructor.Width = width;
        }

        private int GetPanelHeight(Panel p)
        {
            // Experimental; setting the height should not be this hard
            int numLines = 1;
            int counter = 0;
            int controlHeight = 0;
            foreach (CheckBox cb in p.Controls)
            {
                controlHeight = cb.Height + cb.Margin.Vertical; // Set this every time?
                counter += (cb.Width + cb.Margin.Horizontal);
                if (counter > p.Width)
                {
                    counter = 0;
                    numLines++;
                }
            }
            return numLines * controlHeight;
        }

        private void OneTimeSetWidth()
        {
            int width = dataGridView.Right + 2; // 2 is the magic margin everywhere
            this.ClientSize = new System.Drawing.Size(width, this.ClientSize.Height);
        }

        private void SetSize()
        {
            const int MARGIN = 2;

            int alignment = panelQuickInstructor.Right + MARGIN;

            panelQuickInstructor.Height = this.ClientSize.Height - panelQuickInstructor.Top - MARGIN;

            panelQuickStudio.Left = alignment;
            panelQuickStudio.Width = this.ClientSize.Width - panelQuickStudio.Left - MARGIN;
            panelQuickStudio.Height = GetPanelHeight(panelQuickStudio);

            panelQuickClass.Top = panelQuickStudio.Bottom + MARGIN;
            panelQuickClass.Left = alignment;
            panelQuickClass.Width = this.ClientSize.Width - panelQuickClass.Left - MARGIN;
            panelQuickClass.Height = GetPanelHeight(panelQuickClass);

            buttonInstructors.Top = panelQuickClass.Bottom + MARGIN;
            buttonInstructors.Left = alignment;

            buttonStudios.Top = panelQuickClass.Bottom + MARGIN;
            buttonStudios.Left = buttonInstructors.Right + MARGIN;

            buttonClasses.Top = panelQuickClass.Bottom + MARGIN;
            buttonClasses.Left = buttonStudios.Right + MARGIN;

            labelSchedule.Left = buttonClasses.Right + MARGIN;
            labelSchedule.Top = buttonClasses.Bottom - labelSchedule.Height;

            dataGridView.Top = buttonClasses.Bottom + (3 * MARGIN);
            dataGridView.Left = alignment;
            SetGridViewHeightWidth(MARGIN);

            panelRefresh.Top = labelSchedule.Top;
            panelRefresh.Left = dataGridView.Right - panelRefresh.Width;
            if (panelRefresh.Left < labelSchedule.Right + 10) panelRefresh.Left = labelSchedule.Right + 10;

            bool isMinimized = (this.ClientSize.Height == 0);
            if (mOrigBackground != null && !isMinimized)
            {
                // Set the width and height and then get a scaled version of the image that fits
                pictureBoxStudio.Height = this.ClientSize.Height - dataGridView.Top - MARGIN;
                pictureBoxStudio.Width = this.ClientSize.Width - dataGridView.Left - MARGIN;
                pictureBoxStudio.Image = ScaleImage(mOrigBackground, pictureBoxStudio.Width, pictureBoxStudio.Height);

                if (pictureBoxStudio.Image != null)
                {
                    // Now position the image so its aligned at the bottom edge
                    // This allows more visibility of the image;
                    //   especially when the schedule is only 1 day (doesn't fill the whole screen)
                    int desired = this.ClientSize.Height - pictureBoxStudio.Image.Height - MARGIN;
                    pictureBoxStudio.Top = desired < dataGridView.Top ? dataGridView.Top : desired;
                    pictureBoxStudio.Left = alignment;

                    pictureBoxStudio.SendToBack();
                }
            }
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            if (newWidth == 0 || newHeight == 0) return null;

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        private XmlDocument CreateDefaultXmlState()
        {
            string fileContents = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(DEFAULT_STATE_FILE);
            StreamReader sr = new StreamReader(stream);
            fileContents = sr.ReadToEnd();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(fileContents);
            return xmlDocument;
        }

        private void CreateDefaultXmlStateClassGenres()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(DEFAULT_STATE_FILE_GENRES);
            mClassGenreDict = (SerializableDictionary<List<string>, string>)mClassGenreSerializer.Deserialize(stream);
        }

        private void GetXmlState()
        {
            mXmlDocumentState = new XmlDocument();

            try
            {
                mXmlDocumentState.Load(STATE_FILE);
            }
            catch (Exception)
            {
                Console.WriteLine("Creating default XML state...");
                mXmlDocumentState = CreateDefaultXmlState();

                // Save it right away
                mXmlDocumentState.Save(STATE_FILE);
            }

            try
            {
                TextReader textReader = new StreamReader(STATE_FILE_GENRES);
                mClassGenreDict = (SerializableDictionary<List<string>, string>)mClassGenreSerializer.Deserialize(textReader);
                textReader.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Creating default XML state for class genres...");
                CreateDefaultXmlStateClassGenres();

                // Save it right away
                XmlWriter xmlWriter = XmlWriter.Create(STATE_FILE_GENRES, mXmlWriterSettings);
                mClassGenreSerializer.Serialize(xmlWriter, mClassGenreDict);
                xmlWriter.Close();
            }

            try
            {
                XmlNode xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/NumDays");
                numericUpDownScheduleDays.Value = new decimal(Int32.Parse(xmlNode.InnerText)); // Causes numericUpDownScheduleDays_ValueChanged to be fired

                xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Morning");
                checkBoxMorning.Checked = Boolean.Parse((xmlNode.InnerText));

                xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Afternoon");
                checkBoxAfternoon.Checked = Boolean.Parse((xmlNode.InnerText));

                xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Evening");
                checkBoxEvening.Checked = Boolean.Parse((xmlNode.InnerText));

                xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/CookieValidSeconds");
                int cookieValidSeconds = Int32.Parse(xmlNode.InnerText);
                mValidCookieTime = new TimeSpan(0, 0, cookieValidSeconds);

                xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/HttpWebRequestTimeoutSeconds");
                mWebRequestTimeout = Int32.Parse(xmlNode.InnerText);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exeception in GetXmlState: " + e.ToString());
                return;
            }
        }

        private void SaveXmlState()
        {
            XmlNode xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/NumDays");
            xmlNode.InnerText = numericUpDownScheduleDays.Value.ToString();

            xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Morning");
            xmlNode.InnerText = checkBoxMorning.Checked.ToString();

            xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Afternoon");
            xmlNode.InnerText = checkBoxAfternoon.Checked.ToString();

            xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Evening");
            xmlNode.InnerText = checkBoxEvening.Checked.ToString();

            mXmlDocumentState.Save(STATE_FILE);

            // Separate file for class genres
            XmlWriter xmlWriter = XmlWriter.Create(STATE_FILE_GENRES, mXmlWriterSettings);
            mClassGenreSerializer.Serialize(xmlWriter, mClassGenreDict);
            xmlWriter.Close();
        }

        private void InitGrid()
        {
            dataGridView.AutoGenerateColumns = false;

            // Setup the columns
            dataGridView.ColumnCount = 7;
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Aqua;
            columnHeaderStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            dataGridView.Columns[0].Name = "Date";
            dataGridView.Columns[0].DataPropertyName = "when";
            dataGridView.Columns[0].DefaultCellStyle.Format = "ddd MM/d/yyyy";

            dataGridView.Columns[1].Name = "Time";
            dataGridView.Columns[1].DataPropertyName = "when";
            dataGridView.Columns[1].DefaultCellStyle.Format = "HH:mm";
            dataGridView.Columns[1].Width = 80;

            dataGridView.Columns[2].Name = "Instructor";
            dataGridView.Columns[2].DataPropertyName = "instructor";

            dataGridView.Columns[3].Name = "Reg Sched";
            dataGridView.Columns[3].DataPropertyName = "regularlyScheduledInstructor";

            dataGridView.Columns[4].Name = "Studio";
            dataGridView.Columns[4].DataPropertyName = "studio";
            dataGridView.Columns[4].Width = 105;

            dataGridView.Columns[5].Name = "Class";
            dataGridView.Columns[5].DataPropertyName = "className";
            dataGridView.Columns[5].Width = 295;

            dataGridView.Columns[6].Name = "Duration";
            dataGridView.Columns[6].DataPropertyName = "duration";
            dataGridView.Columns[6].Width = 85;
        }

        private void GetFavoriteInstructors()
        {
            // First uncheck all of them
            panelQuickInstructor.CheckNone();

            // Check our favorites
            string toolTip = "Favorites:";
            XmlNodeList xmlNodeList = mXmlDocumentState.SelectNodes("/Omtime/FavoriteInstructors/Instructor");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                CheckBox cb = panelQuickInstructor.SafeGet(xmlNode.InnerText);
                if (cb != null)
                {
                    cb.Checked = true;
                    toolTip += "\n" + xmlNode.InnerText;
                }
            }

            toolTip = (xmlNodeList.Count > 0) ? toolTip : "None";
            //toolTipFavoriteInstructors.SetToolTip(buttonInstructors, toolTip);
        }

        private void GetFavoriteStudios()
        {
            // First uncheck all of them
            panelQuickStudio.CheckNone();

            // Check our favorites
            string toolTip = "Favorites:";
            XmlNodeList xmlNodeList = mXmlDocumentState.SelectNodes("/Omtime/FavoriteStudios/Studio");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                CheckBox cb = panelQuickStudio.SafeGet(xmlNode.InnerText);
                if (cb != null)
                {
                    cb.Checked = true;
                    toolTip += "\n" + xmlNode.InnerText;
                }
            }

            toolTip = (xmlNodeList.Count > 0) ? toolTip : "None";
            //toolTipFavoriteStudios.SetToolTip(buttonStudios, toolTip);
        }

        private void GetFavoriteClasses()
        {
            // First uncheck all of them
            panelQuickClass.CheckNone();

            // Check our favorites
            string toolTip = "Favorites:";
            XmlNodeList xmlNodeList = mXmlDocumentState.SelectNodes("/Omtime/FavoriteClasses/Class");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                CheckBox cb = panelQuickClass.SafeGet(xmlNode.InnerText);
                if (cb != null)
                {
                    cb.Checked = true;
                    toolTip += "\n" + xmlNode.InnerText;
                }
            }

            toolTip = (xmlNodeList.Count > 0) ? toolTip : "None";
            //toolTipFavoriteClasses.SetToolTip(buttonClasses, toolTip);
        }

        private void RefreshGridSchedule()
        {
            TimeSpan noon = new TimeSpan(12, 0, 0);
            TimeSpan five = new TimeSpan(17, 0, 0);
            DateTime referenceDate = DateTime.Now.AddDays((double)numericUpDownScheduleDays.Value);

            // SortableBindingList is a class that uses IComparer to implement a BindingList that supports sorting
            // Credit goes to Tim Van Wassenhove
            // http://www.timvw.be/2007/02/22/presenting-the-sortablebindinglistt/
            SortableBindingList<Practice> practices = new SortableBindingList<Practice>();

            foreach (Practice practice in mPractices)
            {
                bool studio = panelQuickStudio.SafeGet(practice.studio).Checked;
                bool instructor = panelQuickInstructor.SafeGet(practice.instructor).Checked;

                if (!instructor && (practice.regularlyScheduledInstructor != null))
                {
                    instructor = panelQuickInstructor.SafeGet(practice.regularlyScheduledInstructor).Checked;
                }

                bool classGenre = panelQuickClass.SafeGet(practice.classGenre).Checked;

                // Allow past schedule
                bool date = (referenceDate.Date < DateTime.Now.Date) ?
                    practice.when.Date < DateTime.Now.Date && practice.when.Date >= referenceDate.Date :
                    practice.when.Date >= DateTime.Now.Date && practice.when.Date < referenceDate.Date;

                bool morning = (checkBoxMorning.Checked ? practice.when.TimeOfDay < noon : false);
                bool afternoon = (checkBoxAfternoon.Checked ? practice.when.TimeOfDay >= noon && practice.when.TimeOfDay < five : false);
                bool evening = (checkBoxEvening.Checked ? practice.when.TimeOfDay >= five : false);
                bool time = morning || afternoon || evening;
                bool includeClass = studio && instructor && classGenre && date && time;
                if (includeClass)
                {
                    practices.Add(practice);
                }
            }

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = practices;
            dataGridView.DataSource = bindingSource;
            dataGridView.Refresh();

            // Oscilate the background color to distinguish days of the week
            DateTime dateTimeIterator = DateTime.Now;
            Color color1 = Color.FromArgb(240, 240, 240); // Lighter than light grey
            Color color2 = Color.White;
            Color theColor = color1; // Start with lighter than light grey

            // Strikeout any cancelled classes
            DataGridViewCellStyle strikeoutStyle = new DataGridViewCellStyle();
            strikeoutStyle.Font = new Font(dataGridView.Font.OriginalFontName, dataGridView.Font.Size, FontStyle.Strikeout);

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                DateTime dateTime = (DateTime)row.Cells["Date"].Value;
                if (dateTime.Date == dateTimeIterator.Date)
                {
                    row.DefaultCellStyle.BackColor = theColor;
                }
                else
                {
                    dateTimeIterator = dateTime;
                    theColor = (theColor == color1) ? color2 : color1;
                    row.DefaultCellStyle.BackColor = theColor;
                }

                if (((Practice)row.DataBoundItem).isCancelled)
                {
                    row.DefaultCellStyle.ApplyStyle(strikeoutStyle);
                }
            }

            // Set the labels with count
            labelSchedule.Text = "Schedule (" + practices.Count + "):";

            // Experimental; set focus to the grid
            // We want to prevent mouse scroll from numericUpDownScheduleDays
            dataGridView.Focus();

            // Change the size of the datagridview to make room for a studio photo
            SetGridViewHeightWidth(2);
        }

        private void SetGridViewHeightWidth(int margin)
        {
            // Height
            int max = this.ClientSize.Height - dataGridView.Top - margin;
            int desired = dataGridView.RowTemplate.Height * (dataGridView.Rows.Count + 1) + 1;
            dataGridView.Height = desired > max ? max : desired;

            // Width
            // This is sort of hacky, but we check if the scrollbar is visible by the controls within dgv
            //    Control 0 = Horizontal Scroll Bar
            //    Control 1 = Vertical Scroll Bar
            // 3 is the magic margin here
            bool verticalScrollVisible = dataGridView.Controls[1].Visible;
            desired = verticalScrollVisible ? SystemInformation.VerticalScrollBarWidth + 3 : 3;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                desired += column.Width;
            }
            max = this.ClientSize.Width - dataGridView.Left - margin;
            dataGridView.Width = desired > max ? max : desired;
        }

        private void quickInstructorButton_Click(object sender, EventArgs e)
        {
            RefreshGridSchedule();
        }

        private void quickStudioButton_Click(object sender, EventArgs e)
        {
            RefreshGridSchedule();
        }

        private void quickClassButton_Click(object sender, EventArgs e)
        {
            string classGenre = ((CheckBox)sender).Text;

            // Happens on the first button press when we enable this mode
            if (mClassGenreAssimilationSource == classGenre && mClassGenreAssimilationMode == true) return;

            if (mClassGenreAssimilationMode)
            {
                DoClassAssimilation(classGenre);
                mClassGenreAssimilationMode = false;
                Console.WriteLine("mClassGenreAssimilationMode = false");
                mClassGenreAssimilationSource = string.Empty;
            }
            else
            {
                RefreshGridSchedule();
            }
        }

        private void quickInstructorButton_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    buttonInstructors.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                    buttonStudios.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    buttonClasses.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    panelQuickInstructor.CheckNone();
                    panelQuickStudio.CheckAll();
                    panelQuickClass.CheckAll();
                    CheckBox checkBox = (CheckBox)sender;
                    checkBox.Checked = true;
                    RefreshGridSchedule();
                    break;
            }
        }

        private void quickStudioButton_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    buttonInstructors.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    buttonStudios.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                    buttonClasses.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    panelQuickInstructor.CheckAll();
                    panelQuickStudio.CheckNone();
                    panelQuickClass.CheckAll();
                    CheckBox checkBox = (CheckBox)sender;
                    checkBox.Checked = true;
                    RefreshGridSchedule();
                    break;
            }
        }

        private void quickClassButton_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    buttonInstructors.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    buttonStudios.Image = global::Omtime.Properties.Resources.GreenCheck_26x26;
                    buttonClasses.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
                    panelQuickInstructor.CheckAll();
                    panelQuickStudio.CheckAll();
                    panelQuickClass.CheckNone();
                    CheckBox checkBox = (CheckBox)sender;
                    checkBox.Checked = true;
                    RefreshGridSchedule();
                    break;

                case MouseButtons.Left:
                    mClassButtonDown = false;
                    break;
            }
        }

        private void DoClassAssimilation(string classGenre)
        {
            string className = string.Empty;
            foreach (Practice practice in mPractices)
            {
                if (practice.classGenre == mClassGenreAssimilationSource)
                {
                    className = practice.className;
                    break;
                }
            }

            foreach (List<string> list in mClassGenreDict.Keys)
            {
                if (mClassGenreDict[list] == classGenre)
                {
                    Console.WriteLine(String.Format("Assimilate {0} with {1}", className, classGenre));
                    list.Add(className);
                }
            }
        }

        private void quickInstructorButton_MouseDown(object sender, MouseEventArgs e)
        {
            // NOOP, consider removing
        }

        private void quickStudioButton_MouseDown(object sender, MouseEventArgs e)
        {
            // NOOP, consider removing
        }

        private void OnClassGenreAssimilationModeTimer(Object myObject, EventArgs myEventArgs)
        {
            CheckBox cb = (CheckBox)mClassGenreAssimilationModeTimer.Tag;
            mClassGenreAssimilationModeTimer.Stop();
            if (mClassButtonDown)
            {
                mClassGenreAssimilationMode = true;
                cb.BackColor = Color.Red;
                cb.FlatAppearance.CheckedBackColor = Color.Red;
                cb.FlatAppearance.MouseDownBackColor = Color.Red;
                cb.FlatAppearance.MouseOverBackColor = Color.Red;
                Console.WriteLine(String.Format("mClassGenreAssimilationMode = true; source = {0}", cb.Text));
                mClassGenreAssimilationSource = cb.Text;
            }
            else
            {
                cb = null;
            }
        }

        private void quickClassButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || mClassGenreAssimilationMode)
            {
                return;
            }

            mClassGenreAssimilationModeTimer.Stop();
            mClassGenreAssimilationModeTimer.Start();
            mClassGenreAssimilationModeTimer.Tag = sender; // Keep a reference to this button; we check it when the timer fires
            mClassButtonDown = true;
        }

        private Control GetQuickButton(string text, EventHandler ehClick, MouseEventHandler mehMouseUp, MouseEventHandler mehMouseDown)
        {
            CheckBox b = new CheckBox();
            b.Appearance = Appearance.Button;
            b.AutoEllipsis = false;
            b.AutoSize = true;
            b.BackColor = System.Drawing.Color.Yellow;
            b.Click += ehClick;
            b.FlatAppearance.BorderColor = System.Drawing.Color.Orange;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.CheckedBackColor = System.Drawing.Color.Orange;
            b.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
            b.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
            b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            b.Location = new System.Drawing.Point(0, 0);
            b.Margin = new Padding(1); // This will be the space between these buttons in the FlowLayoutPanel
            b.MouseDown += mehMouseDown;
            b.MouseUp += mehMouseUp;
            b.Name = text;
            b.Text = text;
            b.UseMnemonic = false; // We do not care about this and we want an easy way to use &
            b.UseVisualStyleBackColor = true;
            return b;
        }

        private void PopulateLists()
        {
            // Clear/remove/dispose all items
            panelQuickInstructor.Controls.Clear();
            panelQuickStudio.Controls.Clear();
            panelQuickClass.Controls.Clear();

            SortedList<string, Control> sortedListInstructors = new SortedList<string, Control>();
            SortedList<string, Control> sortedListStudios = new SortedList<string, Control>();
            SortedList<string, Control> sortedListClasses = new SortedList<string, Control>();

            // Add items to the lists (instructors, studios, classes)
            foreach (Practice practice in mPractices)
            {
                if (!sortedListInstructors.ContainsKey(practice.instructor))
                {
                    Control c = GetQuickButton(
                        practice.instructor,
                        new EventHandler(quickInstructorButton_Click),
                        new MouseEventHandler(quickInstructorButton_MouseUp),
                        new MouseEventHandler(quickInstructorButton_MouseDown));
                    sortedListInstructors.Add(practice.instructor, c);
                }

                if (practice.regularlyScheduledInstructor != null && !sortedListInstructors.ContainsKey(practice.regularlyScheduledInstructor))
                {
                    Control c = GetQuickButton(
                        practice.regularlyScheduledInstructor,
                        new EventHandler(quickInstructorButton_Click),
                        new MouseEventHandler(quickInstructorButton_MouseUp),
                        new MouseEventHandler(quickInstructorButton_MouseDown));
                    sortedListInstructors.Add(practice.regularlyScheduledInstructor, c);
                }

                if (!sortedListStudios.ContainsKey(practice.studio))
                {
                    Control c = GetQuickButton(
                        practice.studio,
                        new EventHandler(quickStudioButton_Click),
                        new MouseEventHandler(quickStudioButton_MouseUp),
                        new MouseEventHandler(quickStudioButton_MouseDown));
                    sortedListStudios.Add(practice.studio, c);
                }

                if (!sortedListClasses.ContainsKey(practice.classGenre))
                {
                    Control c = GetQuickButton(
                        practice.classGenre,
                        new EventHandler(quickClassButton_Click),
                        new MouseEventHandler(quickClassButton_MouseUp),
                        new MouseEventHandler(quickClassButton_MouseDown));
                    sortedListClasses.Add(practice.classGenre, c);
                }
            }

            // Hackey, but it works: copy the sorted list to an array of Control, then add the range.
            // All of this is to sort the lists

            Control[] sortedArrayInstructors = new Control[sortedListInstructors.Count];
            Control[] sortedArrayStudios = new Control[sortedListStudios.Count];
            Control[] sortedArrayClasses = new Control[sortedListClasses.Count];

            sortedListInstructors.Values.CopyTo(sortedArrayInstructors, 0);
            sortedListStudios.Values.CopyTo(sortedArrayStudios, 0);
            sortedListClasses.Values.CopyTo(sortedArrayClasses, 0);

            panelQuickInstructor.Controls.AddRange(sortedArrayInstructors);
            panelQuickStudio.Controls.AddRange(sortedArrayStudios);
            panelQuickClass.Controls.AddRange(sortedArrayClasses);

            // Log each instructor name to the logfile to help with choosing a name for Thrice
            foreach (var v in sortedListInstructors.Keys)
            {
                // Console.WriteLine($"{v}");
            }
        }

        private List<Practice> GetListPractices()
        {
            XmlNode xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/ScheduleFetchWeeks");
            int scheduleFetchWeeks = Int32.Parse(xmlNode.InnerText);
            Console.WriteLine("Get practices for the next {0} weeks", scheduleFetchWeeks);

            List<Practice> practices = new List<Practice>();
            DateTime dateTime = DateTime.Now;
            for (int i = 0; i < scheduleFetchWeeks; i++)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://clients.mindbodyonline.com/classic/mainclass");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
                httpWebRequest.CookieContainer = GetCookieContainer();
                httpWebRequest.Timeout = 1000 * mWebRequestTimeout;

                string formData = "";
                // CorePower original
                formData += "pageNum=1";
                formData += "&requiredtxtUserName=";
                formData += "&requiredtxtPassword=";
                formData += "&optForwardingLink=";
                formData += "&optRememberMe=";
                formData += "&tabID=7";
                formData += "&filterByClsSch=";
                formData += "&prevFilterByClsSch=-1";
                formData += "&prevFilterByClsSch2=-2";
                formData += "&optLocation=0";
                formData += "&optTG=0";
                formData += "&optVT=0";
                formData += "&useClassLogic="; // Returns one week of data
                formData += "&optView=week";
                formData += "&optInstructor=0";
                formData += String.Format("&txtDate={0}%2F{1}%2F{2}", dateTime.Month, dateTime.Day, dateTime.Year);

                // Orange Theory
                /*
                formData += "&tg=";
                formData += "&vt=";
                formData += "&lvl=";
                formData += "&stype=";
                formData += "&view=";
                formData += "&trn=0";
                formData += "&page=";
                formData += "&catid=";
                formData += "&prodid=";
                formData += String.Format("&date={0}%2F{1}%2F{2}", dateTime.Month, dateTime.Day, dateTime.Year);
                formData += "&classid=0";
                formData += "&prodGroupId=";
                formData += "&sSU=";
                formData += "&optForwardingLink=";
                formData += "&qParam=";
                formData += "&justloggedin=";
                formData += "&nLgIn=";
                formData += "&pMode=0";
                 */

                byte[] bytedata = Encoding.UTF8.GetBytes(formData);
                httpWebRequest.ContentLength = bytedata.Length;
                Console.WriteLine("Form Data (" + httpWebRequest.ContentLength + " bytes): " + formData);

                Stream requestStream;
                try
                {
                    requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(bytedata, 0, bytedata.Length);
                    requestStream.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in httpWebRequest.GetResponse: " + e.ToString());
                    return null;
                }

                HtmlAgilityPack.HtmlDocument htmlDocument = GetHtmlDocument(httpWebRequest);
                if (null == htmlDocument) return null; // mindbodyonline is down
                practices.AddRange(GetListPracticesFromHtmlDocument(htmlDocument));
                dateTime = dateTime.AddDays(7); // One week later
            }

            return practices;
        }

        private CookieContainer GetCookieContainer()
        {
            if (DateTime.Now < mCookieContainerExpires)
            {
                return mCookieContainer;
            }

            XmlNode xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Studios");
            string studioid = xmlNode.Attributes["selectedId"].Value;
            Console.WriteLine("Cookie expired at " + mCookieContainerExpires.ToString() + "; get a new one for studio ID " + studioid + "...");

            // Make a simple request to default page to obtain new cookie
            string requestUri = "https://clients.mindbodyonline.com/classic/home?studioid=" + studioid;
            mCookieContainer = new CookieContainer();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
            httpWebRequest.CookieContainer = mCookieContainer;

            Console.WriteLine("Get cookie from: " + httpWebRequest.RequestUri.AbsoluteUri);

            // August 2018 - It looks like MBO started to enforce stronger security.
            // We were getting this exception here:
            //    "The request was aborted: Could not create SSL/TLS secure channel."
            // Resolve by setting SecurityProtocol to Tls12
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            // Print the properties of each cookie
            foreach (Cookie cook in mCookieContainer.GetCookies(new Uri(requestUri)))
            {
                Console.WriteLine("Cookie:");
                Console.WriteLine("{0} = {1}", cook.Name, cook.Value);
                Console.WriteLine("Domain: {0}", cook.Domain);
                Console.WriteLine("Path: {0}", cook.Path);
                Console.WriteLine("Port: {0}", cook.Port);
                Console.WriteLine("Secure: {0}", cook.Secure);

                Console.WriteLine("When issued: {0}", cook.TimeStamp);
                Console.WriteLine("Expires: {0} (expired? {1})",
                    cook.Expires, cook.Expired);
                Console.WriteLine("Don't save: {0}", cook.Discard);
                Console.WriteLine("Comment: {0}", cook.Comment);
                Console.WriteLine("Uri for comments: {0}", cook.CommentUri);
                Console.WriteLine("Version: RFC {0}", cook.Version == 1 ? "2109" : "2965");

                // Show the string representation of the cookie.
                Console.WriteLine("String: {0}", cook.ToString());
            }

            // Set new expiration
            mCookieContainerExpires = DateTime.Now + mValidCookieTime;
            Console.WriteLine("New cookie expiration: " + mCookieContainerExpires.ToString());

            return mCookieContainer;
        }

        // Covert a "listed dictionary" to a "flat dictionary"
        private Dictionary<string, string> GetFlatDict(SerializableDictionary<List<string>, string> sd)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            foreach (List<string> ls in sd.Keys)
            {
                foreach (string s in ls)
                {
                    d.Add(s, sd[ls]);
                }
            }
            return d;
        }

        private List<Practice> GetListPracticesFromHtmlDocument(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            // City Specific
            XmlNode xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Studios");
            string studioId = xmlNode.Attributes["selectedId"].Value;
            xmlNode = mXmlDocumentState.SelectSingleNode(String.Format("/Omtime/Studios/Studio[@id='{0}']", studioId));
            string citySpecific = xmlNode == null ? "" : xmlNode.Attributes["citySpecific"].Value;

            // Enable class genres - this allows us to show an aggregrated list of classes
            xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/ClassGenresEnabled");
            bool enableGenres = Boolean.Parse(xmlNode.InnerText);

            // EXPERIMENTAL: View past schedule with a negative numericUpDownScheduleDays if the XML state allows this feature
            xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/AllowPastSchedule");
            bool allowPastSchedule = Boolean.Parse(xmlNode.InnerText);

            Dictionary<string, string> flatDictClassGenres = GetFlatDict(mClassGenreDict);

            // Get the headings
            int iWhen, iClassName, iInstructor, iStudio, iDuration;
            iWhen = iClassName = iInstructor = iStudio = iDuration = -1;
            int index = 0;
            foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//table[@id='classSchedule-mainTable']//thead/tr/th"))
            {
                switch (node.Attributes["id"].Value)
                {
                    case "startTimeHeader":
                        iWhen = index;
                        break;
                    case "classNameHeader":
                        iClassName = index;
                        break;
                    case "trainerNameHeader":
                        iInstructor = index;
                        break;
                    case "locationNameHeader":
                        iStudio = index;
                        break;
                    case "durationHeader":
                        iDuration = index;
                        break;
                    default:
                        break;
                }
                //Console.WriteLine(String.Format("{0}: {1}", index, node.Attributes["id"].Value));
                index++;
            }

            // Console.WriteLine("Each tr:");
            DateTime lastDateTime = new DateTime();
            List<Practice> practices = new List<Practice>();
            foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//table[@id='classSchedule-mainTable']//tr"))
            {
                HtmlAttribute classAttribute = node.Attributes["class"];
                HtmlAttribute styleAttribute = node.Attributes["style"];

                if (classAttribute == null && styleAttribute == null)
                {
                    string s = node.InnerHtml.Substring(node.InnerHtml.IndexOf("</span>") + 7);
                    s = s.Remove(s.IndexOf("&nbsp;"));
                    bool b = DateTime.TryParse(s, out lastDateTime);
                    continue;
                }

                if (styleAttribute != null && styleAttribute.Value == "background-color:#F2F2F2;")
                {
                    // Console.WriteLine("no scheduled classes or training sessions");
                    continue;
                }

                if (classAttribute != null && classAttribute.Value == "evenRow" || classAttribute.Value == "oddRow")
                {
                    Practice p = new Practice();
                    p.isCancelled = (node.OuterHtml.IndexOf("Cancelled Today") > 0) ? true : false;

                    HtmlNodeCollection nodeTDs = node.SelectNodes("td");

                    string time = nodeTDs[iWhen].InnerText.Trim();
                    time = time.Replace("&nbsp;", "");
                    DateTime timeOffset;
                    bool goodParse = DateTime.TryParse(time, out timeOffset);
                    p.when = goodParse ? lastDateTime.Add(timeOffset.TimeOfDay) : lastDateTime;

                    // Discard classes from the past, but keep all of today
                    if (!allowPastSchedule && (p.when.Date < DateTime.Now.Date))
                    {
                        // NOTE: This is redundant here because we also check the date in RefreshGridSchedule
                        //       Keeping this 'continue' here allows a shorter list of pracices as the week goes on. Faster?
                        continue;
                    }

                    // Save class name
                    p.className = nodeTDs[iClassName].InnerText.Trim();

                    // Save a class genre - application users can specify genres to narrow down the list of displayed classes
                    if (!enableGenres)
                    {
                        // Maybe a hack; if user wants no genres, then just set the real name here.
                        // All other code already references classGenre
                        p.classGenre = p.className;
                    }
                    else if (flatDictClassGenres.ContainsKey(p.className))
                    {
                        p.classGenre = flatDictClassGenres[p.className];
                    }
                    else
                    {
#if true // This really does look better!
                        int stop = p.className.IndexOf("–"); // long dash
                        if (stop == -1) stop = p.className.IndexOf("-"); // short dash
                        p.classGenre = (stop > 0 ? p.className.Substring(0, stop) : p.className);
                        p.classGenre = p.classGenre.Trim();

                        // Special case for all free classes; experimental, how does it feel? Too vague?
                        if (p.classGenre.ToLower().Contains("free"))
                        {
                            p.classGenre = "FREE";
                        }
#else
                        p.classGenre = p.className;
#endif
                    }

                    try
                    {
                        if (nodeTDs[iClassName].FirstChild.Attributes["name"] != null)
                        {
                            p.classId = nodeTDs[iClassName].FirstChild.Attributes["name"].Value.Replace("cid", "");
                        }
                    }
                    catch (Exception) { }

                    if (p.isCancelled)
                    {
                        const string CANCELLED = "Cancelled";
                        p.instructor = CANCELLED;
                    }
                    else
                    {
                        // Account for subs
                        // It looks like this:
                        // <a title=\"Regularly ScheduledSommer\"><span style=\"color:#990000;\">(9)</span></a>

                        const string REGULARLY_SCHEDULED = "Regularly Scheduled";
                        int regularlyScheduledIndex = nodeTDs[iInstructor].InnerHtml.IndexOf(REGULARLY_SCHEDULED);
                        if (regularlyScheduledIndex > 0)
                        {
                            p.instructor = nodeTDs[iInstructor].InnerText.Trim();

                            // Remove (xx)
                            // (xx) is the index to a list at the bottom of the page showing the regularly scheduled instructor
                            p.instructor = p.instructor.Substring(0, p.instructor.IndexOf('('));
                            p.instructor = p.instructor.Trim();

                            // Get the regularly scheduled instructor
                            int start = regularlyScheduledIndex + REGULARLY_SCHEDULED.Length;
                            int length = nodeTDs[iInstructor].InnerHtml.IndexOf('\"', start) - start;
                            p.regularlyScheduledInstructor = nodeTDs[iInstructor].InnerHtml.Substring(start, length);

                            // Console.WriteLine(p.instructor + " is subbing for " + p.regularlyScheduledInstructor);
                        }
                        else
                        {
                            p.instructor = nodeTDs[iInstructor].InnerText.Trim();
                        }
                        p.instructor = p.instructor.Replace("&nbsp;", "");

                        // Sometimes they omit the instructor name, maybe still looking for someone?
                        const string STAFF = "STAFF"; // Sounds like an infection
                        if (string.IsNullOrEmpty(p.instructor))
                        {
                            p.instructor = STAFF;
                        }

                        try
                        {
                            if (nodeTDs[iInstructor].FirstChild.Attributes["name"] != null)
                            {
                                p.mindBodyOnlineId = nodeTDs[iInstructor].FirstChild.Attributes["name"].Value.Replace("bio", "");
                            }
                        }
                        catch (Exception) { }
                    }

                    // Some MBO (such as Orange) do not include studio in their results
                    //    Instead they create a dediated studio ID for ea studio
                    if (iStudio > 0)
                    {
                        p.studio = p.isCancelled ?
                            nodeTDs[iStudio - 1].SelectNodes("td")[0].InnerText :
                            nodeTDs[iStudio].InnerText.Trim();
                        p.studio = p.studio.Replace("&nbsp;", "");
                    }
                    else
                    {
                        // Just use the studio name
                        string selectedStudioId = mXmlDocumentState.SelectSingleNode("/Omtime/Studios").Attributes["selectedId"].Value;
                        xmlNode = mXmlDocumentState.SelectSingleNode(String.Format("/Omtime/Studios/Studio[@id='{0}']", selectedStudioId));
                        p.studio = xmlNode == null ? "Studio " + selectedStudioId : xmlNode.InnerText;
                    }

                    // City Specific
                    if (citySpecific.Length > 0)
                    {
                        if (!p.studio.StartsWith(citySpecific))
                        {
                            // Omit unwanted cities
                            continue;
                        }
                        else
                        {
                            // Remove the city prefix
                            p.studio = p.studio.Substring(citySpecific.Length);
                            p.studio = p.studio.Trim();
                            p.studio = p.studio.Trim('-');
                        }
                    }

                    // Some studios (Y6) has an inconsistent cancelled duration that always causes trouble
                    // To work around this, we try/catch and default to 0 hours 0 minutes
                    string duration = "";
                    try
                    {
                        duration = (p.isCancelled) ?
                            nodeTDs[iDuration - 2].SelectNodes("td")[1].InnerText.Trim() :
                            nodeTDs[iDuration].InnerText.Trim();
                        duration = duration.Replace("&nbsp;", "");
                        duration = duration.Replace("hours", "hour");
                        duration = duration.Replace("minutes", "minute");
                    }
                    catch (Exception) { }
                    int hours = 0, minutes = 0;
                    string[] durationEements = duration.Split('&');
                    switch (durationEements.Length)
                    {
                        case 1:
                            try
                            {
                                hours = Int32.Parse(durationEements[0].Replace("hour", ""));
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    minutes = Int32.Parse(durationEements[0].Replace("minute", ""));
                                }
                                catch (Exception) { }
                            }
                            break;
                        case 2:
                            hours = Int32.Parse(durationEements[0].Replace("hour", ""));
                            minutes = Int32.Parse(durationEements[1].Replace("minute", ""));
                            break;
                        default:
                            break;
                    }
                    p.duration = new TimeSpan(hours, minutes, 0);

                    practices.Add(p);
                }

                //Console.WriteLine("Unknown tr:");
                //Console.WriteLine(node.InnerHtml);
            }

            Console.WriteLine("Number of practices: " + practices.Count);

            return practices;
        }

        private HtmlAgilityPack.HtmlDocument GetHtmlDocument(HttpWebRequest httpWebRequest)
        {
#if false
            Console.WriteLine("Request headers:");
            foreach (string s in httpWebRequest.Headers.AllKeys)
            {
                Console.WriteLine(string.Format("   {0}: {1}", s, httpWebRequest.Headers.Get(s)));
            }
#endif

            HttpWebResponse httpWebResponse = null;
            try
            {
                Console.WriteLine("Get HTML Document from {0}...", httpWebRequest.RequestUri.AbsoluteUri);
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in httpWebRequest.GetResponse: " + ex.ToString());
                return null;
            }

            Console.WriteLine("Response attributes: ContentType={0} ContentLength={1} StatusCode={2} StatusDescription={3}",
                httpWebResponse.ContentType,
                httpWebResponse.ContentLength,
                httpWebResponse.StatusCode,
                httpWebResponse.StatusDescription);

            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string response = reader.ReadToEnd();

            responseStream.Close();
            httpWebResponse.Close();

#if false
            string filename = "HtmlResponse.html";
            Console.WriteLine("Raw HTML Document written to file " + filename);
            File.WriteAllText(filename, response);
#endif

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(response);
            return htmlDocument;
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            // Save oldStudioID
            XmlNode xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Studios");
            string oldStudioID = xmlNode.Attributes["selectedId"].Value;

            SettingsForm settings = new SettingsForm(mXmlDocumentState);
            settings.ShowDialog();
            if (settings.DialogResult == DialogResult.OK)
            {
                // Get new cookie only if studio ID changed
                xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Studios");
                string newStudioID = xmlNode.Attributes["selectedId"].Value;
                if (!oldStudioID.Equals(newStudioID))
                {
                    Console.WriteLine("Force new cookie because it's a function of studio ID which just changed from " + oldStudioID + " to " + newStudioID);
                    mCookieContainerExpires = DateTime.Now;
                }

                // Perform refresh
                if (workerWebReq.IsBusy) return; // Do not allow another update when it's already running
                DoWebRequest();
            }
        }

        private void ScheduleForm_KeyUp(object sender, KeyEventArgs e)
        {
            // Perform refresh on F5
            if (e.KeyCode.Equals(Keys.F5) && !workerWebReq.IsBusy)
            {
                DoWebRequest();
            }

            // Let the user scroll through the photos using the arrow keys
            if (e.KeyCode.Equals(Keys.Right))
            {
                m_picIndex++;
                if (m_picIndex >= m_picList.Count) m_picIndex = 0;
                mStudioPicSource = m_picList[m_picIndex];
                pictureBoxStudio.LoadAsync(mStudioPicSource);
            }

            if (e.KeyCode.Equals(Keys.Left))
            {
                m_picIndex--;
                if (m_picIndex <= 0) m_picIndex = m_picList.Count - 1;
                mStudioPicSource = m_picList[m_picIndex];
                pictureBoxStudio.LoadAsync(mStudioPicSource);
            }
        }

        private void MyAccountBtn_Click(object sender, EventArgs e)
        {
#if true // This is not complete yet! Find some time, and finish it, Mike!
            MyAccountForm myAct = new MyAccountForm();
            myAct.ShowDialog();
            if (myAct.DialogResult == DialogResult.OK)
            {
            }
#endif
        }
    }
}