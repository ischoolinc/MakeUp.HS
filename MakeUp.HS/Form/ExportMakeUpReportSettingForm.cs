using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Aspose.Cells;

using K12.Data;
using System.Xml;
using System.Data;
using FISCA.Data;
using System.Xml.Linq;
using System.ComponentModel;
using FISCA.Presentation.Controls;

namespace MakeUp.HS.Form
{
    public partial class ExportMakeUpReportSettingForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<UDT_MakeUpBatch> _batchList = new List<UDT_MakeUpBatch>();

        private bool _isFirstLoad = true;

        // 學年度
        private string _schoolYear;

        // 學期
        private string _semester;

        // 目標梯次 ID
        private string _targetBatchID;

        // 目標梯次ID (listItem tag 、DataGridViewRow Tag)
        private string _targetGroupID;

        // 列印方式
        private string _printMode;

        private BackgroundWorker _worker;

        private List<UDT_MakeUpGroup> _makeUpGroupList = new List<UDT_MakeUpGroup>();

        //  <BatchID,Batch>
        private Dictionary<string, UDT_MakeUpBatch> _MakeUpBatchDict = new Dictionary<string, UDT_MakeUpBatch>();

        // 補考 成績結構 <MakeUpGroupID,List<scoreItem>>
        private Dictionary<string, List<UDT_MakeUpData>> _scoreDict = new Dictionary<string, List<UDT_MakeUpData>>();


        private List<UDT_ReportTemplate> _configuresList = new List<UDT_ReportTemplate>();

        private UDT_ReportTemplate _configure { get; set; }


        public ExportMakeUpReportSettingForm(string targetGroupID, string printMode)
        {
            InitializeComponent();

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;

            _printMode = printMode;

            // 依照選擇的模式(依群組、依學生) 修改標題
            this.Text = "產生補考公告" + "(" + _printMode + ")";

            labelMemo.Text = "本功能將選擇梯次內所有補考資料，" + _printMode + "分類列印報表。";

            FISCA.UDT.AccessHelper _AccessHelper = new FISCA.UDT.AccessHelper();

            // 一開始就先 select 假如沒有的話， 傳預設樣板上去
            _configuresList = _AccessHelper.Select<UDT_ReportTemplate>();

            if (_configuresList.Count == 0)
            {
                UDT_ReportTemplate configure1 = new UDT_ReportTemplate();

                configure1.Template = new Aspose.Words.Document(new MemoryStream(Properties.Resources.補考公告樣板_依群組_));

                configure1.PrintMode = "依群組";

                configure1.Encode();
                configure1.Save();

                UDT_ReportTemplate configure2 = new UDT_ReportTemplate();

                configure2.Template = new Aspose.Words.Document(new MemoryStream(Properties.Resources.補考公告樣板_依學生_));

                configure2.PrintMode = "依學生";

                configure2.Encode();
                configure2.Save();

            }

            
            string qry = "PrintMode = '" + _printMode + "'";

            _configuresList = _AccessHelper.Select<UDT_ReportTemplate>(qry);

            if (_configuresList.Count > 0)
            {
                _configure = _configuresList[0];

                _configure.Decode();
            }

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            LoadMakeUpGroup(_targetBatchID);

        }

