using System;
using System.Windows.Forms;

namespace Omtime
{
    public class CheckBoxFlowLayoutPanel : FlowLayoutPanel
    {
        public void CheckAll()
        {
            foreach (CheckBox cb in this.Controls)
            {
                cb.Checked = true;
            }
        }

        public void CheckNone()
        {
            foreach (CheckBox cb in this.Controls)
            {
                cb.Checked = false;
            }
        }

        public CheckBox SafeGet(String s)
        {
            CheckBox cb = null;
            if (this.Controls.ContainsKey(s))
            {
                cb = ((CheckBox)this.Controls[s]);
            }
            return cb;
        }
    }
}
