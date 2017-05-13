using HtmlAgilityPack;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Omtime
{
    public partial class PracticeDetailsForm : Form
    {
        private Practice mPractice;
        private CookieContainer mCookieContainer;

        private BackgroundWorker mBackgroundWorkerWebRequestInstructor = new BackgroundWorker();
        private HtmlAgilityPack.HtmlDocument mHtmlDocumentInstructor = null;

        private BackgroundWorker mBackgroundWorkerWebRequestClass = new BackgroundWorker();
        private HtmlAgilityPack.HtmlDocument mHtmlDocumentClass = null;

        private const string HTML_HEADER = "<html><body style='background-color:rgb(240,240,240);margin:0;font-family:Sans-serif'>";
        private const string HTML_FOOTER = "</body></html>";
        private const int WEB_REQUEST_TIMEOUT = 1000 * 2;

        public PracticeDetailsForm(Practice p, CookieContainer c)
        {
            InitializeComponent();
            mPractice = p;
            mCookieContainer = c;
        }

        private void PracticeDetailsForm_Load(object sender, EventArgs e)
        {
            mBackgroundWorkerWebRequestInstructor.WorkerReportsProgress = false;
            mBackgroundWorkerWebRequestInstructor.WorkerSupportsCancellation = false;
            mBackgroundWorkerWebRequestInstructor.DoWork += new DoWorkEventHandler(DoWebRequestInBackgroudInstructor);
            mBackgroundWorkerWebRequestInstructor.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DoWebRequestInBackgroudInstructorCompleted);
            mBackgroundWorkerWebRequestInstructor.RunWorkerAsync();

            mBackgroundWorkerWebRequestClass.WorkerReportsProgress = false;
            mBackgroundWorkerWebRequestClass.WorkerSupportsCancellation = false;
            mBackgroundWorkerWebRequestClass.DoWork += new DoWorkEventHandler(DoWebRequestInBackgroudClass);
            mBackgroundWorkerWebRequestClass.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DoWebRequestInBackgroudClassCompleted);
            mBackgroundWorkerWebRequestClass.RunWorkerAsync();

            labelClass.Text = mPractice.className;
            labelInstructor.Text = mPractice.instructor;
            this.Text = mPractice.instructor + " - " + mPractice.className;

            SetSize();
        }

        private void DoWebRequestInBackgroudInstructor(object sender, DoWorkEventArgs e)
        {
            if (mPractice.mindBodyOnlineId != null)
            {
                mHtmlDocumentInstructor = GetHtmlDocumentInstructor(mPractice.mindBodyOnlineId);
            }
        }

        private void DoWebRequestInBackgroudInstructorCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("DoWebRequestInBackgroudInstructorCompleted was canceled");
                return;
            }

            if (e.Error != null)
            {
                Console.WriteLine("DoWebRequestInBackgroudInstructorCompleted had error " + e.Error.Message);
                return;
            }

            Console.WriteLine("DoWebRequestInBackgroudInstructorCompleted");
            StringBuilder sb = new StringBuilder();
            sb.Append(HTML_HEADER);

            if (mHtmlDocumentInstructor == null)
            {
                sb.Append(HTML_FOOTER);
                try
                {
                    htmlInstructor.DocumentText = sb.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in htmlInstructor WebBrowser control: " + ex.ToString());
                }

                SetSize();
                return;
            }

            HtmlNode htmlNode = mHtmlDocumentInstructor.DocumentNode.SelectSingleNode("//div[@class='userHTML']");
            if (htmlNode != null)
            {
                sb.Append(htmlNode.InnerHtml);
                sb.Append(HTML_FOOTER);
                try
                {
                    htmlInstructor.DocumentText = sb.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in htmlInstructor WebBrowser control: " + ex.ToString());
                }
            }

            htmlNode = mHtmlDocumentInstructor.DocumentNode.SelectSingleNode("/div/img");
            if (htmlNode != null)
            {
                HtmlAttribute htmlAttribute = htmlNode.Attributes["src"];
                pictureBoxInstructor.LoadAsync(htmlAttribute.Value);
            }

            htmlInstructor.Visible = true;
            pictureBoxInstructor.Visible = true;
            SetSize();
        }

        private void DoWebRequestInBackgroudClass(object sender, DoWorkEventArgs e)
        {
            mHtmlDocumentClass = GetHtmlDocumentClass(mPractice.classId);
        }

        private void DoWebRequestInBackgroudClassCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("DoWebRequestInBackgroudClassCompleted was cancelled");
                return;
            }

            if (e.Error != null)
            {
                Console.WriteLine("DoWebRequestInBackgroudClassCompleted had error " + e.Error.Message);
                return;
            }

            Console.WriteLine("DoWebRequestInBackgroudClassCompleted");
            StringBuilder sb = new StringBuilder();
            sb.Append(HTML_HEADER);

            if (mHtmlDocumentClass == null)
            {
                sb.Append("No information available; please close this form and try again...");
            }
            else
            {
                HtmlNode htmlNode = mHtmlDocumentClass.DocumentNode.SelectSingleNode("//div[@class='userHTML']");
                if (htmlNode == null) return;
                sb.Append(htmlNode.InnerHtml);
            }

            sb.Append(HTML_FOOTER);
            try
            {
                htmlClass.DocumentText = sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in htmlClass WebBrowser control: " + ex.ToString());
            }

            htmlClass.Visible = true;
            SetSize();
        }

        private void pictureBoxInstructor_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            SetSize();
        }

        private void PracticeDetailsForm_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        private void SetSize()
        {
            labelClass.Left = labelInstructor.Right + 12;

            if (mHtmlDocumentInstructor == null)
            {
                htmlClass.Height = this.ClientSize.Height - htmlClass.Top - 12;
                htmlClass.Width = this.ClientSize.Width - 24;
                htmlClass.Left = 12;
                htmlClass.Top = labelClass.Bottom + 24;
            }
            else
            {
                htmlInstructor.Height = this.ClientSize.Height - htmlInstructor.Top - 12;
                htmlClass.Height = htmlInstructor.Height;
                htmlClass.Left = htmlInstructor.Right + 12;
                htmlClass.Top = 48;

                if (pictureBoxInstructor.Image != null)
                {
                    htmlInstructor.Width = (this.ClientSize.Width - pictureBoxInstructor.Image.Width - 48) / 2;
                    htmlClass.Width = htmlInstructor.Width;

                    htmlInstructor.Left = pictureBoxInstructor.Left + pictureBoxInstructor.Image.Width + 12;
                    htmlClass.Left = htmlInstructor.Right + 12;
                }
                else
                {
                    htmlInstructor.Width = (this.ClientSize.Width - 36) / 2;
                    htmlClass.Width = htmlInstructor.Width;

                    htmlInstructor.Left = 12;
                    htmlClass.Left = htmlInstructor.Right + 12;
                }
            }

            // So you don't see a cursor (looks weird w a cursor blinking in this instructor's bio)
            labelInstructor.Focus();
        }

        private HtmlAgilityPack.HtmlDocument GetHtmlDocumentInstructor(string mindBodyOnlineId)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://clients.mindbodyonline.com/Ajax/QuickStaffBio/?trnid=" + mindBodyOnlineId);
            httpWebRequest.Method = "GET";
            httpWebRequest.CookieContainer = mCookieContainer;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
            httpWebRequest.Timeout = WEB_REQUEST_TIMEOUT;

            HttpWebResponse httpWebResponse = null;
            try
            {
                Console.WriteLine("Get the practice details...");
                Console.WriteLine(httpWebRequest.RequestUri.AbsoluteUri);
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in httpWebRequest.GetResponse: " + ex.ToString());
                return null;
            }

            Console.WriteLine("Response attributes:");
            Console.WriteLine("   httpWebResponse.ContentType: {0}", httpWebResponse.ContentType);
            Console.WriteLine("   httpWebResponse.ContentLength: {0}", httpWebResponse.ContentLength);
            Console.WriteLine("   httpWebResponse.StatusCode: {0}", httpWebResponse.StatusCode);
            Console.WriteLine("   httpWebResponse.StatusDescription: {0}", httpWebResponse.StatusDescription);

            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string response = reader.ReadToEnd();

            responseStream.Close();
            httpWebResponse.Close();

            //Console.WriteLine("Whole response:");
            //Console.WriteLine(response);

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(response);
            return htmlDocument;
        }

        private HtmlAgilityPack.HtmlDocument GetHtmlDocumentClass(string classId)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://clients.mindbodyonline.com/Ajax/ClassInfo/?classid=" + classId);
            httpWebRequest.Method = "GET";
            httpWebRequest.CookieContainer = mCookieContainer;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";
            httpWebRequest.Timeout = WEB_REQUEST_TIMEOUT;

            HttpWebResponse httpWebResponse = null;
            try
            {
                Console.WriteLine("Get the class details...");
                Console.WriteLine(httpWebRequest.RequestUri.AbsoluteUri);
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in httpWebRequest.GetResponse: " + ex.ToString());
                return null;
            }

            Console.WriteLine("Response attributes:");
            Console.WriteLine("   httpWebResponse.ContentType: {0}", httpWebResponse.ContentType);
            Console.WriteLine("   httpWebResponse.ContentLength: {0}", httpWebResponse.ContentLength);
            Console.WriteLine("   httpWebResponse.StatusCode: {0}", httpWebResponse.StatusCode);
            Console.WriteLine("   httpWebResponse.StatusDescription: {0}", httpWebResponse.StatusDescription);

            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string response = reader.ReadToEnd();

            responseStream.Close();
            httpWebResponse.Close();

            //Console.WriteLine("Whole response:");
            //Console.WriteLine(response);

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(response);
            return htmlDocument;
        }
    }
}
