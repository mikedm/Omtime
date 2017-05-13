using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Omtime
{
    public partial class SettingsForm : Form
    {
        private XmlDocument mXmlDocumentState;

        public SettingsForm(XmlDocument xmlDocument)
        {
            InitializeComponent();
            mXmlDocumentState = xmlDocument;
        }

        private void SettingsForm_Load(object sender, System.EventArgs e)
        {
            XmlNode xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Studios");
            textBoxStudioID.Text = xmlNode.Attributes["selectedId"].Value;

            xmlNode = mXmlDocumentState.SelectSingleNode(String.Format("/Omtime/Studios/Studio[@id='{0}']", textBoxStudioID.Text));
            textBoxCitySpecific.Text = xmlNode == null ? "" : xmlNode.Attributes["citySpecific"].Value;

            // Populate radio list with available studios from XML file
            int yPos = 0;
            XmlNodeList xmlNodeList = mXmlDocumentState.SelectNodes("/Omtime/Studios/Studio");
            foreach (XmlNode xmlNodeStudio in xmlNodeList)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("id", xmlNodeStudio.Attributes["id"].Value);
                dict.Add("citySpecific", xmlNodeStudio.Attributes["citySpecific"].Value);

                RadioButton rb = new RadioButton();
                rb.Text = xmlNodeStudio.InnerText;
                rb.Tag = dict;
                rb.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
                rb.AutoSize = true;
                rb.Location = new Point(0, yPos);
                yPos += 20;
                panelStudios.Controls.Add(rb);
            }
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            XmlNode xmlNode = mXmlDocumentState.SelectSingleNode("/Omtime/Studios");
            xmlNode.Attributes["selectedId"].Value = textBoxStudioID.Text;

            xmlNode = mXmlDocumentState.SelectSingleNode(String.Format("/Omtime/Studios/Studio[@id='{0}']", textBoxStudioID.Text));
            if (xmlNode != null)
            {
                xmlNode.Attributes["citySpecific"].Value = textBoxCitySpecific.Text;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {

        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = (RadioButton)sender;
            Dictionary<string, string> dict = (Dictionary<string, string>)r.Tag;
            textBoxStudioID.Text = dict["id"];
            textBoxCitySpecific.Text = dict["citySpecific"];
        }
    }
}
