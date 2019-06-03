namespace MakeUp.HS.Form
{
    partial class MakeUpScoreStatusForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.chkDisplayNotFinish = new System.Windows.Forms.CheckBox();
            this.btnExport = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnRefresh = new DevComponents.DotNetBar.ButtonX();
            this.cboMakeUpBatch = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.picLoading = new System.Windows.Forms.PictureBox();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cbosemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.ColCourseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColRefTeacher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTotalStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // chkDisplayNotFinish
            // 
            this.chkDisplayNotFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDisplayNotFinish.AutoSize = true;
            this.chkDisplayNotFinish.BackColor = System.Drawing.Color.Transparent;
            this.chkDisplayNotFinish.Checked = true;
            this.chkDisplayNotFinish.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDisplayNotFinish.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.chkDisplayNotFinish.Location = new System.Drawing.Point(828, 45);
            this.chkDisplayNotFinish.Name = "chkDisplayNotFinish";
            this.chkDisplayNotFinish.Size = new System.Drawing.Size(170, 21);
            this.chkDisplayNotFinish.TabIndex = 3;
            this.chkDisplayNotFinish.Text = "僅顯示未完成輸入之課程";
            this.chkDisplayNotFinish.UseVisualStyleBackColor = false;
            this.chkDisplayNotFinish.CheckedChanged += new System.EventHandler(this.chkDisplayNotFinish_CheckedChanged);
            // 
            // btnExport
            // 
            this.btnExport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.BackColor = System.Drawing.Color.Transparent;
            this.btnExport.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExport.Location = new System.Drawing.Point(6, 452);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(138, 23);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "匯出到 Excel";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(923, 452);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Excel 2003 (*.xls)|*.xls";
            this.saveFileDialog1.Title = "儲存檔案";
            // 
            // btnRefresh
            // 
            this.btnRefresh.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRefresh.Location = new System.Drawing.Point(826, 452);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 23);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "重新整理";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cboMakeUpBatch
            // 
            this.cboMakeUpBatch.DisplayMember = "Text";
            this.cboMakeUpBatch.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboMakeUpBatch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMakeUpBatch.FormattingEnabled = true;
            this.cboMakeUpBatch.ItemHeight = 19;
            this.cboMakeUpBatch.Location = new System.Drawing.Point(110, 45);
            this.cboMakeUpBatch.Name = "cboMakeUpBatch";
            this.cboMakeUpBatch.Size = new System.Drawing.Size(216, 25);
            this.cboMakeUpBatch.TabIndex = 7;
            this.cboMakeUpBatch.SelectedIndexChanged += new System.EventHandler(this.cboMakeUpBatch_SelectedIndexChanged);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 45);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(110, 23);
            this.labelX1.TabIndex = 8;
            this.labelX1.Text = "請選擇補考梯次";
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToDeleteRows = false;
            this.dataGridViewX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColCourseName,
            this.ColRefTeacher,
            this.ColTotalStatus,
            this.ColDescription});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dataGridViewX1.Location = new System.Drawing.Point(12, 84);
            this.dataGridViewX1.MultiSelect = false;
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.ReadOnly = true;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(980, 362);
            this.dataGridViewX1.TabIndex = 9;
            this.dataGridViewX1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewX1_CellMouseDoubleClick);
            // 
            // picLoading
            // 
            this.picLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picLoading.BackColor = System.Drawing.Color.Transparent;
            this.picLoading.Image = global::MakeUp.HS.Properties.Resources.loading;
            this.picLoading.InitialImage = null;
            this.picLoading.Location = new System.Drawing.Point(473, 227);
            this.picLoading.Name = "picLoading";
            this.picLoading.Size = new System.Drawing.Size(44, 46);
            this.picLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLoading.TabIndex = 10;
            this.picLoading.TabStop = false;
            this.picLoading.Visible = false;
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(110, 14);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(86, 25);
            this.cboSchoolYear.TabIndex = 15;
            this.cboSchoolYear.SelectedIndexChanged += new System.EventHandler(this.cboSchoolYear_SelectedIndexChanged);
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(56, 16);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(48, 23);
            this.labelX2.TabIndex = 16;
            this.labelX2.Text = "學年度";
            // 
            // cbosemester
            // 
            this.cbosemester.DisplayMember = "Text";
            this.cbosemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbosemester.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbosemester.FormattingEnabled = true;
            this.cbosemester.ItemHeight = 19;
            this.cbosemester.Location = new System.Drawing.Point(257, 14);
            this.cbosemester.Name = "cbosemester";
            this.cbosemester.Size = new System.Drawing.Size(69, 25);
            this.cbosemester.TabIndex = 13;
            this.cbosemester.SelectedIndexChanged += new System.EventHandler(this.cbosemester_SelectedIndexChanged);
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(202, 14);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(36, 23);
            this.labelX3.TabIndex = 14;
            this.labelX3.Text = "學期";
            // 
            // ColCourseName
            // 
            this.ColCourseName.HeaderText = "補考群組名稱";
            this.ColCourseName.Name = "ColCourseName";
            this.ColCourseName.ReadOnly = true;
            this.ColCourseName.Width = 200;
            // 
            // ColRefTeacher
            // 
            this.ColRefTeacher.HeaderText = "閱卷老師";
            this.ColRefTeacher.Name = "ColRefTeacher";
            this.ColRefTeacher.ReadOnly = true;
            // 
            // ColTotalStatus
            // 
            this.ColTotalStatus.HeaderText = "填寫完畢項目";
            this.ColTotalStatus.Name = "ColTotalStatus";
            this.ColTotalStatus.ReadOnly = true;
            this.ColTotalStatus.Width = 130;
            // 
            // ColDescription
            // 
            this.ColDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColDescription.HeaderText = "描述";
            this.ColDescription.Name = "ColDescription";
            this.ColDescription.ReadOnly = true;
            // 
            // MakeUpScoreStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 481);
            this.Controls.Add(this.cboSchoolYear);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.cbosemester);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.picLoading);
            this.Controls.Add(this.dataGridViewX1);
            this.Controls.Add(this.cboMakeUpBatch);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.chkDisplayNotFinish);
            this.DoubleBuffered = true;
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(520, 300);
            this.Name = "MakeUpScoreStatusForm";
            this.Text = "補考成績輸入狀況";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkDisplayNotFinish;
        private DevComponents.DotNetBar.ButtonX btnExport;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private DevComponents.DotNetBar.ButtonX btnRefresh;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboMakeUpBatch;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.PictureBox picLoading;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbosemester;
        private DevComponents.DotNetBar.LabelX labelX3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCourseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColRefTeacher;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTotalStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDescription;
    }
}