using System;
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
    public partial class MakeUpScoreStatusForm : FISCA.Presentation.Controls.BaseForm
    {

        private List<GroupDataGridViewRow> _groupDataGridViewRowList;

        private List<UDT_MakeUpBatch> _batchList = new List<UDT_MakeUpBatch>();

        private List<UDT_MakeUpGroup> _makeUpGroupList = new List<UDT_MakeUpGroup>();

        private List<TeacherRecord> _teacherList = new List<TeacherRecord>();

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

        //  學生修課資料
        private List<K12.Data.SCAttendRecord> _scaList = new List<SCAttendRecord>();

        // 補考 成績結構 <MakeUpGroupID,List<scoreItem>>
        private Dictionary<string, List<UDT_MakeUpData>> _scoreDict = new Dictionary<string, List<UDT_MakeUpData>>();




        public MakeUpScoreStatusForm()
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

            chkDisplayNotFinish.Enabled = false;
            picLoading.Visible = false;

            _isFirstLoad = false;
        }

        private void GetMakeUpBatch()
        {
            // 清空舊資料
            dataGridViewX1.Rows.Clear();
            picLoading.Visible = true;

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


            #region 取得學生 補考資料(本梯次內 所有的群組)


            //_scaList = K12.Data.SCAttend.SelectByCourseIDs(_CourseIDList);

            #endregion

        }


        private void FillCboMakeUpbatch()
        {
            picLoading.Visible = true;

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
        /// 更新ListView
        /// </summary>
        private void RefreshListView()
        {
            picLoading.Visible = true;

            #region DataGridView  新介面
            // 清內容
            dataGridViewX1.Rows.Clear();

            // 清表頭
            dataGridViewX1.Columns.Clear();

            // 補考群組名稱表頭
            DataGridViewColumn colGroupName = new DataGridViewColumn();

            colGroupName.CellTemplate = new DataGridViewTextBoxCell();
            colGroupName.Name = "colGroupName";
            colGroupName.HeaderText = "補考群組名稱";
            colGroupName.ReadOnly = true;
            colGroupName.Width = 200;
            dataGridViewX1.Columns.Add(colGroupName);

            // 閱卷老師表頭
            DataGridViewColumn colRefTeacher = new DataGridViewColumn();
            colRefTeacher.CellTemplate = new DataGridViewTextBoxCell();
            colRefTeacher.Name = "colRefTeacher";
            colRefTeacher.HeaderText = "閱卷老師表頭";
            colRefTeacher.ReadOnly = true;
            colRefTeacher.Width = 200;
            dataGridViewX1.Columns.Add(colRefTeacher);



            // 填寫完畢項目表頭
            DataGridViewColumn colTotalStatus = new DataGridViewColumn();

            colTotalStatus.CellTemplate = new DataGridViewTextBoxCell();
            colTotalStatus.Name = "colTotalStatus";
            colTotalStatus.HeaderText = "填寫完畢項目";
            colTotalStatus.ReadOnly = true;
            colTotalStatus.Width = 130;
            dataGridViewX1.Columns.Add(colTotalStatus);

            // 填寫完畢項目表頭
            DataGridViewColumn colDescription = new DataGridViewColumn();

            colDescription.CellTemplate = new DataGridViewTextBoxCell();
            colDescription.Name = "colDescription";
            colDescription.HeaderText = "描述";
            colDescription.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colDescription.ReadOnly = true;
            colDescription.Width = 130;
            dataGridViewX1.Columns.Add(colDescription);


            #endregion


            // 暫停畫面控制項
            chkDisplayNotFinish.Enabled = false;
            cboMakeUpBatch.SuspendLayout();
            cboSchoolYear.SuspendLayout();
            cbosemester.SuspendLayout();
            dataGridViewX1.SuspendLayout();


            // 選擇的項目
            DevComponents.DotNetBar.ComboBoxItem selectedItem = (DevComponents.DotNetBar.ComboBoxItem)cboMakeUpBatch.SelectedItem;

            _targetBatchID = "" + selectedItem.Tag;

            _worker.RunWorkerAsync();
        }


        /// <summary>
        /// 依補考梯次 取得所有 補考群組 以及 其下 所有的補考資料
        /// </summary>
        /// <param name="targetBatchID"></param>
        private void LoadMakeUpGroup(string targetBatchID)
        {

            _worker.ReportProgress(0, "取得補考群組、補考資料...");


            _teacherList.Clear();
            _scoreDict.Clear();


            // 取得所有教師資料 之後可以對照出 閱卷老師
            _teacherList = K12.Data.Teacher.SelectAll();


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

                    // 閱卷老師全名 老師名字(綽號)
                    group.TeacherName = _teacherList.Find(t => t.ID == "" + row["ref_teacher_id"]) != null ? _teacherList.Find(t => t.ID == "" + row["ref_teacher_id"]).Name + "(" + _teacherList.Find(t => t.ID == "" + row["ref_teacher_id"]).Nickname + ")" : "" ;

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

                    //成績分數
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




            


            int scoreCount = 0;

            // 填 DataGridView
            _groupDataGridViewRowList = new List<GroupDataGridViewRow>();

            foreach (string groupID in _scoreDict.Keys)
            {

                _worker.ReportProgress(60 + 30 * (scoreCount++ / _scoreDict.Keys.Count), "取得補考成績資料...");

                GroupDataGridViewRow row = new GroupDataGridViewRow(groupID, _scoreDict[groupID],_makeUpGroupList);

                row.Tag = groupID; // 用補考群組 ID 當作 Tag

                _groupDataGridViewRowList.Add(row);


            }
        }



        /// <summary>
        /// 將課程填入 DataGridView
        /// </summary>
        private void FillGroups(List<GroupDataGridViewRow> list)
        {
            // 有東西 才更新，沒項目 就全部清掉
            if (list.Count > 0)
            {
                dataGridViewX1.SuspendLayout();
                dataGridViewX1.Rows.Clear();
                dataGridViewX1.Rows.AddRange(list.ToArray());
                dataGridViewX1.ResumeLayout();

            }
            else
            {
                dataGridViewX1.Rows.Clear();
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
        /// 改變「僅顯示未完成輸入之課程」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDisplayNotFinish_CheckedChanged(object sender, EventArgs e)
        {
            FillGroups(GetDisplayDataGridViewList());
        }




        /// <summary>
        /// 取得要顯示的 CourseListViewItemList
        /// </summary>
        /// <returns></returns>
        private List<GroupDataGridViewRow> GetDisplayDataGridViewList()
        {
            if (chkDisplayNotFinish.Checked == true)
            {
                List<GroupDataGridViewRow> list = new List<GroupDataGridViewRow>();
                foreach (GroupDataGridViewRow item in _groupDataGridViewRowList)
                {
                    if (item.IsFinish) continue;
                    list.Add(item);
                }
                return list;
            }
            else
            {
                return _groupDataGridViewRowList;
            }
        }

        /// <summary>
        /// 按下「匯出到 Excel」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet ws = book.Worksheets[book.Worksheets.Add()];
            ws.Name = "補考成績輸入檢視.xlsx";

            int index = 0;
            Dictionary<string, int> map = new Dictionary<string, int>();

            #region 建立標題
            for (int i = 0; i < dataGridViewX1.Columns.Count; i++)
            {
                DataGridViewColumn col = dataGridViewX1.Columns[i];
                ws.Cells[index, i].PutValue(col.HeaderText);
                map.Add(col.HeaderText, i);
            }
            index++;
            #endregion

            #region 填入內容
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    int column = map[cell.OwningColumn.HeaderText];
                    ws.Cells[index, column].PutValue("" + cell.Value);
                }
                index++;
            }
            #endregion

            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "補考成績輸入檢視";
            sd.Filter = "Excel檔案(*.xlsx)|*.xlsx";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                DialogResult result = new DialogResult();

                try
                {
                    book.Save(sd.FileName, SaveFormat.Xlsx);
                    result = MsgBox.Show("檔案儲存完成，是否開啟檔案?", "是否開啟", MessageBoxButtons.YesNo);
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
        }

        /// <summary>
        /// 按下「重新整理」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {

            RefreshListView();
        }



        /// <summary>
        /// 每一筆課程的評量狀況 (DataGridView)
        /// </summary>
        private class GroupDataGridViewRow : DataGridViewRow
        {
            //  已輸入人數 / 總人數 (教師名稱)
            private const string Format = "{0}/{1} ({2})";

            private bool _is_finish;
            public bool IsFinish { get { return _is_finish; } }


            public GroupDataGridViewRow(string groupID, List<UDT_MakeUpData> scoreList, List<UDT_MakeUpGroup> gourpList)
            {

                // 預設是完成
                _is_finish = true;

                // 補考群組名稱

                DataGridViewCell batchNameCell = new DataGridViewTextBoxCell();

                batchNameCell.Value = gourpList.Find(g => g.UID == groupID) != null ? gourpList.Find(g => g.UID == groupID).MakeUp_Group : "";

                this.Cells.Add(batchNameCell);


                // 閱卷老師名稱

                DataGridViewCell teacherCell = new DataGridViewTextBoxCell();

                teacherCell.Value = gourpList.Find(g => g.UID == groupID) != null ? gourpList.Find(g => g.UID == groupID).TeacherName : "";

                this.Cells.Add(teacherCell);


                //補考分數 已完成 數量

                int scoreCount = 0;

                // 總補考資料筆數
                string total = "" + scoreList.Count();

                foreach (UDT_MakeUpData data in scoreList)
                {
                    if (data.MakeUp_Score != null && data.MakeUp_Score != "")
                    {
                        scoreCount++;
                    }
                }

                string ScoreField = string.Format("{0}/{1}", scoreCount, total);


                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = ScoreField;

                DataGridViewCellStyle style = new DataGridViewCellStyle();

                style.ForeColor = (total == "" + scoreCount) ? Color.Black : Color.Red;

                cell.Style = style;

                this.Cells.Add(cell);

                // 假如 應完成數量 與 完成數量不同 標記 "未完成"
                if (total != "" + scoreCount)
                {
                    _is_finish = false;
                }


                // 補考群組描述

                DataGridViewCell batchDescriptionCell = new DataGridViewTextBoxCell();

                batchDescriptionCell.Value = gourpList.Find(g => g.UID == groupID) != null ? gourpList.Find(g => g.UID == groupID).Description : "";

                this.Cells.Add(batchDescriptionCell);

            }


        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            LoadMakeUpGroup(_targetBatchID);

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 繼續 畫面控制項       
            picLoading.Visible = false;
            chkDisplayNotFinish.Enabled = true;
            cboMakeUpBatch.ResumeLayout();
            cboSchoolYear.ResumeLayout();
            cbosemester.ResumeLayout();

            dataGridViewX1.ResumeLayout();

            FillGroups(GetDisplayDataGridViewList());

            FISCA.Presentation.MotherForm.SetStatusBarMessage("取得補考成績輸入狀態完成");

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }


        private void dataGridViewX1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (e.ColumnIndex < 0) return;
            if (e.RowIndex < 0) return;
            DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            _targetGroupID = "" + cell.OwningRow.Tag; //  targetTermName

           
            UDT_MakeUpGroup selectedGroup = _makeUpGroupList.Find(g => g.UID == _targetGroupID);


            // 管理補考成績                
            InsertUpdateMakeUpGroupForm iumgf = new InsertUpdateMakeUpGroupForm(_schoolYear, _semester, "管理補考成績", selectedGroup);

            iumgf.ShowDialog();

            RefreshListView(); // 更改完成績後，重整畫面
        }


        /// <summary>
        /// 當補考梯次改變時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboMakeUpBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshListView();
        }

        private void cbosemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 非第一次開啟，則 切換到該學期的補考梯次
            if (!_isFirstLoad)
            {
                _semester = cbosemester.Text;
                
                GetMakeUpBatch();

                FillCboMakeUpbatch();

                chkDisplayNotFinish.Enabled = false;
                picLoading.Visible = false;

            }
        }

        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 非第一次開啟，則 切換到該學期的補考梯次
            if (!_isFirstLoad)
            {
                _schoolYear = cboSchoolYear.Text;

                GetMakeUpBatch();

                FillCboMakeUpbatch();

                chkDisplayNotFinish.Enabled = false;
                picLoading.Visible = false;

            }
        }
    }
}

