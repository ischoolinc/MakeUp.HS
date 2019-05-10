namespace MakeUp.HS.Form
{
    partial class InsertUpdateMakeUpGroupForm
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
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtGroupName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtDescription = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.picLoading = new System.Windows.Forms.PictureBox();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.ColStudentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSeat_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStudentNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSubject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCredit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRequired_By = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColIs_Required = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMakeUp_Score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPass_Standard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMakeUp_Standard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.cboTeacher = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.btnSwap = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(907, 659);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(102, 23);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "離開";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(797, 659);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(18, 23);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(61, 23);
            this.labelX1.TabIndex = 14;
            this.labelX1.Text = "補考群組:";
            // 
            // txtGroupName
            // 
            this.txtGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtGroupName.Border.Class = "TextBoxBorder";
            this.txtGroupName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtGroupName.Location = new System.Drawing.Point(85, 23);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new System.Drawing.Size(518, 25);
            this.txtGroupName.TabIndex = 15;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtDescription.Border.Class = "TextBoxBorder";
            this.txtDescription.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDescription.Location = new System.Drawing.Point(85, 54);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(922, 83);
            this.txtDescription.TabIndex = 17;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(18, 52);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(80, 23);
            this.labelX2.TabIndex = 16;
            this.labelX2.Text = "群組說明:";
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(18, 147);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(61, 23);
            this.labelX3.TabIndex = 18;
            this.labelX3.Text = "補考資料:";
            // 
            // picLoading
            // 
            this.picLoading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.picLoading.BackColor = System.Drawing.Color.Transparent;
            this.picLoading.Image = global::MakeUp.HS.Properties.Resources.loading;
            this.picLoading.InitialImage = null;
            this.picLoading.Location = new System.Drawing.Point(460, 354);
            this.picLoading.Name = "picLoading";
            this.picLoading.Size = new System.Drawing.Size(71, 75);
            this.picLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLoading.TabIndex = 22;
            this.picLoading.TabStop = false;
            this.picLoading.Visible = false;
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToDeleteRows = false;
            this.dataGridViewX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewX1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColStudentName,
            this.ColClassName,
            this.ColSeat_no,
            this.ColStudentNumber,
            this.ColSubject,
            this.ColLevel,
            this.ColCredit,
            this.ColumnRequired_By,
            this.ColIs_Required,
            this.ColScore,
            this.ColMakeUp_Score,
            this.ColPass_Standard,
            this.ColMakeUp_Standard});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(18, 176);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(988, 477);
            this.dataGridViewX1.TabIndex = 23;
            // 
            // ColStudentName
            // 
            this.ColStudentName.FillWeight = 190.4105F;
            this.ColStudentName.HeaderText = "學生姓名";
            this.ColStudentName.Name = "ColStudentName";
            // 
            // ColClassName
            // 
            this.ColClassName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColClassName.FillWeight = 70.76244F;
            this.ColClassName.HeaderText = "班級";
            this.ColClassName.Name = "ColClassName";
            this.ColClassName.ReadOnly = true;
            // 
            // ColSeat_no
            // 
            this.ColSeat_no.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColSeat_no.FillWeight = 47.71125F;
            this.ColSeat_no.HeaderText = "座號";
            this.ColSeat_no.Name = "ColSeat_no";
            this.ColSeat_no.ReadOnly = true;
            // 
            // ColStudentNumber
            // 
            this.ColStudentNumber.FillWeight = 138.2111F;
            this.ColStudentNumber.HeaderText = "學號";
            this.ColStudentNumber.Name = "ColStudentNumber";
            this.ColStudentNumber.ReadOnly = true;
            // 
            // ColSubject
            // 
            this.ColSubject.FillWeight = 92.14074F;
            this.ColSubject.HeaderText = "科目";
            this.ColSubject.Name = "ColSubject";
            // 
            // ColLevel
            // 
            this.ColLevel.FillWeight = 92.14074F;
            this.ColLevel.HeaderText = "級別";
            this.ColLevel.Name = "ColLevel";
            // 
            // ColCredit
            // 
            this.ColCredit.FillWeight = 92.14074F;
            this.ColCredit.HeaderText = "學分";
            this.ColCredit.Name = "ColCredit";
            // 
            // ColumnRequired_By
            // 
            this.ColumnRequired_By.FillWeight = 92.14074F;
            this.ColumnRequired_By.HeaderText = "校部定";
            this.ColumnRequired_By.Name = "ColumnRequired_By";
            // 
            // ColIs_Required
            // 
            this.ColIs_Required.FillWeight = 92.14074F;
            this.ColIs_Required.HeaderText = "必選修";
            this.ColIs_Required.Name = "ColIs_Required";
            // 
            // ColScore
            // 
            this.ColScore.FillWeight = 92.14074F;
            this.ColScore.HeaderText = "成績分數";
            this.ColScore.Name = "ColScore";
            // 
            // ColMakeUp_Score
            // 
            this.ColMakeUp_Score.FillWeight = 92.14074F;
            this.ColMakeUp_Score.HeaderText = "補考分數";
            this.ColMakeUp_Score.Name = "ColMakeUp_Score";
            // 
            // ColPass_Standard
            // 
            this.ColPass_Standard.FillWeight = 92.14074F;
            this.ColPass_Standard.HeaderText = "及格標準";
            this.ColPass_Standard.Name = "ColPass_Standard";
            // 
            // ColMakeUp_Standard
            // 
            this.ColMakeUp_Standard.FillWeight = 92.14074F;
            this.ColMakeUp_Standard.HeaderText = "補考標準";
            this.ColMakeUp_Standard.Name = "ColMakeUp_Standard";
            // 
            // labelX4
            // 
            this.labelX4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(609, 23);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(61, 23);
            this.labelX4.TabIndex = 24;
            this.labelX4.Text = "閱卷老師:";
            // 
            // cboTeacher
            // 
            this.cboTeacher.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTeacher.DisplayMember = "Text";
            this.cboTeacher.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboTeacher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTeacher.FormattingEnabled = true;
            this.cboTeacher.ItemHeight = 19;
            this.cboTeacher.Location = new System.Drawing.Point(676, 23);
            this.cboTeacher.Name = "cboTeacher";
            this.cboTeacher.Size = new System.Drawing.Size(141, 25);
            this.cboTeacher.TabIndex = 25;
            // 
            // btnSwap
            // 
            this.btnSwap.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSwap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSwap.BackColor = System.Drawing.Color.Transparent;
            this.btnSwap.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSwap.Location = new System.Drawing.Point(18, 659);
            this.btnSwap.Name = "btnSwap";
            this.btnSwap.Size = new System.Drawing.Size(160, 23);
            this.btnSwap.TabIndex = 26;
            this.btnSwap.Text = "補考資料移至其他群組";
            this.btnSwap.Click += new System.EventHandler(this.btnSwap_Click);
            // 
            // InsertUpdateMakeUpGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 694);
            this.Controls.Add(this.btnSwap);
            this.Controls.Add(this.cboTeacher);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.picLoading);
            this.Controls.Add(this.dataGridViewX1);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.txtGroupName);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.DoubleBuffered = true;
            this.Name = "InsertUpdateMakeUpGroupForm";
            this.Text = "新增補考群組";
            this.Load += new System.EventHandler(this.InsertUpdateMakeUpBatchForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtGroupName;
        private DevComponents.DotNetBar.Controls.TextBoxX txtDescription;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private System.Windows.Forms.PictureBox picLoading;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboTeacher;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStudentName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColClassName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSeat_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStudentNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSubject;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCredit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRequired_By;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColIs_Required;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMakeUp_Score;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPass_Standard;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMakeUp_Standard;
        private DevComponents.DotNetBar.ButtonX btnSwap;
    }
}