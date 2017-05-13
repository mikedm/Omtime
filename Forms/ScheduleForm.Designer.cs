namespace Omtime
{
    partial class ScheduleForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduleForm));
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.labelSchedule = new System.Windows.Forms.Label();
            this.numericUpDownScheduleDays = new System.Windows.Forms.NumericUpDown();
            this.labelNumDays = new System.Windows.Forms.Label();
            this.panelRefresh = new System.Windows.Forms.Panel();
            this.MyAccountBtn = new System.Windows.Forms.Button();
            this.SettingsBtn = new System.Windows.Forms.Button();
            this.checkBoxEvening = new System.Windows.Forms.CheckBox();
            this.checkBoxAfternoon = new System.Windows.Forms.CheckBox();
            this.checkBoxMorning = new System.Windows.Forms.CheckBox();
            this.toolTipFavoriteInstructors = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipFavoriteStudios = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipFavoriteClasses = new System.Windows.Forms.ToolTip(this.components);
            this.buttonClasses = new System.Windows.Forms.Button();
            this.buttonInstructors = new System.Windows.Forms.Button();
            this.buttonStudios = new System.Windows.Forms.Button();
            this.pictureBoxStudio = new System.Windows.Forms.PictureBox();
            this.workerWebReq = new System.ComponentModel.BackgroundWorker();
            this.workerStudioPic = new System.ComponentModel.BackgroundWorker();
            this.toolTipRefreshButton = new System.Windows.Forms.ToolTip(this.components);
            this.panelQuickInstructor = new Omtime.CheckBoxFlowLayoutPanel();
            this.panelQuickStudio = new Omtime.CheckBoxFlowLayoutPanel();
            this.panelQuickClass = new Omtime.CheckBoxFlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScheduleDays)).BeginInit();
            this.panelRefresh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStudio)).BeginInit();
            this.SuspendLayout();
            // 
            // RefreshBtn
            // 
            this.RefreshBtn.Location = new System.Drawing.Point(464, 1);
            this.RefreshBtn.Name = "RefreshBtn";
            this.RefreshBtn.Size = new System.Drawing.Size(69, 23);
            this.RefreshBtn.TabIndex = 5;
            this.RefreshBtn.Text = "Refresh";
            this.RefreshBtn.UseVisualStyleBackColor = true;
            this.RefreshBtn.Click += new System.EventHandler(this.RefreshBtn_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.Location = new System.Drawing.Point(43, 150);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(763, 210);
            this.dataGridView.TabIndex = 8;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            this.dataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentDoubleClick);
            this.dataGridView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_CellMouseDown);
            this.dataGridView.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridView_ColumnWidthChanged);
            // 
            // labelSchedule
            // 
            this.labelSchedule.AutoSize = true;
            this.labelSchedule.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSchedule.Location = new System.Drawing.Point(128, 121);
            this.labelSchedule.Name = "labelSchedule";
            this.labelSchedule.Size = new System.Drawing.Size(116, 20);
            this.labelSchedule.TabIndex = 6;
            this.labelSchedule.Text = "Schedule (0):";
            this.labelSchedule.Click += new System.EventHandler(this.labelSchedule_Click);
            // 
            // numericUpDownScheduleDays
            // 
            this.numericUpDownScheduleDays.Location = new System.Drawing.Point(412, 2);
            this.numericUpDownScheduleDays.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDownScheduleDays.Minimum = new decimal(new int[] {
            120,
            0,
            0,
            -2147483648});
            this.numericUpDownScheduleDays.Name = "numericUpDownScheduleDays";
            this.numericUpDownScheduleDays.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownScheduleDays.TabIndex = 4;
            this.numericUpDownScheduleDays.ValueChanged += new System.EventHandler(this.numericUpDownScheduleDays_ValueChanged);
            // 
            // labelNumDays
            // 
            this.labelNumDays.AutoSize = true;
            this.labelNumDays.Location = new System.Drawing.Point(325, 5);
            this.labelNumDays.Name = "labelNumDays";
            this.labelNumDays.Size = new System.Drawing.Size(84, 13);
            this.labelNumDays.TabIndex = 3;
            this.labelNumDays.Text = "Number of days:";
            // 
            // panelRefresh
            // 
            this.panelRefresh.Controls.Add(this.MyAccountBtn);
            this.panelRefresh.Controls.Add(this.SettingsBtn);
            this.panelRefresh.Controls.Add(this.checkBoxEvening);
            this.panelRefresh.Controls.Add(this.checkBoxAfternoon);
            this.panelRefresh.Controls.Add(this.RefreshBtn);
            this.panelRefresh.Controls.Add(this.checkBoxMorning);
            this.panelRefresh.Controls.Add(this.numericUpDownScheduleDays);
            this.panelRefresh.Controls.Add(this.labelNumDays);
            this.panelRefresh.Location = new System.Drawing.Point(274, 119);
            this.panelRefresh.Name = "panelRefresh";
            this.panelRefresh.Size = new System.Drawing.Size(532, 25);
            this.panelRefresh.TabIndex = 7;
            // 
            // MyAccountBtn
            // 
            this.MyAccountBtn.FlatAppearance.BorderSize = 0;
            this.MyAccountBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MyAccountBtn.Location = new System.Drawing.Point(1, 1);
            this.MyAccountBtn.Name = "MyAccountBtn";
            this.MyAccountBtn.Size = new System.Drawing.Size(55, 23);
            this.MyAccountBtn.TabIndex = 7;
            this.MyAccountBtn.Text = "Account";
            this.MyAccountBtn.UseVisualStyleBackColor = true;
            this.MyAccountBtn.Click += new System.EventHandler(this.MyAccountBtn_Click);
            // 
            // SettingsBtn
            // 
            this.SettingsBtn.FlatAppearance.BorderSize = 0;
            this.SettingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsBtn.Location = new System.Drawing.Point(56, 1);
            this.SettingsBtn.Name = "SettingsBtn";
            this.SettingsBtn.Size = new System.Drawing.Size(53, 23);
            this.SettingsBtn.TabIndex = 6;
            this.SettingsBtn.Text = "Settings";
            this.SettingsBtn.UseVisualStyleBackColor = true;
            this.SettingsBtn.Click += new System.EventHandler(this.SettingsBtn_Click);
            // 
            // checkBoxEvening
            // 
            this.checkBoxEvening.AutoSize = true;
            this.checkBoxEvening.Checked = true;
            this.checkBoxEvening.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEvening.Location = new System.Drawing.Point(257, 3);
            this.checkBoxEvening.Name = "checkBoxEvening";
            this.checkBoxEvening.Size = new System.Drawing.Size(65, 17);
            this.checkBoxEvening.TabIndex = 2;
            this.checkBoxEvening.Text = "Evening";
            this.checkBoxEvening.UseVisualStyleBackColor = true;
            this.checkBoxEvening.CheckedChanged += new System.EventHandler(this.checkBoxEvening_CheckedChanged);
            // 
            // checkBoxAfternoon
            // 
            this.checkBoxAfternoon.AutoSize = true;
            this.checkBoxAfternoon.Checked = true;
            this.checkBoxAfternoon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAfternoon.Location = new System.Drawing.Point(182, 4);
            this.checkBoxAfternoon.Name = "checkBoxAfternoon";
            this.checkBoxAfternoon.Size = new System.Drawing.Size(72, 17);
            this.checkBoxAfternoon.TabIndex = 1;
            this.checkBoxAfternoon.Text = "Afternoon";
            this.checkBoxAfternoon.UseVisualStyleBackColor = true;
            this.checkBoxAfternoon.CheckedChanged += new System.EventHandler(this.checkBoxAfternoon_CheckedChanged);
            // 
            // checkBoxMorning
            // 
            this.checkBoxMorning.AutoSize = true;
            this.checkBoxMorning.Checked = true;
            this.checkBoxMorning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMorning.Location = new System.Drawing.Point(115, 4);
            this.checkBoxMorning.Name = "checkBoxMorning";
            this.checkBoxMorning.Size = new System.Drawing.Size(64, 17);
            this.checkBoxMorning.TabIndex = 0;
            this.checkBoxMorning.Text = "Morning";
            this.checkBoxMorning.UseVisualStyleBackColor = true;
            this.checkBoxMorning.CheckedChanged += new System.EventHandler(this.checkBoxMorning_CheckedChanged);
            // 
            // toolTipFavoriteInstructors
            // 
            this.toolTipFavoriteInstructors.AutomaticDelay = 250;
            this.toolTipFavoriteInstructors.AutoPopDelay = 30000;
            this.toolTipFavoriteInstructors.InitialDelay = 250;
            this.toolTipFavoriteInstructors.ReshowDelay = 50;
            // 
            // toolTipFavoriteStudios
            // 
            this.toolTipFavoriteStudios.AutomaticDelay = 250;
            this.toolTipFavoriteStudios.AutoPopDelay = 30000;
            this.toolTipFavoriteStudios.InitialDelay = 250;
            this.toolTipFavoriteStudios.ReshowDelay = 50;
            // 
            // toolTipFavoriteClasses
            // 
            this.toolTipFavoriteClasses.AutomaticDelay = 250;
            this.toolTipFavoriteClasses.AutoPopDelay = 30000;
            this.toolTipFavoriteClasses.InitialDelay = 250;
            this.toolTipFavoriteClasses.ReshowDelay = 50;
            // 
            // buttonClasses
            // 
            this.buttonClasses.BackColor = System.Drawing.Color.Transparent;
            this.buttonClasses.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonClasses.FlatAppearance.BorderSize = 0;
            this.buttonClasses.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonClasses.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonClasses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClasses.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClasses.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
            this.buttonClasses.Location = new System.Drawing.Point(95, 116);
            this.buttonClasses.Margin = new System.Windows.Forms.Padding(0);
            this.buttonClasses.Name = "buttonClasses";
            this.buttonClasses.Size = new System.Drawing.Size(26, 26);
            this.buttonClasses.TabIndex = 5;
            this.buttonClasses.UseVisualStyleBackColor = false;
            this.buttonClasses.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonClasses_MouseUp);
            // 
            // buttonInstructors
            // 
            this.buttonInstructors.BackColor = System.Drawing.Color.Transparent;
            this.buttonInstructors.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonInstructors.FlatAppearance.BorderSize = 0;
            this.buttonInstructors.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonInstructors.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonInstructors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonInstructors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInstructors.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
            this.buttonInstructors.Location = new System.Drawing.Point(43, 116);
            this.buttonInstructors.Margin = new System.Windows.Forms.Padding(0);
            this.buttonInstructors.Name = "buttonInstructors";
            this.buttonInstructors.Size = new System.Drawing.Size(26, 26);
            this.buttonInstructors.TabIndex = 3;
            this.buttonInstructors.UseVisualStyleBackColor = false;
            this.buttonInstructors.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonInstructors_MouseUp);
            // 
            // buttonStudios
            // 
            this.buttonStudios.BackColor = System.Drawing.Color.Transparent;
            this.buttonStudios.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonStudios.FlatAppearance.BorderSize = 0;
            this.buttonStudios.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonStudios.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonStudios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStudios.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStudios.Image = global::Omtime.Properties.Resources.OrangeSun_26x26;
            this.buttonStudios.Location = new System.Drawing.Point(69, 116);
            this.buttonStudios.Margin = new System.Windows.Forms.Padding(0);
            this.buttonStudios.Name = "buttonStudios";
            this.buttonStudios.Size = new System.Drawing.Size(26, 26);
            this.buttonStudios.TabIndex = 4;
            this.buttonStudios.UseVisualStyleBackColor = false;
            this.buttonStudios.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonStudios_MouseUp);
            // 
            // pictureBoxStudio
            // 
            this.pictureBoxStudio.Location = new System.Drawing.Point(43, 366);
            this.pictureBoxStudio.Name = "pictureBoxStudio";
            this.pictureBoxStudio.Size = new System.Drawing.Size(763, 90);
            this.pictureBoxStudio.TabIndex = 9;
            this.pictureBoxStudio.TabStop = false;
            this.pictureBoxStudio.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxStudio_LoadCompleted);
            // 
            // workerWebReq
            // 
            this.workerWebReq.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoWebRequestInBackgroud);
            this.workerWebReq.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DoWebRequestInBackgroudCompleted);
            // 
            // workerStudioPic
            // 
            this.workerStudioPic.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoWebRequestInBackgroudStudioPic);
            this.workerStudioPic.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DoWebRequestInBackgroudCompletedStudioPic);
            // 
            // panelQuickInstructor
            // 
            this.panelQuickInstructor.AutoScroll = true;
            this.panelQuickInstructor.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelQuickInstructor.Location = new System.Drawing.Point(2, 2);
            this.panelQuickInstructor.Name = "panelQuickInstructor";
            this.panelQuickInstructor.Size = new System.Drawing.Size(35, 454);
            this.panelQuickInstructor.TabIndex = 0;
            this.panelQuickInstructor.WrapContents = false;
            this.panelQuickInstructor.Click += new System.EventHandler(this.panelQuickInstructor_Click);
            this.panelQuickInstructor.MouseEnter += new System.EventHandler(this.panelQuickInstructor_MouseEnter);
            this.panelQuickInstructor.MouseLeave += new System.EventHandler(this.panelQuickInstructor_MouseLeave);
            // 
            // panelQuickStudio
            // 
            this.panelQuickStudio.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelQuickStudio.Location = new System.Drawing.Point(43, 2);
            this.panelQuickStudio.Name = "panelQuickStudio";
            this.panelQuickStudio.Size = new System.Drawing.Size(763, 53);
            this.panelQuickStudio.TabIndex = 1;
            // 
            // panelQuickClass
            // 
            this.panelQuickClass.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelQuickClass.Location = new System.Drawing.Point(43, 61);
            this.panelQuickClass.Name = "panelQuickClass";
            this.panelQuickClass.Size = new System.Drawing.Size(763, 52);
            this.panelQuickClass.TabIndex = 2;
            // 
            // ScheduleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 460);
            this.Controls.Add(this.panelQuickInstructor);
            this.Controls.Add(this.panelQuickStudio);
            this.Controls.Add(this.panelQuickClass);
            this.Controls.Add(this.buttonClasses);
            this.Controls.Add(this.buttonInstructors);
            this.Controls.Add(this.buttonStudios);
            this.Controls.Add(this.pictureBoxStudio);
            this.Controls.Add(this.panelRefresh);
            this.Controls.Add(this.labelSchedule);
            this.Controls.Add(this.dataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ScheduleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ideas for the YG";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScheduleForm_FormClosing);
            this.Load += new System.EventHandler(this.ScheduleForm_Load);
            this.Shown += new System.EventHandler(this.ScheduleForm_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ScheduleForm_KeyUp);
            this.Resize += new System.EventHandler(this.ScheduleForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScheduleDays)).EndInit();
            this.panelRefresh.ResumeLayout(false);
            this.panelRefresh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStudio)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RefreshBtn;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label labelSchedule;
        private System.Windows.Forms.NumericUpDown numericUpDownScheduleDays;
        private System.Windows.Forms.Label labelNumDays;
        private System.Windows.Forms.Panel panelRefresh;
        private System.Windows.Forms.CheckBox checkBoxEvening;
        private System.Windows.Forms.CheckBox checkBoxAfternoon;
        private System.Windows.Forms.CheckBox checkBoxMorning;
        private System.Windows.Forms.ToolTip toolTipFavoriteInstructors;
        private System.Windows.Forms.ToolTip toolTipFavoriteStudios;
        private System.Windows.Forms.ToolTip toolTipFavoriteClasses;
        private System.Windows.Forms.PictureBox pictureBoxStudio;
        private System.Windows.Forms.Button buttonStudios;
        private System.Windows.Forms.Button buttonClasses;
        private System.Windows.Forms.Button buttonInstructors;
        private System.ComponentModel.BackgroundWorker workerStudioPic;
        private System.ComponentModel.BackgroundWorker workerWebReq;
        private Omtime.CheckBoxFlowLayoutPanel panelQuickInstructor;
        private Omtime.CheckBoxFlowLayoutPanel panelQuickStudio;
        private Omtime.CheckBoxFlowLayoutPanel panelQuickClass;
        private System.Windows.Forms.Button SettingsBtn;
        private System.Windows.Forms.ToolTip toolTipRefreshButton;
        private System.Windows.Forms.Button MyAccountBtn;
    }
}

