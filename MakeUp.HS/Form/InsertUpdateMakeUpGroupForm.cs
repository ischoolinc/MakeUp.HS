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
using FISCA.Authentication;
using FISCA.LogAgent;
using FISCA.DSAUtil;

namespace MakeUp.HS.Form
{
    public partial class InsertUpdateMakeUpGroupForm : FISCA.Presentation.Controls.BaseForm
    {
        // 開啟這個視窗的動作別(分為:新增群組、修改群組、管理補考成績)
        private string _action;


        // 抓補考資料用　BackgroundWorker
        private BackgroundWorker _dataWorker;

        // 補考群組(修改使用)
        private UDT_MakeUpGroup _group;

        // 補考梯次(新增使用)
        private UDT_MakeUpBatch _batch;

        // 本學期 已經有的補考群組名稱(驗證重覆用)
        private List<string> _groupNameList;

        // 補考資料
        private List<UDT_MakeUpData> _dataList;

        // 選取的補考資料
        private List<UDT_MakeUpData> _selected_dataList;

        // 教師清單
        private List<K12.Data.TeacherRecord> _teacherList;

        private string _school_year;

        private string _semester;

        // 目前補考群組的 補考梯次名
        private string _makeup_batch;

        // 修正模式 傳補考群組物件
        public InsertUpdateMakeUpGroupForm(string school_year, string semester, string action, UDT_MakeUpGroup group)
        {
            InitializeComponent();

            _school_year = school_year;

            _semester = semester;

            _dataList = new List<UDT_MakeUpData>();

            _selected_dataList = new List<UDT_MakeUpData>();

            _groupNameList = new List<string>();

            _teacherList = K12.Data.Teacher.SelectAll();

            // 加入空白教師，提供使用者可以取消
            cboTeacher.Items.Add("");

            //將教師加入清單
            foreach (K12.Data.TeacherRecord teacher in _teacherList)
            {
                // 老師全名 
                cboTeacher.Items.Add(teacher.Name + "(" + teacher.Nickname + ")");
            }



            K12.Data.TeacherRecord groupTeacher = _teacherList.Find(t => t.ID == group.Ref_Teacher_ID);

            // 預設為群組的閱卷老師
            cboTeacher.Text = groupTeacher != null ? groupTeacher.Name + "(" + groupTeacher.Nickname + ")" : "";

            _dataWorker = new BackgroundWorker();
            _dataWorker.DoWork += new DoWorkEventHandler(DataWorker_DoWork);
            _dataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DataWorker_RunWorkerCompleted);
            _dataWorker.ProgressChanged += new ProgressChangedEventHandler(DataWorker_ProgressChanged);
            _dataWorker.WorkerReportsProgress = true;

            _action = action;

            _group = group;

            // 只有新增 模式 才可以讓使用者 編輯 
            if (_action == "新增群組")
            {

            }

            // 修改模式 使用者只能修改 
            if (_action == "修改群組")
            {
                this.Text = "管理補考群組";
                txtGroupName.Text = _group.MakeUp_Group;

                txtDescription.Text = _group.Description;

                txtDate.Text = _group.MakeUp_Date;

                txtTime.Text = _group.MakeUp_Time;

                txtPlace.Text = _group.MakeUp_Place;


                txtGroupName.Enabled = false;
                txtDescription.Enabled = false;
                cboTeacher.Enabled = false;
                btnSave.Enabled = false;
                btnClose.Enabled = false;

                picLoading.Visible = true;


                _dataWorker.RunWorkerAsync();
            }

            // 修改模式 使用者只能修改 
            if (_action == "管理補考成績")
            {
                this.Text = "管理補考成績";
                txtGroupName.Text = _group.MakeUp_Group;
                txtDescription.Text = _group.Description;

                txtGroupName.Enabled = false;
                txtDescription.Enabled = false;
                cboTeacher.Enabled = false;
                btnSave.Enabled = false;
                btnClose.Enabled = false;
                txtDate.Enabled = false;
                txtTime.Enabled = false;
                txtPlace.Enabled = false;

                picLoading.Visible = true;


                _dataWorker.RunWorkerAsync();
            }


        }

