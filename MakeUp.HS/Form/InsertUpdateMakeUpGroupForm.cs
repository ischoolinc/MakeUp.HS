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

        // 補考梯次(修改使用)
        private UDT_MakeUpGroup _group;

        // 本學期 已經有的補考群組名稱(驗證重覆用)
        private List<string> _groupNameList;

        private List<UDT_MakeUpData> _dataList;


        public InsertUpdateMakeUpGroupForm(string action,  UDT_MakeUpGroup group)
        {
            InitializeComponent();

            _dataList = new List<UDT_MakeUpData>();

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
                btnSave.Enabled = false;
                btnClose.Enabled = false;

                picLoading.Visible = true;


                _dataWorker.RunWorkerAsync();
            }

        }

        private void InsertUpdateMakeUpBatchForm_Load(object sender, EventArgs e)
        {





            picLoading.Visible = false;
        }



        private void DataWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            dataGridViewX1.Rows.Clear();

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
    $make.up.data.Ref_MakeUp_Group_ID = '" + _group.UID + "'" ;


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

            foreach (UDT_MakeUpData data in _dataList)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                // row 的 Tag 為 補考梯次的系統編號
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
                FISCA.Presentation.Controls.MsgBox.Show("補考梯次名稱必須輸入。");
                return;
            }


            //if (txtBatchName.Text == "")
            //{
            //    FISCA.Presentation.Controls.MsgBox.Show("補考梯次名稱不得與本學期補考梯次名稱重覆。");
            //    return;
            //}

            
            

            // LOG 資訊
            string _actor = DSAServices.UserAccount; ;
            string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            //拚SQL
            // 兜資料
            List<string> dataList = new List<string>();

            string sql ="";

            if (_action == "新增")
            {
               
            }
            else
            {
                
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
