namespace MakeUp.HS.Form
{
    partial class MakeUpGroupManagerForm
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
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.expandablePanel1 = new DevComponents.DotNetBar.ExpandablePanel();
            this.btnUpdate = new DevComponents.DotNetBar.ButtonX();
            this.picLoadingAdvTreeMakeUpBatch = new System.Windows.Forms.PictureBox();
            this.advTreeMakeUpBatch = new DevComponents.AdvTree.AdvTree();
            this.node1 = new DevComponents.AdvTree.Node();
            this.node3 = new DevComponents.AdvTree.Node();
            this.node4 = new DevComponents.AdvTree.Node();
            this.node5 = new DevComponents.AdvTree.Node();
            this.node2 = new DevComponents.AdvTree.Node();
            this.node6 = new DevComponents.AdvTree.Node();
            this.node7 = new DevComponents.AdvTree.Node();
            this.node8 = new DevComponents.AdvTree.Node();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.cbosemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.expandableSplitter1 = new DevComponents.DotNetBar.ExpandableSplitter();
            this.expandablePanel2 = new DevComponents.DotNetBar.ExpandablePanel();
            this.picLoadingDgvXMakeUpGroup = new System.Windows.Forms.PictureBox();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.ColMakeUpGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColRefTeacher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMakeUpStudentCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDesription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.expandablePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoadingAdvTreeMakeUpBatch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.advTreeMakeUpBatch)).BeginInit();
            this.expandablePanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoadingDgvXMakeUpGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Excel 2003 (*.xls)|*.xls";
            this.saveFileDialog1.Title = "儲存檔案";
            // 
            // expandablePanel1
            // 
            this.expandablePanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.expandablePanel1.CollapseDirection = DevComponents.DotNetBar.eCollapseDirection.RightToLeft;
            this.expandablePanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.expandablePanel1.Controls.Add(this.btnUpdate);
            this.expandablePanel1.Controls.Add(this.picLoadingAdvTreeMakeUpBatch);
            this.expandablePanel1.Controls.Add(this.advTreeMakeUpBatch);
            this.expandablePanel1.Controls.Add(this.cboSchoolYear);
            this.expandablePanel1.Controls.Add(this.labelX1);
            this.expandablePanel1.Controls.Add(this.cbosemester);
            this.expandablePanel1.Controls.Add(this.labelX3);
            this.expandablePanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.expandablePanel1.Location = new System.Drawing.Point(0, 0);
            this.expandablePanel1.Name = "expandablePanel1";
            this.expandablePanel1.Size = new System.Drawing.Size(200, 706);
            this.expandablePanel1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.expandablePanel1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandablePanel1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.expandablePanel1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.expandablePanel1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.expandablePanel1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandablePanel1.Style.GradientAngle = 90;
            this.expandablePanel1.TabIndex = 0;
            this.expandablePanel1.TitleStyle.Alignment = System.Drawing.StringAlignment.Center;
            this.expandablePanel1.TitleStyle.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandablePanel1.TitleStyle.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.expandablePanel1.TitleStyle.Border = DevComponents.DotNetBar.eBorderType.RaisedInner;
            this.expandablePanel1.TitleStyle.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandablePanel1.TitleStyle.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.expandablePanel1.TitleStyle.GradientAngle = 90;
            this.expandablePanel1.TitleText = "補考梯次";
            this.expandablePanel1.ExpandedChanged += new DevComponents.DotNetBar.ExpandChangeEventHandler(this.expandablePanel1_ExpandedChanged);
            this.expandablePanel1.Click += new System.EventHandler(this.cbosemester_SelectedIndexChanged);
            // 
            // btnUpdate
            // 
            this.btnUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnUpdate.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnUpdate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnUpdate.Location = new System.Drawing.Point(0, 686);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(200, 20);
            this.btnUpdate.TabIndex = 14;
            this.btnUpdate.Text = "儲存";
            // 
            // picLoadingAdvTreeMakeUpBatch
            // 
            this.picLoadingAdvTreeMakeUpBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picLoadingAdvTreeMakeUpBatch.BackColor = System.Drawing.Color.White;
            this.picLoadingAdvTreeMakeUpBatch.Image = global::MakeUp.HS.Properties.Resources.loading;
            this.picLoadingAdvTreeMakeUpBatch.InitialImage = null;
            this.picLoadingAdvTreeMakeUpBatch.Location = new System.Drawing.Point(67, 318);
            this.picLoadingAdvTreeMakeUpBatch.MaximumSize = new System.Drawing.Size(44, 46);
            this.picLoadingAdvTreeMakeUpBatch.MinimumSize = new System.Drawing.Size(44, 46);
            this.picLoadingAdvTreeMakeUpBatch.Name = "picLoadingAdvTreeMakeUpBatch";
            this.picLoadingAdvTreeMakeUpBatch.Size = new System.Drawing.Size(44, 46);
            this.picLoadingAdvTreeMakeUpBatch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLoadingAdvTreeMakeUpBatch.TabIndex = 11;
            this.picLoadingAdvTreeMakeUpBatch.TabStop = false;
            this.picLoadingAdvTreeMakeUpBatch.Visible = false;
            // 
            // advTreeMakeUpBatch
            // 
            this.advTreeMakeUpBatch.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.advTreeMakeUpBatch.AllowDrop = true;
            this.advTreeMakeUpBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.advTreeMakeUpBatch.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.advTreeMakeUpBatch.BackgroundStyle.Class = "TreeBorderKey";
            this.advTreeMakeUpBatch.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.advTreeMakeUpBatch.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
            this.advTreeMakeUpBatch.Location = new System.Drawing.Point(0, 93);
            this.advTreeMakeUpBatch.Name = "advTreeMakeUpBatch";
            this.advTreeMakeUpBatch.Nodes.AddRange(new DevComponents.AdvTree.Node[] {
            this.node1,
            this.node2});
            this.advTreeMakeUpBatch.NodesConnector = this.nodeConnector1;
            this.advTreeMakeUpBatch.NodeStyle = this.elementStyle1;
            this.advTreeMakeUpBatch.PathSeparator = ";";
            this.advTreeMakeUpBatch.Size = new System.Drawing.Size(200, 570);
            this.advTreeMakeUpBatch.Styles.Add(this.elementStyle1);
            this.advTreeMakeUpBatch.TabIndex = 13;
            this.advTreeMakeUpBatch.Text = "advTree1";
            // 
            // node1
            // 
            this.node1.Expanded = true;
            this.node1.Name = "node1";
            this.node1.Nodes.AddRange(new DevComponents.AdvTree.Node[] {
            this.node3,
            this.node4,
            this.node5});
            this.node1.Selectable = false;
            this.node1.Text = "107學年度第1學期";
            // 
            // node3
            // 
            this.node3.DragDropEnabled = false;
            this.node3.Expanded = true;
            this.node3.Name = "node3";
            this.node3.Text = "普通科補考";
            // 
            // node4
            // 
            this.node4.DragDropEnabled = false;
            this.node4.Expanded = true;
            this.node4.Name = "node4";
            this.node4.Text = "商業科普考";
            // 
            // node5
            // 
            this.node5.DragDropEnabled = false;
            this.node5.Expanded = true;
            this.node5.Name = "node5";
            this.node5.Text = "資訊科補考";
            // 
            // node2
            // 
            this.node2.Expanded = true;
            this.node2.Name = "node2";
            this.node2.Nodes.AddRange(new DevComponents.AdvTree.Node[] {
            this.node6,
            this.node7,
            this.node8});
            this.node2.Selectable = false;
            this.node2.Text = "107學年度第2學期";
            // 
            // node6
            // 
            this.node6.DragDropEnabled = false;
            this.node6.Expanded = true;
            this.node6.Name = "node6";
            this.node6.Text = "普通科補考";
            // 
            // node7
            // 
            this.node7.DragDropEnabled = false;
            this.node7.Expanded = true;
            this.node7.Name = "node7";
            this.node7.Text = "商業科普考";
            // 
            // node8
            // 
            this.node8.DragDropEnabled = false;
            this.node8.Expanded = true;
            this.node8.Name = "node8";
            this.node8.Text = "資訊科補考";
            // 
            // nodeConnector1
            // 
            this.nodeConnector1.LineColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle1
            // 
            this.elementStyle1.Class = "";
            this.elementStyle1.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.elementStyle1.Name = "elementStyle1";
            this.elementStyle1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(53, 33);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(99, 25);
            this.cboSchoolYear.TabIndex = 11;
            this.cboSchoolYear.SelectedIndexChanged += new System.EventHandler(this.cboSchoolYear_SelectedIndexChanged_1);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(3, 35);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(79, 23);
            this.labelX1.TabIndex = 12;
            this.labelX1.Text = "學年度";
            // 
            // cbosemester
            // 
            this.cbosemester.DisplayMember = "Text";
            this.cbosemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbosemester.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbosemester.FormattingEnabled = true;
            this.cbosemester.ItemHeight = 19;
            this.cbosemester.Location = new System.Drawing.Point(53, 62);
            this.cbosemester.Name = "cbosemester";
            this.cbosemester.Size = new System.Drawing.Size(99, 25);
            this.cbosemester.TabIndex = 9;
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
            this.labelX3.Location = new System.Drawing.Point(3, 64);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(36, 23);
            this.labelX3.TabIndex = 10;
            this.labelX3.Text = "學期";
            // 
            // expandableSplitter1
            // 
            this.expandableSplitter1.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.expandableSplitter1.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandableSplitter1.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandableSplitter1.ExpandFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.expandableSplitter1.ExpandFillColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandableSplitter1.ExpandLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.expandableSplitter1.ExpandLineColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandableSplitter1.GripDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.expandableSplitter1.GripDarkColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandableSplitter1.GripLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.expandableSplitter1.GripLightColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.expandableSplitter1.HotBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(151)))), ((int)(((byte)(61)))));
            this.expandableSplitter1.HotBackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(184)))), ((int)(((byte)(94)))));
            this.expandableSplitter1.HotBackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemPressedBackground2;
            this.expandableSplitter1.HotBackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemPressedBackground;
            this.expandableSplitter1.HotExpandFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.expandableSplitter1.HotExpandFillColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandableSplitter1.HotExpandLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.expandableSplitter1.HotExpandLineColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandableSplitter1.HotGripDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.expandableSplitter1.HotGripDarkColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandableSplitter1.HotGripLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.expandableSplitter1.HotGripLightColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.expandableSplitter1.Location = new System.Drawing.Point(200, 0);
            this.expandableSplitter1.Name = "expandableSplitter1";
            this.expandableSplitter1.Size = new System.Drawing.Size(6, 706);
            this.expandableSplitter1.Style = DevComponents.DotNetBar.eSplitterStyle.Office2007;
            this.expandableSplitter1.TabIndex = 1;
            this.expandableSplitter1.TabStop = false;
            // 
            // expandablePanel2
            // 
            this.expandablePanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.expandablePanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.expandablePanel2.Controls.Add(this.picLoadingDgvXMakeUpGroup);
            this.expandablePanel2.Controls.Add(this.dataGridViewX1);
            this.expandablePanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expandablePanel2.ExpandButtonVisible = false;
            this.expandablePanel2.Location = new System.Drawing.Point(206, 0);
            this.expandablePanel2.Name = "expandablePanel2";
            this.expandablePanel2.Size = new System.Drawing.Size(798, 706);
            this.expandablePanel2.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.expandablePanel2.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandablePanel2.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.expandablePanel2.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.expandablePanel2.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.expandablePanel2.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandablePanel2.Style.GradientAngle = 90;
            this.expandablePanel2.TabIndex = 2;
            this.expandablePanel2.TitleStyle.Alignment = System.Drawing.StringAlignment.Center;
            this.expandablePanel2.TitleStyle.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandablePanel2.TitleStyle.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.expandablePanel2.TitleStyle.Border = DevComponents.DotNetBar.eBorderType.RaisedInner;
            this.expandablePanel2.TitleStyle.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandablePanel2.TitleStyle.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.expandablePanel2.TitleStyle.GradientAngle = 90;
            this.expandablePanel2.TitleText = "補考群組";
            // 
            // picLoadingDgvXMakeUpGroup
            // 
            this.picLoadingDgvXMakeUpGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picLoadingDgvXMakeUpGroup.BackColor = System.Drawing.Color.Transparent;
            this.picLoadingDgvXMakeUpGroup.Image = global::MakeUp.HS.Properties.Resources.loading;
            this.picLoadingDgvXMakeUpGroup.InitialImage = null;
            this.picLoadingDgvXMakeUpGroup.Location = new System.Drawing.Point(383, 318);
            this.picLoadingDgvXMakeUpGroup.MaximumSize = new System.Drawing.Size(44, 46);
            this.picLoadingDgvXMakeUpGroup.MinimumSize = new System.Drawing.Size(44, 46);
            this.picLoadingDgvXMakeUpGroup.Name = "picLoadingDgvXMakeUpGroup";
            this.picLoadingDgvXMakeUpGroup.Size = new System.Drawing.Size(44, 46);
            this.picLoadingDgvXMakeUpGroup.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLoadingDgvXMakeUpGroup.TabIndex = 12;
            this.picLoadingDgvXMakeUpGroup.TabStop = false;
            this.picLoadingDgvXMakeUpGroup.Visible = false;
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToDeleteRows = false;
            this.dataGridViewX1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColMakeUpGroupName,
            this.ColRefTeacher,
            this.ColMakeUpStudentCount,
            this.ColDesription});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(0, 26);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(798, 680);
            this.dataGridViewX1.TabIndex = 1;
            this.dataGridViewX1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewX1_CellMouseDoubleClick);
            this.dataGridViewX1.SelectionChanged += new System.EventHandler(this.dataGridViewX1_SelectionChanged);
            // 
            // ColMakeUpGroupName
            // 
            this.ColMakeUpGroupName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColMakeUpGroupName.FillWeight = 105.9603F;
            this.ColMakeUpGroupName.HeaderText = "群組名稱";
            this.ColMakeUpGroupName.Name = "ColMakeUpGroupName";
            this.ColMakeUpGroupName.ReadOnly = true;
            // 
            // ColRefTeacher
            // 
            this.ColRefTeacher.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColRefTeacher.FillWeight = 48.5805F;
            this.ColRefTeacher.HeaderText = "閱卷老師";
            this.ColRefTeacher.Name = "ColRefTeacher";
            this.ColRefTeacher.ReadOnly = true;
            // 
            // ColMakeUpStudentCount
            // 
            this.ColMakeUpStudentCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColMakeUpStudentCount.FillWeight = 51.78084F;
            this.ColMakeUpStudentCount.HeaderText = "補考人數";
            this.ColMakeUpStudentCount.Name = "ColMakeUpStudentCount";
            this.ColMakeUpStudentCount.ReadOnly = true;
            // 
            // ColDesription
            // 
            this.ColDesription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColDesription.FillWeight = 193.6784F;
            this.ColDesription.HeaderText = "描述";
            this.ColDesription.Name = "ColDesription";
            this.ColDesription.ReadOnly = true;
            // 
            // MakeUpGroupManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 706);
            this.Controls.Add(this.expandablePanel2);
            this.Controls.Add(this.expandableSplitter1);
            this.Controls.Add(this.expandablePanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(520, 300);
            this.Name = "MakeUpGroupManagerForm";
            this.Text = "補考群組管理";
            this.expandablePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLoadingAdvTreeMakeUpBatch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.advTreeMakeUpBatch)).EndInit();
            this.expandablePanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLoadingDgvXMakeUpGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private DevComponents.DotNetBar.ExpandablePanel expandablePanel1;
        private DevComponents.DotNetBar.ExpandableSplitter expandableSplitter1;
        private DevComponents.DotNetBar.ExpandablePanel expandablePanel2;
        private DevComponents.AdvTree.AdvTree advTreeMakeUpBatch;
        private DevComponents.AdvTree.Node node1;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbosemester;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.PictureBox picLoadingAdvTreeMakeUpBatch;
        private DevComponents.AdvTree.Node node3;
        private DevComponents.AdvTree.Node node4;
        private DevComponents.AdvTree.Node node5;
        private DevComponents.AdvTree.Node node2;
        private DevComponents.AdvTree.Node node6;
        private DevComponents.AdvTree.Node node7;
        private DevComponents.AdvTree.Node node8;
        private DevComponents.DotNetBar.ButtonX btnUpdate;
        private System.Windows.Forms.PictureBox picLoadingDgvXMakeUpGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMakeUpGroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColRefTeacher;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMakeUpStudentCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDesription;
    }
}