        /// <summary>
        /// 依補考梯次 取得所有 補考群組 以及 其下 所有的補考資料
        /// </summary>
        /// <param name="targetBatchID"></param>
        private void LoadMakeUpGroup(string targetBatchID)
        {

            _worker.ReportProgress(0, "取得補考群組、補考資料...");

            _scoreDict.Clear();
            ;

            #region 取得補考群組

            string query = @"
SELECT 
$make.up.group.uid
,$make.up.group.ref_makeup_batch_id
,$make.up.group.makeup_group
,$make.up.group.ref_teacher_id
,$make.up.group.description
,COUNT($make.up.data.uid) AS studentCount
FROM  $make.up.group
LEFT JOIN  $make.up.data ON  $make.up.data.ref_makeup_group_id :: BIGINT = $make.up.group.uid
WHERE  $make.up.group.ref_makeup_batch_id = '" + targetBatchID + @"'
GROUP BY  $make.up.group.uid 
ORDER BY $make.up.group.makeup_group";

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            //整理目前的補考梯次 資料
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    UDT_MakeUpGroup group = new UDT_MakeUpGroup();

                    group.UID = "" + row["uid"];

                    //補考群組
                    group.MakeUp_Group = "" + row["makeup_group"];

                    //補考群組 參考梯次uid
                    group.Ref_MakeUp_Batch_ID = "" + row["ref_makeup_batch_id"];

                    //閱卷老師 ID
                    group.Ref_Teacher_ID = "" + row["ref_teacher_id"];

                    //補考人數
                    group.StudentCount = "" + row["studentCount"]; ;

                    // 描述
                    group.Description = "" + row["description"];


                    if (!_scoreDict.ContainsKey(group.UID))
                    {
                        _scoreDict.Add(group.UID, new List<UDT_MakeUpData>());
                    }

                    // 所屬補考梯次
                    group.MakeUpBatch = _batchList.Find(b => b.UID == "" + row["ref_makeup_batch_id"]);

                    _makeUpGroupList.Add(group);

                }
            }
            #endregion

            #region 取得補考資料

            List<string> groupIDList = new List<string>();

            foreach (UDT_MakeUpGroup group in _makeUpGroupList)
            {
                groupIDList.Add("'" + group.UID + "'");
            }

            string groupIDs = string.Join(",", groupIDList);


            query = @"
SELECT 
    $make.up.data.uid
    ,$make.up.data.ref_student_id    
    ,$make.up.data.ref_makeup_group_id    
    ,student.name AS student_name
    ,dept.name AS department
    ,class.class_name
    ,student.seat_no
    ,student.student_number
    ,$make.up.data.subject
    ,$make.up.data.level 
    ,$make.up.data.credit 
    ,$make.up.data.c_is_required_by 
    ,$make.up.data.c_is_required 
    ,$make.up.data.score 
    ,$make.up.data.makeup_score 
    ,$make.up.data.pass_standard 
    ,$make.up.data.makeup_standard 
FROM $make.up.data
    LEFT JOIN student ON student.id = $make.up.data.ref_student_id :: BIGINT
    LEFT JOIN class ON class.id = student.ref_class_id    
    LEFT JOIN dept ON dept.id = student.ref_dept_id OR dept.id = class.ref_dept_id
