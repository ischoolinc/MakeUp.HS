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
    public partial class SwapMakeUpGroupForm : FISCA.Presentation.Controls.BaseForm
    {

        private UDT_MakeUpGroup _oldGroup;

        // 目前補考資料的 補考梯次ID
        private string _makeup_batch_id;

        // 要更動補考群組的 補考資料 名單
        private List<UDT_MakeUpData> _makeup_data_list;

        // 補考群組字典 <群組名稱，群組UID>
        private Dictionary<string,string> _makeupDict ;

        // 抓補考群組用　BackgroundWorker
        private BackgroundWorker _groupWorker;

        public SwapMakeUpGroupForm(UDT_MakeUpGroup oldGroup, List<UDT_MakeUpData> makeup_data_list)
        {
            _oldGroup = oldGroup;

            _makeup_data_list = makeup_data_list;

            _makeupDict = new Dictionary<string, string>();

            InitializeComponent();

            _groupWorker = new BackgroundWorker();
            _groupWorker.DoWork += new DoWorkEventHandler(GroupWorker_DoWork);
            _groupWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GroupWorker_RunWorkerCompleted);
            _groupWorker.ProgressChanged += new ProgressChangedEventHandler(GroupWorker_ProgressChanged);
            _groupWorker.WorkerReportsProgress = true;

            _groupWorker.RunWorkerAsync();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboMakeupGroup.Text == "")
            {
                FISCA.Presentation.Controls.MsgBox.Show("請選擇群組名稱。");

                return;
            }


            if (DialogResult.OK == FISCA.Presentation.Controls.MsgBox.Show("確定要將選取的補考資料，自『" + _oldGroup.MakeUp_Group + "』轉移到『" + cboMakeupGroup.Text + "』嗎?", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                // LOG 資訊
                string _actor = DSAServices.UserAccount; ;
                string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

                //拚SQL
                // 兜資料
                List<string> dataList = new List<string>();

                // 指定新群組的
                string new_ref_makeup_group_id = _makeupDict[cboMakeupGroup.Text];

                string sql = "";
               
                foreach (UDT_MakeUpData makeupdata in _makeup_data_list)
                {

                    string logDetail = @" 高中補考 學年度「" + _oldGroup.MakeUpBatch.School_Year +
                  @"」，學期「" + _oldGroup.MakeUpBatch.Semester + @"」， 補考梯次「 " + _oldGroup.MakeUpBatch.MakeUp_Batch + @"」， 
                    補考資料 學生「 " + makeupdata.StudentName + "」，班級「 " + makeupdata.ClassName + "」，座號「 " + makeupdata.Seat_no + @"」，
，科目「 " + makeupdata.Subject + "」，級別「 " + makeupdata.Level + "」，學分「 " + makeupdata.Credit + "」，校部定「 " + makeupdata.C_Is_Required_By + "」，必選修「 " + makeupdata.C_Is_Required + @"」，
，成績分數「 " + makeupdata.Score + "」，補考分數「 " + makeupdata.MakeUp_Score + "」，及格標準「 " + makeupdata.Pass_Standard + "」，補考標準「 " + makeupdata.MakeUp_Standard + @"」
。補考群組自「 " + _oldGroup.MakeUp_Group + "」更改為 「"+ cboMakeupGroup.Text + "」。";

                    
                    string data = string.Format(@"
                SELECT                       
                    '{0}'::BIGINT AS uid
                    ,'{1}'::TEXT AS ref_makeup_group_id
                    ,'{2}'::TEXT AS log_detail
                ", makeupdata.UID, new_ref_makeup_group_id,logDetail);

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
        ref_makeup_group_id = data_row.ref_makeup_group_id 
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
	, '高中補考資料更換補考群組' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{2}' AS client_info
	, '高中補考資料更換補考群組'AS action_by   
	,data_row.log_detail  AS description 
FROM
	data_row

", dataString, _actor, _client_info);

                K12.Data.UpdateHelper uh = new UpdateHelper();

                //執行sql
                uh.Execute(sql);
            }

            
            FISCA.Presentation.Controls.MsgBox.Show("儲存成功。");

            // 傳回結果 OK。
            this.DialogResult = DialogResult.OK;

            this.Close();
        }


        private void GroupWorker_DoWork(object sender, DoWorkEventArgs e)
        {


            #region 取得本學期得其他補考群組資料 
            string query_make_up_group = @"
SELECT 
    $make.up.group.uid
    ,$make.up.group.makeup_group
FROM $make.up.group
WHERE  $make.up.group.ref_makeup_batch_id ::BIGINT = " + _oldGroup.Ref_MakeUp_Batch_ID + " AND $make.up.group.uid !=" + _oldGroup.UID;


            QueryHelper qh_make_up_group = new QueryHelper();
            DataTable dt_make_up_group = qh_make_up_group.Select(query_make_up_group);

            if (dt_make_up_group.Rows.Count > 0)
            {
                foreach (DataRow row in dt_make_up_group.Rows)
                {
                    
                    if (!_makeupDict.Keys.Contains("" + row["makeup_group"]))
                    {
                        _makeupDict.Add("" + row["makeup_group"], "" + row["uid"]);
                    }
                }
            }
            #endregion


        }

        private void GroupWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            // 將 Cbo 加入 其他 同梯次的 補考群組
            foreach (string makeup_group in _makeupDict.Keys)
            {
                // 老師全名 
                cboMakeupGroup.Items.Add(makeup_group);
            }

            cboMakeupGroup.SelectedIndex = 0;

        }


        private void GroupWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }
    }
}