        // 新增模式 傳 補考群組 物件
        public InsertUpdateMakeUpGroupForm(string school_year, string semester, string action, UDT_MakeUpBatch ref_makeup_batch)
        {
            InitializeComponent();

            _school_year = school_year;

            _semester = semester;

            _dataList = new List<UDT_MakeUpData>();

            _groupNameList = new List<string>();

            _teacherList = K12.Data.Teacher.SelectAll();

            //將教師加入清單
            foreach (K12.Data.TeacherRecord teacher in _teacherList)
            {
                // 老師全名 
                cboTeacher.Items.Add(teacher.Name + "(" + teacher.Nickname + ")");
            }


            _dataWorker = new BackgroundWorker();
            _dataWorker.DoWork += new DoWorkEventHandler(DataWorker_DoWork);
            _dataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DataWorker_RunWorkerCompleted);
            _dataWorker.ProgressChanged += new ProgressChangedEventHandler(DataWorker_ProgressChanged);
            _dataWorker.WorkerReportsProgress = true;

            _action = action;

            _batch = ref_makeup_batch;
        }

        private void InsertUpdateMakeUpBatchForm_Load(object sender, EventArgs e)
        {





            picLoading.Visible = false;
        }



        private void DataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 清空 舊資料
            _dataList.Clear();

            #region 取得本學期得其他補考群組資料 驗證重覆使用
            string query_make_up_group = @"
SELECT 
    $make.up.batch.makeup_batch
    ,$make.up.group.makeup_group
FROM $make.up.group
LEFT JOIN $make.up.batch ON $make.up.batch.uid = $make.up.group.ref_makeup_batch_id :: BIGINT 
WHERE  $make.up.batch.uid = " + _group.Ref_MakeUp_Batch_ID + " AND $make.up.group.uid !=" + _group.UID;


            QueryHelper qh_make_up_group = new QueryHelper();
            DataTable dt_make_up_group = qh_make_up_group.Select(query_make_up_group);

            if (dt_make_up_group.Rows.Count > 0)
            {
                foreach (DataRow row in dt_make_up_group.Rows)
                {
                    _makeup_batch = "" + row["makeup_batch"];

                    if (!_groupNameList.Contains("" + row["makeup_group"]))
                    {
                        _groupNameList.Add("" + row["makeup_group"]);
                    }
                }
            }
            #endregion


            #region 取得補考資料

            string query = @"
SELECT 
    $make.up.data.uid
    ,$make.up.data.ref_student_id    
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
    ,$make.up.data.decimalnumber 
    ,$make.up.data.calrole 
FROM $make.up.data
    LEFT JOIN student ON student.id = $make.up.data.ref_student_id :: BIGINT
    LEFT JOIN class ON class.id = student.ref_class_id    
    LEFT JOIN dept ON dept.id = student.ref_dept_id OR dept.id = class.ref_dept_id