WHERE
    $make.up.data.Ref_MakeUp_Group_ID IN (" + groupIDs + ")";


            qh = new QueryHelper();
            dt = qh.Select(query);

            //整理目前的補考資料
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    UDT_MakeUpData data = new UDT_MakeUpData();

                    data.UID = "" + row["uid"];

                    // 參考 補考群組 id
                    data.Ref_MakeUp_Group_ID = "" + row["ref_makeup_group_id"];


                    //學生ID 
                    data.Ref_Student_ID = "" + row["ref_student_id"];

                    //學生科別
                    data.Department = "" + row["department"];

                    //學生姓名
                    data.StudentName = "" + row["student_name"];

                    //學生班級
                    data.ClassName = "" + row["class_name"];

                    //學生座號
                    data.Seat_no = "" + row["seat_no"];

                    //學生學號
                    data.StudentNumber = "" + row["student_number"];

                    //科目
                    data.Subject = "" + row["subject"];

                    //科目
                    data.Level = "" + row["level"];

                    //學分
                    data.Credit = "" + row["credit"];

                    //校部定
                    data.C_Is_Required_By = "" + row["c_is_required_by"];

                    //必選修
                    data.C_Is_Required = "" + row["c_is_required"];

                    //成績分數(原始成績)
                    data.Score = "" + row["score"];

                    //補考分數
                    data.MakeUp_Score = "" + row["makeup_score"];

                    //及格標準
                    data.Pass_Standard = "" + row["pass_standard"];

                    //補考標準
                    data.MakeUp_Standard = "" + row["makeup_standard"];


                    if (_scoreDict.ContainsKey(data.Ref_MakeUp_Group_ID))
                    {
                        _scoreDict[data.Ref_MakeUp_Group_ID].Add(data);
                    }
                }
            }
            #endregion

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }


        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            

            FISCA.Presentation.MotherForm.SetStatusBarMessage("補考公告產生完成");
        }


        /// <summary>
        /// 按下「關閉」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 按下「匯出到 Excel」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {








        }


        // 下載合併欄位 功能變數總表
        private void linklabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Aspose.Words.Document doc = new Aspose.Words.Document();

            if (_printMode == "依群組")
            {
                doc = new Aspose.Words.Document(new MemoryStream(Properties.Resources.補考公告功能變數_依群組_));
            }

            if (_printMode == "依學生")
            {
                doc = new Aspose.Words.Document(new MemoryStream(Properties.Resources.補考公告功能變數_依學生_));
            }

            #region 儲存檔案
            string inputReportName = "合併欄位總表" + "(" + _printMode + ")";
            string reportName = inputReportName;

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".docx");

            if (System.IO.File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!System.IO.File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                doc.Save(path, Aspose.Words.SaveFormat.Docx);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".docx";
                sd.Filter = "Word檔案 (*.docx)|*.docx|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        doc.Save(path, Aspose.Words.SaveFormat.Docx);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            #endregion



        }

        // 檢視套印樣板
        private void linklabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            // 當沒有設定檔
            if (_configure == null) return;
            linklabel1.Enabled = false;
            #region 儲存檔案

            string reportName = "補考公告樣板(" + _configure.PrintMode + ").docx";

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".docx");

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                System.IO.FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                _configure.Template.Save(stream, Aspose.Words.SaveFormat.Docx);

                stream.Flush();
                stream.Close();
                System.Diagnostics.Process.Start(path);
            }
            catch
            {

            }
            linklabel1.Enabled = true;
            #endregion



        }

        // 變更套印樣板
        private void linklabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_configure == null) return;
            linklabel2.Enabled = false;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "上傳樣板";
            dialog.Filter = "Word檔案 (*.doc)|*.doc|Word檔案 (*.docx)|*.docx|所有檔案 (*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    _configure.Template = new Aspose.Words.Document(dialog.FileName);
                    _configure.Encode();
                    _configure.Save();
                    MessageBox.Show("樣板上傳成功。");
                }
                catch
                {
                    MessageBox.Show("樣板開啟失敗。");
                }
            }
            linklabel2.Enabled = true;

        }

        // 產生功能變數小工具 很好用
        private void CreateFieldTemplate()
        {
            Aspose.Words.Document doc = new Aspose.Words.Document();
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            builder.StartTable();

            builder.InsertCell();
            builder.Write("補考群組名稱");
            builder.InsertCell();
            builder.Write("補考日期");
            builder.InsertCell();
            builder.Write("補考時間");
            builder.InsertCell();
            builder.Write("補考地點");
            builder.InsertCell();            
            builder.Write("補考群組描述");
            builder.InsertCell();
            builder.Write("科目名稱");
            builder.InsertCell();
            builder.Write("科目級別");
            ;

            builder.EndRow();

            for (int i = 1; i <= 30; i++)
            {
                builder.InsertCell();
                builder.InsertField("MERGEFIELD " + "補考群組名稱" + i + " \\* MERGEFORMAT ", "«補考群組名稱" + i+"»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD " + "補考日期" + i + " \\* MERGEFORMAT ", "«補考日期" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD " + "補考時間" + i + " \\* MERGEFORMAT ", "«補考時間" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD " + "補考地點" + i + " \\* MERGEFORMAT ", "«補考地點" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD " + "補考群組描述" + i + " \\* MERGEFORMAT ", "«補考群組描述" + i + "»");
                builder.InsertCell();                
                builder.InsertField("MERGEFIELD " + "科目名稱" + i + " \\* MERGEFORMAT ", "«科目名稱" + i + "»");
                builder.InsertCell();
                builder.InsertField("MERGEFIELD " + "科目級別" + i + " \\* MERGEFORMAT ", "«科目級別" + i + "»");
                builder.EndRow();
            }

            builder.EndTable();
            builder.Writeln();

            #region 儲存檔案
            string inputReportName = "合併欄位總表";
            string reportName = inputReportName;

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".docx");

            if (System.IO.File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!System.IO.File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                doc.Save(path, Aspose.Words.SaveFormat.Docx);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".docx";
                sd.Filter = "Word檔案 (*.docx)|*.docx|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        doc.Save(path, Aspose.Words.SaveFormat.Docx);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            #endregion

        }

    }
}

