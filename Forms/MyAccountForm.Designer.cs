namespace Omtime
{
    partial class MyAccountForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyAccountForm));
            this.labelClassesTitle = new System.Windows.Forms.Label();
            this.labelLocationsTitle = new System.Windows.Forms.Label();
            this.labelGurusTitle = new System.Windows.Forms.Label();
            this.labelClasses = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelLocations = new System.Windows.Forms.Label();
            this.labelGurus = new System.Windows.Forms.Label();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // labelClassesTitle
            // 
            this.labelClassesTitle.AutoSize = true;
            this.labelClassesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClassesTitle.Location = new System.Drawing.Point(10, 10);
            this.labelClassesTitle.Name = "labelClassesTitle";
            this.labelClassesTitle.Size = new System.Drawing.Size(106, 29);
            this.labelClassesTitle.TabIndex = 0;
            this.labelClassesTitle.Text = "Classes";
            // 
            // labelLocationsTitle
            // 
            this.labelLocationsTitle.AutoSize = true;
            this.labelLocationsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLocationsTitle.Location = new System.Drawing.Point(148, 10);
            this.labelLocationsTitle.Name = "labelLocationsTitle";
            this.labelLocationsTitle.Size = new System.Drawing.Size(125, 29);
            this.labelLocationsTitle.TabIndex = 1;
            this.labelLocationsTitle.Text = "Locations";
            // 
            // labelGurusTitle
            // 
            this.labelGurusTitle.AutoSize = true;
            this.labelGurusTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGurusTitle.Location = new System.Drawing.Point(305, 10);
            this.labelGurusTitle.Name = "labelGurusTitle";
            this.labelGurusTitle.Size = new System.Drawing.Size(82, 29);
            this.labelGurusTitle.TabIndex = 2;
            this.labelGurusTitle.Text = "Gurus";
            // 
            // labelClasses
            // 
            this.labelClasses.AutoSize = true;
            this.labelClasses.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClasses.ForeColor = System.Drawing.Color.Yellow;
            this.labelClasses.Location = new System.Drawing.Point(20, 37);
            this.labelClasses.Name = "labelClasses";
            this.labelClasses.Size = new System.Drawing.Size(87, 36);
            this.labelClasses.TabIndex = 3;
            this.labelClasses.Text = "3333";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panel1.Controls.Add(this.labelGurus);
            this.panel1.Controls.Add(this.labelLocations);
            this.panel1.Controls.Add(this.labelClasses);
            this.panel1.Controls.Add(this.labelGurusTitle);
            this.panel1.Controls.Add(this.labelClassesTitle);
            this.panel1.Controls.Add(this.labelLocationsTitle);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(810, 92);
            this.panel1.TabIndex = 0;
            // 
            // labelLocations
            // 
            this.labelLocations.AutoSize = true;
            this.labelLocations.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLocations.ForeColor = System.Drawing.Color.Yellow;
            this.labelLocations.Location = new System.Drawing.Point(167, 37);
            this.labelLocations.Name = "labelLocations";
            this.labelLocations.Size = new System.Drawing.Size(87, 36);
            this.labelLocations.TabIndex = 4;
            this.labelLocations.Text = "3333";
            // 
            // labelGurus
            // 
            this.labelGurus.AutoSize = true;
            this.labelGurus.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGurus.ForeColor = System.Drawing.Color.Yellow;
            this.labelGurus.Location = new System.Drawing.Point(303, 37);
            this.labelGurus.Name = "labelGurus";
            this.labelGurus.Size = new System.Drawing.Size(87, 36);
            this.labelGurus.TabIndex = 5;
            this.labelGurus.Text = "3333";
            // 
            // dgvHistory
            // 
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Location = new System.Drawing.Point(0, 88);
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.ShowEditingIcon = false;
            this.dgvHistory.Size = new System.Drawing.Size(810, 480);
            this.dgvHistory.TabIndex = 1;
            // 
            // MyAccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 568);
            this.Controls.Add(this.dgvHistory);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyAccountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "My Account";
            this.Load += new System.EventHandler(this.MyAccountForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelClassesTitle;
        private System.Windows.Forms.Label labelLocationsTitle;
        private System.Windows.Forms.Label labelGurusTitle;
        private System.Windows.Forms.Label labelClasses;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelGurus;
        private System.Windows.Forms.Label labelLocations;
        private System.Windows.Forms.DataGridView dgvHistory;
    }
}