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
    public partial class ExportMakeUpScoreForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<UDT_MakeUpBatch> _batchList = new List<UDT_MakeUpBatch>();

        private List<UDT_MakeUpGroup> _makeUpGroupList = new List<UDT_MakeUpGroup>();

        private bool _isFirstLoad = true;

        private BackgroundWorker _worker;

        // 學年度
        private string _schoolYear;

        // 學期
        private string _semester;

        // 目標梯次 ID
        private string _targetBatchID;

        // 目標梯次ID (listItem tag 、DataGridViewRow Tag)
        private string _targetGroupID;


        //  <BatchID,Batch>
        private Dictionary<string, UDT_MakeUpBatch> _MakeUpBatchDict = new Dictionary<string, UDT_MakeUpBatch>();

        // 補考 成績結構 <MakeUpGroupID,List<scoreItem>>
        private Dictionary<string, List<UDT_MakeUpData>> _scoreDict = new Dictionary<string, List<UDT_MakeUpData>>();


        public ExportMakeUpScoreForm()
        {
            InitializeComponent();

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;


            // 學年度 
            cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 3);
            cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 2);
            cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 1);
            cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear));

            // 學期
            cbosemester.Items.Add(1);
            cbosemester.Items.Add(2);

            _schoolYear = School.DefaultSchoolYear;
            _semester = School.DefaultSemester;

            // 預設為學校的當學年度學期
            cboSchoolYear.Text = School.DefaultSchoolYear;
            cbosemester.Text = School.DefaultSemester;

            GetMakeUpBatch();

            FillCboMakeUpbatch();

            _isFirstLoad = false;
        }

        private void GetMakeUpBatch()
        {

            #region 取得 該學年度學期的 補考梯次            
            string query = @"
                    SELECT 
                    * 
                    FROM $make.up.batch
                    WHERE
                    school_year = '" + _schoolYear + "'" +
                        "AND semester = '" + _semester + "'";

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            _batchList.Clear(); // 清空

            //整理目前的ESL 課程資料
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    UDT_MakeUpBatch batch = new UDT_MakeUpBatch();

                    batch.UID = "" + dr["uid"];

                    //補考梯次
                    batch.MakeUp_Batch = "" + dr["makeup_batch"];

                    //學年度
                    batch.School_Year = "" + dr["school_year"];

                    //學期
                    batch.Semester = "" + dr["semester"];

                    //補考說明
                    batch.Description = "" + dr["description"];

                    //包含班級id
                    batch.Included_Class_ID = "" + dr["included_class_id"];

                    _batchList.Add(batch);

                }
            }
            #endregion

        }


        private void FillCboMakeUpbatch()
        {

            cboMakeUpBatch.Items.Clear();

            foreach (UDT_MakeUpBatch batch in _batchList)
            {
                object item = new object();

                DevComponents.DotNetBar.ComboBoxItem cboI = new DevComponents.DotNetBar.ComboBoxItem();

                cboI.Text = batch.MakeUp_Batch;

                // Tag  用 梯次的 id ， 方便 之後 查詢 補考群組、補考資料
                cboI.Tag = batch.UID;

                cboMakeUpBatch.Items.Add(cboI);
            }

            // 假如 梯次 數大於 0 ，預設 選擇 第一項
            if (_batchList.Count > 0)
            {
                cboMakeUpBatch.SelectedIndex = 0;

            }
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
            if (targetBatchID != "")
            {
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
                    _makeUpGroupList.Clear();
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


                if (groupIDs != "")
                {
                    //透過 group_id 取得學生修課及格與補考標準
                    Utility uti = new Utility();
                    Dictionary<string, DataRow> studPassScoreDict = uti.GetStudentMakeupPassScoreByGroupIDs(groupIDList);


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
    $make.up.data.ref_makeup_group_id IN (" + groupIDs + ")";

                    qh = new QueryHelper();
                    dt = qh.Select(query);

                    // 取得學生學年度學期修課標準， 如果沒有使用預設，比對後填入


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


                            string key = data.Ref_MakeUp_Group_ID + "_" + data.Ref_Student_ID + "_" + data.Subject + "_" + data.Level;

                            if (studPassScoreDict.ContainsKey(key))
                            {
                                //及格標準
                                if (studPassScoreDict[key]["passing_standard"] != null)
                                    data.Pass_Standard = studPassScoreDict[key]["passing_standard"].ToString();

                                //補考標準
                                if (studPassScoreDict[key]["makeup_standard"] != null)
                                    data.MakeUp_Standard = studPassScoreDict[key]["makeup_standard"].ToString();
                            }

                            ////及格標準
                            //data.Pass_Standard = "" + row["pass_standard"];

                            ////補考標準
                            //data.MakeUp_Standard = "" + row["makeup_standard"];


                            if (_scoreDict.ContainsKey(data.Ref_MakeUp_Group_ID))
                            {
                                _scoreDict[data.Ref_MakeUp_Group_ID].Add(data);
                            }
                        }
                    }
                }

                #endregion
            }





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
            // 暫停畫面控制項

            cboMakeUpBatch.SuspendLayout();
            cboSchoolYear.SuspendLayout();
            cbosemester.SuspendLayout();


            // 選擇的項目
            DevComponents.DotNetBar.ComboBoxItem selectedItem = (DevComponents.DotNetBar.ComboBoxItem)cboMakeUpBatch.SelectedItem;

            if (selectedItem == null)
            {
                MsgBox.Show("梯次別不得空白", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if ("" + selectedItem.Text == "")
                {
                    MsgBox.Show("梯次別不得空白", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            btnExport.Enabled = false;
            _targetBatchID = "" + selectedItem.Tag;

            _worker.RunWorkerAsync();

        }




        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            LoadMakeUpGroup(_targetBatchID);

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 繼續 畫面控制項       
            btnExport.Enabled = true;
            cboMakeUpBatch.ResumeLayout();
            cboSchoolYear.ResumeLayout();
            cbosemester.ResumeLayout();

            // 取得 系統預設的樣板            
            Workbook book = new Workbook(new MemoryStream(Properties.Resources.匯入學期科目成績_補考成績_));
            Worksheet ws = book.Worksheets[0];


            int index = 1;

            // 沒有補考分數的清單
            List<UDT_MakeUpData> noMakeUpscoreList = new List<UDT_MakeUpData>();

            #region 填入內容

            foreach (string groupID in _scoreDict.Keys)
            {
                foreach (UDT_MakeUpData score in _scoreDict[groupID])
                {
                    // 沒有補考分數 另外處理 不在匯出名單中
                    if (score.MakeUp_Score == null || score.MakeUp_Score == "")
                    {
                        noMakeUpscoreList.Add(score);

                        continue;
                    }

                    // 學生系統編號
                    ws.Cells[index, 0].PutValue("" + score.Ref_Student_ID);

                    // 學號
                    ws.Cells[index, 1].PutValue("" + score.StudentNumber);

                    // 班級
                    ws.Cells[index, 2].PutValue("" + score.ClassName);

                    // 座號
                    ws.Cells[index, 3].PutValue("" + score.Seat_no);

                    // 科別
                    ws.Cells[index, 4].PutValue("" + score.Department);

                    // 姓名
                    ws.Cells[index, 5].PutValue("" + score.StudentName);

                    // 科目
                    ws.Cells[index, 6].PutValue("" + score.Subject);

                    // 科目級別
                    ws.Cells[index, 7].PutValue("" + score.Level);

                    // 學年度
                    ws.Cells[index, 8].PutValue(_schoolYear);

                    // 學期
                    ws.Cells[index, 9].PutValue(_semester);

                    // 是否取得學分
                    string getCredit = "否";

                    // 補考後的分數
                    decimal makeUp_Score = new decimal();

                    // 原始成績
                    decimal ori_score = new decimal();

                    // 及格標準
                    decimal pass_standard = new decimal();


                    bool makeUp_ScoreHasValue = decimal.TryParse("" + score.MakeUp_Score, out makeUp_Score);

                    bool pass_standardHasValue = decimal.TryParse("" + score.Pass_Standard, out pass_standard);


                    if (makeUp_ScoreHasValue && pass_standardHasValue)
                    {
                        //補考分數 等於或是 比 及格標準高 ， 則取得學分 ， 補考分數 及格標準計
                        if (makeUp_Score >= pass_standard)
                        {
                            getCredit = "是";

                            makeUp_Score = pass_standard;
                        }
                        // 補考分數 比 及格標準低 ， 沒有取得分數，補考分數 以原始分數計
                        else
                        {

                        }
                    }

                    // 指定成新的補考分數
                    score.MakeUp_Score = "" + makeUp_Score;

                    // 原始成績
                    ws.Cells[index, 10].PutValue("" + score.Score);

                    // 補考成績
                    ws.Cells[index, 11].PutValue("" + score.MakeUp_Score);

                    // 及格標準
                    ws.Cells[index, 12].PutValue("" + score.Pass_Standard);


                    // 取得學分
                    ws.Cells[index, 13].PutValue(getCredit);


                    index++;

                }
            }


            // 沒有補考分數的名單 提醒使用者
            Worksheet ws_warning = book.Worksheets[1];

            index = 1;

            foreach (UDT_MakeUpData score in noMakeUpscoreList)
            {

                // 學生系統編號
                ws_warning.Cells[index, 0].PutValue("" + score.Ref_Student_ID);

                // 學號
                ws_warning.Cells[index, 1].PutValue("" + score.StudentNumber);

                // 班級
                ws_warning.Cells[index, 2].PutValue("" + score.ClassName);

                // 座號
                ws_warning.Cells[index, 3].PutValue("" + score.Seat_no);

                // 科別
                ws_warning.Cells[index, 4].PutValue("" + score.Department);

                // 姓名
                ws_warning.Cells[index, 5].PutValue("" + score.StudentName);

                // 科目
                ws_warning.Cells[index, 6].PutValue("" + score.Subject);

                // 科目級別
                ws_warning.Cells[index, 7].PutValue("" + score.Level);

                // 學年度
                ws_warning.Cells[index, 8].PutValue(_schoolYear);

                // 學期
                ws_warning.Cells[index, 9].PutValue(_semester);

                // 原始成績
                ws_warning.Cells[index, 10].PutValue("" + score.Score);

                // 補考成績
                ws_warning.Cells[index, 11].PutValue("" + score.MakeUp_Score);

                // 及格標準
                ws_warning.Cells[index, 12].PutValue("" + score.Pass_Standard);


                // 取得學分
                ws_warning.Cells[index, 13].PutValue("");


                index++;

            }




            #endregion

            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "匯入學期科目成績(補考成績)";
            sd.Filter = "Excel檔案(*.xls)|*.xls";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                DialogResult result = new DialogResult();

                try
                {
                    book.Save(sd.FileName, SaveFormat.Excel97To2003);
                    result = MsgBox.Show("檔案儲存完成，是否開啟檔案? 本補考成績匯入由系統自動以成績計算規則判斷，建議須人工檢查後再做匯入", "是否開啟", MessageBoxButtons.YesNo);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗。" + ex.Message);
                }

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show("開啟檔案發生失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            FISCA.Presentation.MotherForm.SetStatusBarMessage("補考成績匯入學期科目成績輸出完成");

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }



        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 非第一次開啟，則 切換到該學期的補考梯次
            if (!_isFirstLoad)
            {
                _schoolYear = cboSchoolYear.Text;

                GetMakeUpBatch();

                FillCboMakeUpbatch();

            }
        }

        private void cbosemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 非第一次開啟，則 切換到該學期的補考梯次
            if (!_isFirstLoad)
            {
                _semester = cbosemester.Text;

                GetMakeUpBatch();

                FillCboMakeUpbatch();



            }
        }


    }
}

