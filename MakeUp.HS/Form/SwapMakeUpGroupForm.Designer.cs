namespace MakeUp.HS.Form
{
    partial class SwapMakeUpGroupForm
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
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.cboMakeupGroup = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Location = new System.Drawing.Point(206, 96);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(102, 23);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "離開";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(96, 96);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 23);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cboMakeupGroup
            // 
            this.cboMakeupGroup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboMakeupGroup.DisplayMember = "Text";
            this.cboMakeupGroup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboMakeupGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMakeupGroup.FormattingEnabled = true;
            this.cboMakeupGroup.ItemHeight = 19;
            this.cboMakeupGroup.Location = new System.Drawing.Point(81, 27);
            this.cboMakeupGroup.Name = "cboMakeupGroup";
            this.cboMakeupGroup.Size = new System.Drawing.Size(230, 25);
            this.cboMakeupGroup.TabIndex = 27;
            // 
            // labelX4
            // 
            this.labelX4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(14, 27);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(61, 23);
            this.labelX4.TabIndex = 26;
            this.labelX4.Text = "補考群組:";
            // 
            // SwapMakeUpGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 131);
            this.Controls.Add(this.cboMakeupGroup);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(339, 170);
            this.MinimumSize = new System.Drawing.Size(339, 170);
            this.Name = "SwapMakeUpGroupForm";
            this.Text = "移至其他群組";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboMakeupGroup;
        private DevComponents.DotNetBar.LabelX labelX4;
    }
}