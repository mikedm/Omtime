using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Omtime
{
    public partial class MyAccountForm : Form
    {
        public class Visit
        {
            public DateTime when { get; set; }
            public string teacher { get; set; }
            public string studio { get; set; }
            public string classType { get; set; }
        }

        public List<Visit> mVisits = new List<Visit>();

        private CookieContainer mCookieContainer;
        private DateTime mCookieContainerExpires = new DateTime(0);
        private TimeSpan mValidCookieTime;

        public MyAccountForm()
        {
            mValidCookieTime = new TimeSpan(0, 10, 0);
            InitializeComponent();
        }

        private void MyAccountForm_Load(object sender, EventArgs e)
        {
            HtmlAgilityPack.HtmlDocument htmlDocument;
#if true
            MyAccountDoLogin();
            htmlDocument = GetVisitHistory();
#else
            string filename = "..\\..\\Sample Data\\HtmlResponseMyAccountVisitHistory.html";
            htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(File.ReadAllText(filename));
#endif
            ParseVisitHistory(htmlDocument);
            PopulateLabels();

            InitGrid();
        }

        private void InitGrid()
        {
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.AutoGenerateColumns = false;
            dgvHistory.ColumnCount = 5;

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Aqua;
            columnHeaderStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
            dgvHistory.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            dgvHistory.Columns[0].Name = "Date";
            dgvHistory.Columns[0].DataPropertyName = "when";
            dgvHistory.Columns[0].DefaultCellStyle.Format = "ddd MM/d/yyyy";

            dgvHistory.Columns[1].Name = "Time";
            dgvHistory.Columns[1].DataPropertyName = "when";
            dgvHistory.Columns[1].DefaultCellStyle.Format = "HH:mm";
            dgvHistory.Columns[1].Width = 80;

            dgvHistory.Columns[2].Name = "Guru";
            dgvHistory.Columns[2].DataPropertyName = "teacher";

            dgvHistory.Columns[3].Name = "Studio";
            dgvHistory.Columns[3].DataPropertyName = "studio";

            dgvHistory.Columns[4].Name = "Class";
            dgvHistory.Columns[4].DataPropertyName = "classType";
            dgvHistory.Columns[4].Width = 105;

            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvHistory.DataSource = mVisits;
            SetGridViewHeightWidth();
        }

        private void SetGridViewHeightWidth()
        {
            // Width
            // This is sort of hacky, but we check if the scrollbar is visible by the controls within dgv
            //    Control 0 = Horizontal Scroll Bar
            //    Control 1 = Vertical Scroll Bar
            // 3 is the magic margin here
            bool verticalScrollVisible = dgvHistory.Controls[1].Visible;
            int desired = verticalScrollVisible ? SystemInformation.VerticalScrollBarWidth + 3 : 3;

            foreach (DataGridViewColumn column in dgvHistory.Columns)
            {
                desired += column.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
            }
            dgvHistory.Width = desired;
            this.ClientSize = new Size(dgvHistory.Width, this.ClientSize.Height);
        }

        private void GetCounts()
        {
            Dictionary<string, int> dictTeacher = new Dictionary<string, int>();
            Dictionary<string, int> dictStudio = new Dictionary<string, int>();

            foreach (Visit visit in mVisits)
            {
                if (!dictTeacher.ContainsKey(visit.teacher)) dictTeacher.Add(visit.teacher, 0);
                dictTeacher[visit.teacher]++;

                if (!dictStudio.ContainsKey(visit.studio)) dictStudio.Add(visit.studio, 0);
                dictStudio[visit.studio]++;
            }

            labelClasses.Text = mVisits.Count.ToString();
            labelGurus.Text = dictTeacher.Count.ToString();
            labelLocations.Text = dictStudio.Count.ToString();

            labelClasses.Left = ((labelClassesTitle.Width - labelClasses.Width) / 2) + labelClassesTitle.Left;
            labelGurus.Left = ((labelGurusTitle.Width - labelGurus.Width) / 2) + labelGurusTitle.Left;
            labelLocations.Left = ((labelLocationsTitle.Width - labelLocations.Width) / 2) + labelLocationsTitle.Left;
        }

        private void PopulateLabels()
        {
            GetCounts();
        }

        private void ParseVisitHistory(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            int iDate = 0;
            int iTime = 2;
            int iTeacher = 3;
            int iStudio = 4;
            int iPayMeth = 5;
            int iClassType = 6;
            int iStatus = 7;
            int iWeb = 8;
            int iPayRef = 9;

            foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//table[@class='myInfoTable']//tr"))
            {
                HtmlAttribute classAttribute = node.Attributes["class"];
                HtmlAttribute styleAttribute = node.Attributes["style"];

                if (classAttribute != null && classAttribute.Value == "tableHeader")
                {
                    continue; // skip the header row
                }

                HtmlNodeCollection nodeTDs = node.SelectNodes("td");

                Visit visit = new Visit();

                DateTime date;
                string d = nodeTDs[iDate].InnerText.Trim();
                string t = nodeTDs[iTime].InnerText.Trim();
                t = t.Replace("&nbsp;", "");
                bool goodParse = DateTime.TryParse(String.Format("{0} {1}", d, t), out date);

                visit.when = date;
                visit.teacher = nodeTDs[iTeacher].InnerText.Trim();
                visit.studio = nodeTDs[iStudio].InnerText.Trim();
                string payMeth = nodeTDs[iPayMeth].InnerText.Trim();
                visit.classType = nodeTDs[iClassType].InnerText.Trim();
                string status = nodeTDs[iStatus].InnerText.Trim();
                string web = nodeTDs[iWeb].InnerText.Trim();
                string payRef = nodeTDs[iPayRef].InnerText.Trim();

#if false
                Console.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
                    date.ToString(), date.DayOfWeek, visit.teacher, visit.studio, payMeth, visit.classType, status, web, payRef);
#endif
                mVisits.Add(visit);
            }
        }

        private HtmlAgilityPack.HtmlDocument GetVisitHistory()
        {
            Console.WriteLine("Get visit history");

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://clients.mindbodyonline.com/ASP/my_vh.asp");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";  //"application/x-www-form-urlencoded";
            httpWebRequest.CookieContainer = GetCookieContainer();
            httpWebRequest.Timeout = 1000 * 10;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";

            Stream requestStream;
            try
            {
                requestStream = httpWebRequest.GetRequestStream();
                requestStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in httpWebRequest.GetResponse: " + e.ToString());
                return null;
            }

            HtmlAgilityPack.HtmlDocument htmlDocument = GetHtmlDocument(httpWebRequest);
            return htmlDocument;
        }

        private void MyAccountDoLogin()
        {
            Console.WriteLine("Get my account data for {0} ", "mikedm");

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://clients.mindbodyonline.com/Login?studioID=1419&isLibAsync=true&isJson=true");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";  //"application/x-www-form-urlencoded";
            httpWebRequest.CookieContainer = GetCookieContainer();
            httpWebRequest.Timeout = 1000 * 10;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";

            string formData = "";
            formData += "requiredtxtUserName=mikedm%40gmail.com";
            formData += "&requiredtxtPassword=11csdg33k";
            formData += "&tg=";
            formData += "&vt=";
            formData += "&lvl=";
            formData += "&stype=";
            formData += "&qParam=";
            formData += "&view=";
            formData += "&trn=0";
            formData += "&page=";
            formData += "&catid=";
            formData += "&prodid=";
            formData += "&date=1%2F10%2F2017";
            formData += "&classid=0";
            formData += "&sSU=";
            formData += "&optForwardingLink=";
            formData += "&isAsync=false";

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
                return;
            }

            HtmlAgilityPack.HtmlDocument htmlDocument = GetHtmlDocument(httpWebRequest);
            if (null == htmlDocument) return; // mindbodyonline is down
        }

        private HtmlAgilityPack.HtmlDocument GetHtmlDocument(HttpWebRequest httpWebRequest)
        {
#if true
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

            // Save the cookies from the login request
            foreach (Cookie cookie in httpWebResponse.Cookies)
            {
                mCookieContainer.Add(cookie);
            }

            responseStream.Close();
            httpWebResponse.Close();

#if true
            string filename = "HtmlResponse.html";
            Console.WriteLine("Raw HTML Document written to file " + filename);
            File.WriteAllText(filename, response);
#endif

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(response);
            return htmlDocument;
        }

        private CookieContainer GetCookieContainer()
        {
            if (DateTime.Now < mCookieContainerExpires)
            {
                return mCookieContainer;
            }

            Console.WriteLine("Cookie expired at " + mCookieContainerExpires.ToString() + "; get a new one...");

            // Make a simple request to default page to obtain new cookie
            string requestUri = "http://clients.mindbodyonline.com/ws.asp?studioid=1419";
            mCookieContainer = new CookieContainer();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
            httpWebRequest.CookieContainer = mCookieContainer;

            Console.WriteLine("Get cookie from: " + httpWebRequest.RequestUri.AbsoluteUri);
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
    }
}
