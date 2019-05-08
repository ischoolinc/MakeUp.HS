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

namespace MakeUp.HS.Form
{
    public partial class InsertUpdateMakeUpGroupForm : FISCA.Presentation.Controls.BaseForm
    {
        // 開啟這個視窗的動作別(分為:新增、修改)
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

            _groupNameList = new List<string>();

            _teacherList = K12.Data.Teacher.SelectAll();

            //將教師加入清單
            foreach (K12.Data.TeacherRecord teacher in _teacherList)
            {
                // 老師全名 
                cboTeacher.Items.Add(teacher.Name + "(" + teacher.Nickname + ")");                              
            }

            K12.Data.TeacherRecord groupTeacher = _teacherList.Find(t => t.ID == group.Ref_Teacher_ID);

            // 預設為群組的閱卷老師
            cboTeacher.Text = groupTeacher != null ? groupTeacher.Name +"(" + groupTeacher.Nickname + ")" : "" ;

            _dataWorker = new BackgroundWorker();
            _dataWorker.DoWork += new DoWorkEventHandler(DataWorker_DoWork);
            _dataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DataWorker_RunWorkerCompleted);
            _dataWorker.ProgressChanged += new ProgressChangedEventHandler(DataWorker_ProgressChanged);
            _dataWorker.WorkerReportsProgress = true;

            _action = action;

            _group = group;

            // 只有新增 模式 才可以讓使用者 編輯 
            if (_action == "新增")
            {

            }

            // 修改模式 使用者只能修改 
            if (_action == "修改")
            {
                this.Text = "管理補考群組";
                txtGroupName.Text = _group.MakeUp_Group;
                txtDescription.Text = _group.Description;

                txtGroupName.Enabled = false;
                txtDescription.Enabled = false;
                cboTeacher.Enabled = false;
                btnSave.Enabled = false;
                btnClose.Enabled = false;

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

            dataGridViewX1.Rows.Clear();

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
    ,student.name AS student_name
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


                    _dataList.Add(data);
                }
            }
            #endregion

        }

        private void DataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 填寫補考資料
            foreach (UDT_MakeUpData data in _dataList)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                // row 的 Tag 為 補考資料的系統編號
                row.Tag = data.UID;

                row.Cells[0].Value = data.StudentName;

                row.Cells[1].Value = data.ClassName;

                row.Cells[2].Value = data.Seat_no;

                row.Cells[3].Value = data.StudentNumber;

                row.Cells[4].Value = data.Subject;

                row.Cells[5].Value = data.Level;

                row.Cells[6].Value = data.Credit;

                row.Cells[7].Value = data.C_Is_Required_By;

                row.Cells[8].Value = data.C_Is_Required;

                row.Cells[9].Value = data.Score;

                row.Cells[10].Value = data.MakeUp_Score;

                row.Cells[11].Value = data.Pass_Standard;

                row.Cells[12].Value = data.MakeUp_Standard;

                dataGridViewX1.Rows.Add(row);
            }

            // 資料 載完後 才可以 讓使用者編輯畫面
            cboTeacher.Enabled = true;
            txtGroupName.Enabled = true;
            txtDescription.Enabled = true;
            btnSave.Enabled = true;
            btnClose.Enabled = true;

            picLoading.Visible = false;
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




            // LOG 資訊
            string _actor = DSAServices.UserAccount; ;
            string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            //拚SQL
            // 兜資料
            List<string> dataList = new List<string>();

            string sql = "";

            if (_action == "新增")
            {
                string logDetail = @" 高中補考 學年度「" + _school_year +
                    @"」，學期「" + _semester + @"」， 補考梯次「 " + _batch.MakeUp_Batch + @"」， 
                    新增補考群組「 " + txtGroupName.Text + "」，閱卷老師「 " + cboTeacher.Text + "」，群組說明「 " + txtDescription.Text + "」";


                string ref_teacher_id = _teacherList.Find(t => (t.Name + "(" + t.Nickname + ")" == cboTeacher.Text)).ID;

                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_group
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description                                    
                    ,'{4}'::TEXT AS log_detail                    
                    ,'{5}'::TEXT AS ref_makeup_batch_id
                    ,'{6}'::TEXT AS ref_teacher_id
                ", txtGroupName.Text, _school_year, _semester, txtDescription.Text, logDetail, _batch.UID, ref_teacher_id);

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
        ,ref_makeup_batch_id
        ,ref_teacher_id
    )
    SELECT
        data_row.makeup_group
        ,data_row.description
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
            else
            {
                string logDetail = @" 高中補考 學年度「" + _school_year +
                    @"」，學期「" + _semester + @"」， 補考梯次「 " + _makeup_batch + @"」， 
                    補考群組「 " + _group.MakeUp_Group + "」，";

                string ref_teacher_id = _teacherList.Find(t => (t.Name + "(" + t.Nickname + ")" == cboTeacher.Text)).ID;

                K12.Data.TeacherRecord oldTeacher = _teacherList.Find(t => _group.Ref_Teacher_ID == t.ID);

                string old_teacher_name = oldTeacher!= null ?  oldTeacher.Name + "(" + oldTeacher.Nickname + ")" : "";

                if (txtGroupName.Text != _group.MakeUp_Group)
                {
                    logDetail +=  "名稱由「 " + _group.MakeUp_Group + "」更改為 「 " + txtGroupName.Text + "」 ";
                }

                if (ref_teacher_id != _group.Ref_Teacher_ID)
                {

                    logDetail += "閱卷教師由「 " + old_teacher_name + "」更改為 「 " + cboTeacher.Text + "」 ";
                }


                if (txtDescription.Text != _group.Description)
                {

                    logDetail += "群組說明由「 " + _group.Description + "」更改為 「 " + txtDescription.Text + "」 ";
                }

                


                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_group
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description                                    
                    ,'{4}'::TEXT AS log_detail
                    ,'{5}'::BIGINT AS uid
                    ,'{6}'::BIGINT AS ref_teacher_id
                ", txtGroupName.Text, _school_year, _semester, txtDescription.Text, logDetail, _group.UID, ref_teacher_id);

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

            K12.Data.UpdateHelper uh = new UpdateHelper();

            //執行sql
            uh.Execute(sql);

          
            FISCA.Presentation.Controls.MsgBox.Show("儲存成功。");

            // 儲存後關閉
            this.Close();
        }


    }
}