WHERE
    $make.up.data.Ref_MakeUp_Group_ID = '" + _group.UID + "'";


            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            //整理目前的補考資料
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    UDT_MakeUpData data = new UDT_MakeUpData();

                    data.UID = "" + row["uid"];

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

                    // 取得本學生 成績的輸入小數位數規則
                    // 只有 管理補考成績的情境用得到， 目前會要用比較慢一筆一筆學生查詢，而不直接寫在SQL內的原因
                    // 是因為要跟 產生補考清單 時 使用的API 邏輯一致
                    if (_action == "管理補考成績")
                    {
                        data.DecimalNumber = "" + row["decimalnumber"];

                        data.HasNewMakeUpScore = false;

                        // 成績身分
                        data.CalRole = "" + row["calrole"];
                    }
                    

                    _dataList.Add(data);
                }
            }
            #endregion

        }

        private void DataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridViewX1.Rows.Clear();

            // 填寫補考資料
            foreach (UDT_MakeUpData data in _dataList)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                // row 的 Tag 為 補考資料的系統編號
                row.Tag = data.UID;

                row.Cells[0].Value = data.StudentName;

                row.Cells[1].Value = data.Department;

                row.Cells[2].Value = data.ClassName;

                row.Cells[3].Value = data.Seat_no;

                row.Cells[4].Value = data.StudentNumber;

                row.Cells[5].Value = data.Subject;

                row.Cells[6].Value = data.Level;

                row.Cells[7].Value = data.Credit;

                row.Cells[8].Value = data.C_Is_Required_By;

                row.Cells[9].Value = data.C_Is_Required;

                row.Cells[10].Value = data.Score;

                row.Cells[11].Value = data.MakeUp_Score;

                row.Cells[12].Value = data.Pass_Standard;

                row.Cells[13].Value = data.MakeUp_Standard;

                dataGridViewX1.Rows.Add(row);
            }

            // 管理補考成績 只能輸入 補考資料的分數 其他項目不得編輯
            if (_action == "管理補考成績")
            {
                btnSave.Enabled = true;
                btnClose.Enabled = true;

                // 補考成績輸入的 功能打開
                dataGridViewX1.Columns[11].ReadOnly = false;

                // 管理補考成績 才看的到此文字
                labelInputScoreHint.Visible = true;

                picLoading.Visible = false;
            }
            else
            {


                // 資料 載完後 才可以 讓使用者編輯畫面
                cboTeacher.Enabled = true;

                // 未分群組 不給調整 群組名稱。
                if (txtGroupName.Text != "未分群組")
                {
                    txtGroupName.Enabled = true;
                }
                
                txtDescription.Enabled = true;
                btnSave.Enabled = true;
                btnClose.Enabled = true;

                picLoading.Visible = false;
            }

        }

        private void DataWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }


        // 關閉
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 儲存
        private void btnSave_Click(object sender, EventArgs e)
        {

            // 管理補考成績 的專門驗證
            if (_action == "管理補考成績")
            {




            }
            else
            {
                if (txtGroupName.Text == "")
                {
                    FISCA.Presentation.Controls.MsgBox.Show("補考群組名稱必須輸入。");
                    return;
                }


                if (_groupNameList.Contains(txtGroupName.Text))
                {
                    FISCA.Presentation.Controls.MsgBox.Show("本補考梯次:『 " + _makeup_batch + "』已有相同補考群組名稱。");
                    return;
                }

            }


            // LOG 資訊
            string _actor = DSAServices.UserAccount; ;
            string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            //拚SQL
            // 兜資料
            List<string> dataList = new List<string>();

            string sql = "";

            if (_action == "新增群組")
            {
                string logDetail = @" 高中補考 學年度「" + _school_year +
                    @"」，學期「" + _semester + @"」， 補考梯次「 " + _batch.MakeUp_Batch + @"」， 
                    新增補考群組「 " + txtGroupName.Text + "」，閱卷老師「 " + cboTeacher.Text + "」，群組說明「 " + txtDescription.Text + "」，補考日期「 " + txtDate.Text + "」，補考時間「 " + txtTime.Text + "」，補考地點「 " + txtPlace.Text + "」";


                string ref_teacher_id = _teacherList.Find(t => (t.Name + "(" + t.Nickname + ")" == cboTeacher.Text)).ID;

                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_group
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description                                    
                    ,'{4}'::TEXT AS makeup_date        
                    ,'{5}'::TEXT AS makeup_time        
                    ,'{6}'::TEXT AS makeup_place        
                    ,'{7}'::TEXT AS log_detail                    
                    ,'{8}'::TEXT AS ref_makeup_batch_id
                    ,'{9}'::TEXT AS ref_teacher_id
                ", txtGroupName.Text, _school_year, _semester, txtDescription.Text, txtDate.Text,txtTime.Text,txtPlace.Text, logDetail, _batch.UID, ref_teacher_id);

                dataList.Add(data);

                string dataString = string.Join(" UNION ALL", dataList);

                sql = string.Format(@"
WITH data_row AS(			 
                {0}     
)
,insert_data AS (
    INSERT INTO $make.up.group(
        makeup_group
        ,description
        ,makeup_date
        ,makeup_time
        ,makeup_place
        ,ref_makeup_batch_id
        ,ref_teacher_id
    )
    SELECT
        data_row.makeup_group
        ,data_row.description
        ,data_row.makeup_date
        ,data_row.makeup_time
        ,data_row.makeup_place
        ,data_row.ref_makeup_batch_id
        ,data_row.ref_teacher_id
    FROM data_row         
)
INSERT INTO log(
	actor
	, action_type
	, action
	, target_category
	, target_id
	, server_time
	, client_info
	, action_by
	, description
)
SELECT 
	'{1}'::TEXT AS actor
	, 'Record' AS action_type
	, '新增高中補考群組' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{2}' AS client_info
	, '新增高中補考群組'AS action_by   
	,data_row.log_detail  AS description 
FROM
	data_row

", dataString, _actor, _client_info);


            }
            else if (_action == "修改群組")
            {
                string logDetail = @" 高中補考 學年度「" + _school_year +
                    @"」，學期「" + _semester + @"」， 補考梯次「 " + _makeup_batch + @"」， 
                    補考群組「 " + _group.MakeUp_Group + "」，";

                string ref_teacher_id = _teacherList.Find(t => (t.Name + "(" + t.Nickname + ")" == cboTeacher.Text))!=null ? _teacherList.Find(t => (t.Name + "(" + t.Nickname + ")" == cboTeacher.Text)).ID :""; 

                K12.Data.TeacherRecord oldTeacher = _teacherList.Find(t => _group.Ref_Teacher_ID == t.ID);

                string old_teacher_name = oldTeacher != null ? oldTeacher.Name + "(" + oldTeacher.Nickname + ")" : "";

                if (txtGroupName.Text != _group.MakeUp_Group)
                {
                    logDetail += "名稱由「 " + _group.MakeUp_Group + "」更改為 「 " + txtGroupName.Text + "」 ";
                }

                if (ref_teacher_id != _group.Ref_Teacher_ID)
                {

                    logDetail += "閱卷教師由「 " + old_teacher_name + "」更改為 「 " + cboTeacher.Text + "」 ";
                }


                if (txtDescription.Text != _group.Description)
                {

                    logDetail += "群組說明由「 " + _group.Description + "」更改為 「 " + txtDescription.Text + "」 ";
                }

                if (txtDate.Text != _group.MakeUp_Date)
                {

                    logDetail += "補考日期由「 " + _group.MakeUp_Date + "」更改為 「 " + txtDate.Text + "」 ";
                }

                if (txtTime.Text != _group.MakeUp_Time)
                {

                    logDetail += "補考時間由「 " + _group.MakeUp_Time + "」更改為 「 " + txtTime.Text + "」 ";
                }

                if (txtPlace.Text != _group.MakeUp_Place)
                {

                    logDetail += "補考地點由「 " + _group.MakeUp_Place + "」更改為 「 " + txtPlace.Text + "」 ";
                }




                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_group
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description
                    ,'{4}'::TEXT AS makeup_date
                    ,'{5}'::TEXT AS makeup_time
                    ,'{6}'::TEXT AS makeup_place
                    ,'{7}'::TEXT AS log_detail
                    ,'{8}'::BIGINT AS uid
                    ,'{9}'::TEXT AS ref_teacher_id
                ", txtGroupName.Text, _school_year, _semester, txtDescription.Text, txtDate.Text, txtTime.Text, txtPlace.Text, logDetail, _group.UID, ref_teacher_id);

                dataList.Add(data);

                string dataString = string.Join(" UNION ALL", dataList);

                sql = string.Format(@"
WITH data_row AS(			 
                {0}     
)
,update_data AS (
    Update $make.up.group
    SET
        makeup_group = data_row.makeup_group
        ,description = data_row.description        
        ,makeup_date = data_row.makeup_date
        ,makeup_time = data_row.makeup_time
        ,makeup_place = data_row.makeup_place
        ,ref_teacher_id = data_row.ref_teacher_id    
    FROM data_row     
    WHERE $make.up.group.uid = data_row.uid
)
INSERT INTO log(
	actor
	, action_type
	, action
	, target_category
	, target_id
	, server_time
	, client_info
	, action_by
	, description
)
SELECT 
	'{1}'::TEXT AS actor
	, 'Record' AS action_type
	, '高中補考群組更新' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{2}' AS client_info
	, '高中補考群組更新'AS action_by   
	,data_row.log_detail  AS description 
FROM
	data_row

", dataString, _actor, _client_info);
            }
            else if (_action == "管理補考成績")
            {

                foreach (UDT_MakeUpData input_data in _dataList)
                {
                    // 假如分數沒有改變 則不更新
                    if (!input_data.HasNewMakeUpScore)
                    {
                        continue;
                    }

                    string logDetail = @" 高中補考 學年度「" + _group.MakeUpBatch.School_Year +
                  @"」，學期「" + _group.MakeUpBatch.Semester + @"」， 補考梯次「 " + _group.MakeUpBatch.MakeUp_Batch + @"」， 補考群組「 " + _group.MakeUp_Group + @"」
                    補考資料 學生「 " + input_data.StudentName + "」，科別「 " + input_data.Department + "」，班級「 " + input_data.ClassName + "」，座號「 " + input_data.Seat_no + @"」，
，科目「 " + input_data.Subject + "」，級別「 " + input_data.Level + "」，學分「 " + input_data.Credit + "」，校部定「 " + input_data.C_Is_Required_By + "」，必選修「 " + input_data.C_Is_Required + @"」，
，成績分數「 " + input_data.Score + "」，及格標準「 " + input_data.Pass_Standard + "」，補考標準「 " + input_data.MakeUp_Standard + @"」
。補考分數 自「 " + input_data.MakeUp_Score + "」，更改為 「" + input_data.New_MakeUp_Score + "」。";


                    string data = string.Format(@"
                SELECT                       
                    '{0}'::BIGINT AS uid
                    ,'{1}'::TEXT AS makeup_score
                    ,'{2}'::TEXT AS log_detail
                ", input_data.UID, input_data.New_MakeUp_Score, logDetail);

                    dataList.Add(data);

                }

                string dataString = string.Join(" UNION ALL", dataList);

                sql = string.Format(@"
WITH data_row AS(			 
                {0}     
)
,update_data AS (
    Update $make.up.data
    SET
        makeup_score = data_row.makeup_score 
    FROM data_row     
    WHERE $make.up.data.uid = data_row.uid
)
INSERT INTO log(
	actor
	, action_type
	, action
	, target_category
	, target_id
	, server_time
	, client_info
	, action_by
	, description
)
SELECT 
	'{1}'::TEXT AS actor
	, 'Record' AS action_type
	, '高中補考補考分數輸入' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{2}' AS client_info
	, '高中補考補考分數輸入'AS action_by   
	,data_row.log_detail  AS description 
FROM
	data_row

", dataString, _actor, _client_info);




            }

            if (dataList.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("未修正任何補考成績。");

                // 儲存後關閉
                this.Close();
            }
            else
            {
                K12.Data.UpdateHelper uh = new UpdateHelper();

                //執行sql
                uh.Execute(sql);


                FISCA.Presentation.Controls.MsgBox.Show("儲存成功。");

                // 儲存後關閉
                this.Close();
            }
        }

        // 將補考資料 移至其他群組
        private void btnSwap_Click(object sender, EventArgs e)
        {
            int selectRows = 0;

            // 將舊資料清除
            _selected_dataList.Clear();

            // 計算 所有被選取的 補考資料 項目數
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.Selected)
                {
                    selectRows++;

                    UDT_MakeUpData data = _dataList.Find(d => d.UID == "" + row.Tag);

                    if (data != null)
                    {
                        _selected_dataList.Add(data);
                    }
                }

            }

            if (selectRows < 1)
            {
                FISCA.Presentation.Controls.MsgBox.Show("轉移群組功能需選擇 大於1個資料。");
                return;
            }


            // 傳進目前 的 補考群組、 選擇 欲轉其他群組的補考資料
            SwapMakeUpGroupForm smuf = new SwapMakeUpGroupForm(_group, _selected_dataList);

            if (DialogResult.OK == smuf.ShowDialog())
            {
                _dataWorker.RunWorkerAsync();

            }


        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet ws = book.Worksheets[book.Worksheets.Add()];
            ws.Name = _group.MakeUp_Group;

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
            sd.FileName = "高中補考名單資料匯出";
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

        private int GetDecimalNumber(string studentID )
        {
            // 預設小數位數為2
            int DecimalNumber = 2;

            XmlElement scoreCalcRule = SmartSchool.Evaluation.ScoreCalcRule.ScoreCalcRule.Instance.GetStudentScoreCalcRuleInfo(studentID) == null ? null : SmartSchool.Evaluation.ScoreCalcRule.ScoreCalcRule.Instance.GetStudentScoreCalcRuleInfo(studentID).ScoreCalcRuleElement;

            if (scoreCalcRule != null)
            {
                DSXmlHelper helper = new DSXmlHelper(scoreCalcRule);

                // 抓取該學生 成績計算規則 的 科目成績計算位數 
                foreach (XmlElement element in helper.GetElements("各項成績計算位數/科目成績計算位數"))
                {
                    DecimalNumber = int.Parse( element.GetAttribute("位數"));
                }

            }
                return DecimalNumber;
        }

        private void dataGridViewX1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // 只驗 分數
            if (e.ColumnIndex != 11)
            {
                return;
            }

            DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            // 找到對應的成績資料 以便對出 小數位數計算設定
            UDT_MakeUpData inputData = _dataList.Find(data => data.UID == "" + cell.OwningRow.Tag);

            cell.ErrorText = String.Empty;

            decimal d = 0;

            if (!decimal.TryParse("" + e.FormattedValue, out d))
            {
                if ("" + e.FormattedValue != "缺" && "" + e.FormattedValue != "")
                {
                    cell.ErrorText = "缺考請輸入『缺』，本欄無法輸入其他文字。";

                    inputData.HasNewMakeUpScore = false;
                }                
                if ("" + e.FormattedValue == "缺" )
                {
                    cell.ErrorText = String.Empty;

                    inputData.New_MakeUp_Score = "缺";

                    inputData.HasNewMakeUpScore = true;
                }

                // 原本有值 填成空白 也要更新
                if ("" + e.FormattedValue == "" && inputData.MakeUp_Score !="")
                {
                    cell.ErrorText = String.Empty;

                    inputData.New_MakeUp_Score = "";

                    inputData.HasNewMakeUpScore = true;
                }
            }
            else
            {
                int index = (""+ e.FormattedValue).IndexOf('.');
                // 輸入整數 (Ex:66， 因為沒有 「.」，所以會回傳-1)
                if (index == -1)
                {
                    // 輸入整數，就不必管任何驗證了。

                    inputData.New_MakeUp_Score = "" + e.FormattedValue;

                    inputData.HasNewMakeUpScore = true;

                }
                else
                {
                    decimal dn = decimal.Parse(inputData.DecimalNumber);

                    if (("" + e.FormattedValue).Substring(index + 1).Length > dn)
                    {
                        cell.ErrorText = "補考成績小數位數輸入超過計算規則設定值『" + inputData.DecimalNumber + "』。";

                        inputData.HasNewMakeUpScore = false;
                    }
                    else
                    {
                        // 驗過了 改變值
                        inputData.New_MakeUp_Score = "" + e.FormattedValue;

                        inputData.HasNewMakeUpScore = true;
                    }

                }                                
            }
        }

        private void dataGridViewX1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                //只檢查分數欄，有錯誤就不給存
                DataGridViewCell cell = dataGridViewX1.Rows[row.Index].Cells[11];

                if (cell.ErrorText != String.Empty)
                {
                    btnSave.Enabled = false;
                    return; // 有一個錯誤，就不給存，跳出檢查迴圈。
                }
                else
                {
                    btnSave.Enabled = true;
                }

            }

        }
    }
}
