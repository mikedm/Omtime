namespace Omtime
{
    partial class PracticeDetailsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PracticeDetailsForm));
            this.pictureBoxInstructor = new System.Windows.Forms.PictureBox();
            this.labelInstructor = new System.Windows.Forms.Label();
            this.htmlInstructor = new System.Windows.Forms.WebBrowser();
            this.htmlClass = new System.Windows.Forms.WebBrowser();
            this.labelClass = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInstructor)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxInstructor
            // 
            this.pictureBoxInstructor.Location = new System.Drawing.Point(12, 48);
            this.pictureBoxInstructor.Name = "pictureBoxInstructor";
            this.pictureBoxInstructor.Size = new System.Drawing.Size(250, 350);
            this.pictureBoxInstructor.TabIndex = 0;
            this.pictureBoxInstructor.TabStop = false;
            this.pictureBoxInstructor.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxInstructor_LoadCompleted);
            // 
            // labelInstructor
            // 
            this.labelInstructor.AutoSize = true;
            this.labelInstructor.Font = new System.Drawing.Font("Monotype Corsiva", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstructor.Location = new System.Drawing.Point(6, 9);
            this.labelInstructor.Name = "labelInstructor";
            this.labelInstructor.Size = new System.Drawing.Size(75, 33);
            this.labelInstructor.TabIndex = 0;
            this.labelInstructor.Text = "Name";
            this.labelInstructor.UseMnemonic = false;
            // 
            // htmlInstructor
            // 
            this.htmlInstructor.Location = new System.Drawing.Point(274, 48);
            this.htmlInstructor.Name = "htmlInstructor";
            this.htmlInstructor.ScrollBarsEnabled = false;
            this.htmlInstructor.Size = new System.Drawing.Size(250, 350);
            this.htmlInstructor.TabIndex = 2;
            this.htmlInstructor.Visible = false;
            // 
            // htmlClass
            // 
            this.htmlClass.Location = new System.Drawing.Point(519, 48);
            this.htmlClass.Name = "htmlClass";
            this.htmlClass.ScrollBarsEnabled = false;
            this.htmlClass.Size = new System.Drawing.Size(250, 350);
            this.htmlClass.TabIndex = 3;
            this.htmlClass.Visible = false;
            // 
            // labelClass
            // 
            this.labelClass.AutoSize = true;
            this.labelClass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClass.Location = new System.Drawing.Point(119, 23);
            this.labelClass.Name = "labelClass";
            this.labelClass.Size = new System.Drawing.Size(47, 16);
            this.labelClass.TabIndex = 1;
            this.labelClass.Text = "Class";
            this.labelClass.UseMnemonic = false;
            // 
            // PracticeDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 561);
            this.Controls.Add(this.htmlClass);
            this.Controls.Add(this.labelClass);
            this.Controls.Add(this.htmlInstructor);
            this.Controls.Add(this.labelInstructor);
            this.Controls.Add(this.pictureBoxInstructor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PracticeDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yoga Practice";
            this.Load += new System.EventHandler(this.PracticeDetailsForm_Load);
            this.Resize += new System.EventHandler(this.PracticeDetailsForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInstructor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxInstructor;
        private System.Windows.Forms.Label labelInstructor;
        private System.Windows.Forms.WebBrowser htmlInstructor;
        private System.Windows.Forms.WebBrowser htmlClass;
        private System.Windows.Forms.Label labelClass;
    }
}