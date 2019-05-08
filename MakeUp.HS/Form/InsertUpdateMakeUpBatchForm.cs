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
    public partial class InsertUpdateMakeUpBatchForm : FISCA.Presentation.Controls.BaseForm
    {
        // 開啟這個視窗的動作別(分為:新增、修改)
        private string _action;

        // 學年度
        private string _schoolYear;

        // 學期
        private string _semester;

        // 補考梯次(修改使用)
        private UDT_MakeUpBatch _batch;

        // 本學期 已經有的補考梯次名稱(驗證重覆用)
        private List<string> _batchNameList;

        public InsertUpdateMakeUpBatchForm(string action, string schoolYear, string semester)
        {
            InitializeComponent();

            _action = action;

            _schoolYear = schoolYear;

            _semester = semester;

            // 只有新增 模式 才可以讓使用者 編輯 補考梯次名稱、包含班級
            if (_action == "新增")
            {
                txtBatchName.Enabled = true;
                lstClass.Enabled = true;
                chkSelectAll.Enabled = true;
            }

            // 修改模式 使用者只能修改 補考說明
            if (_action == "修改")
            {


            }

        }

        public InsertUpdateMakeUpBatchForm(string action, string schoolYear, string semester, UDT_MakeUpBatch batch)
        {
            InitializeComponent();

            _action = action;

            _schoolYear = schoolYear;

            _semester = semester;

            _batch = batch;

            // 只有新增 模式 才可以讓使用者 編輯 補考梯次名稱、包含班級
            if (_action == "新增")
            {
                txtBatchName.Enabled = true;
                lstClass.Enabled = true;
                chkSelectAll.Enabled = true;
            }

            // 修改模式 使用者只能修改 補考說明
            if (_action == "修改")
            {
                this.Text = "管理補考梯次";
                txtBatchName.Text = _batch.MakeUp_Batch;
                txtDescription.Text = _batch.Description;
            }

        }

        private void InsertUpdateMakeUpBatchForm_Load(object sender, EventArgs e)
        {

            // 加入班級清單
            List<K12.Data.ClassRecord> classList = K12.Data.Class.SelectAll();

            // 以班級名稱排序
            classList.Sort((x, y) => { return x.Name.CompareTo(y.Name); });

            foreach (K12.Data.ClassRecord classRecord in classList)
            {
                ListViewItem vItem = new ListViewItem();
                vItem.Name = classRecord.Name;
                vItem.Tag = classRecord.ID;
                vItem.Text = classRecord.Name;
                vItem.Checked = true;
                lstClass.Items.Add(vItem);
            }

            // 修改模式 將 原本選得班級勾起
            if (_action == "修改")
            {              
                selectCheckedClass(_batch.Included_Class_ID);               
            }

            picLoading.Visible = false;
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem Item in lstClass.Items)
            {
                Item.Checked = chkSelectAll.Checked;
            }
        }

        // 關閉
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 儲存
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtBatchName.Text == "")
            {
                FISCA.Presentation.Controls.MsgBox.Show("補考梯次名稱必須輸入。");
                return;
            }


            //if (txtBatchName.Text == "")
            //{
            //    FISCA.Presentation.Controls.MsgBox.Show("補考梯次名稱不得與本學期補考梯次名稱重覆。");
            //    return;
            //}

            bool atLeastOneClass = false;

            foreach (ListViewItem Item in lstClass.Items)
            {
                if (Item.Checked)
                {
                    atLeastOneClass = true;
                }
            }

            if (!atLeastOneClass)
            {
                FISCA.Presentation.Controls.MsgBox.Show("補考梯次包含班級請至少選擇一班。");
                return;
            }

            // LOG 資訊
            string _actor = DSAServices.UserAccount; ;
            string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            //拚SQL
            // 兜資料
            List<string> dataList = new List<string>();

            string sql;

            if (_action == "新增")
            {
                string included_class_id_xml = "";

                XmlDocument doc = new XmlDocument();

                XmlElement root = doc.DocumentElement;

                //string.Empty makes cleaner code
                XmlElement element_root = doc.CreateElement(string.Empty, "Root", string.Empty);

                doc.AppendChild(element_root);

                foreach (ListViewItem Item in lstClass.Items)
                {
                    if (Item.Checked)
                    {
                        XmlElement element_classID = doc.CreateElement(string.Empty, "ClassID", string.Empty);
                        element_classID.SetAttribute("ClassName", "" + Item.Text);
                        element_classID.InnerText = "" + Item.Tag;
                        element_root.AppendChild(element_classID);
                    }
                }

                included_class_id_xml = doc.OuterXml;

                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_batch
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description
                    ,'{4}'::TEXT AS included_class_id                    
                ", txtBatchName.Text, _schoolYear, _semester, txtDescription.Text, included_class_id_xml);

                dataList.Add(data);

                string dataString = string.Join(" UNION ALL", dataList);



                sql = string.Format(@"
WITH data_row AS(			 
                {0}     
)
,insert_data AS (
INSERT INTO $make.up.batch(
	makeup_batch	
	,school_year
    ,semester
	,description
	,included_class_id
)
SELECT 
	data_row.makeup_batch::TEXT AS makeup_batch	
	,data_row.school_year::TEXT AS school_year	
    ,data_row.semester::TEXT AS semester	
	,data_row.description::TEXT AS description	
	,data_row.included_class_id::TEXT AS included_class_id		
FROM
	data_row
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
	, '高中補考梯次新增' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{2}' AS client_info
	, '高中補考梯次新增'AS action_by   
	, ' 新增高中 學年度「'|| data_row.school_year||'」，學期「'|| data_row.semester||'」， 補考梯次「'|| data_row.makeup_batch||'」，補考說明「'|| data_row.description ||'」，包含班級ID「'|| data_row.included_class_id || '」。' AS description 
FROM
	data_row

", dataString, _actor, _client_info);

            }
            else
            {
                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_batch
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description
                    ,'{4}'::TEXT AS included_class_id                    
                    ,'{5}'::TEXT AS old_description
                    ,'{6}'::BIGINT AS uid
                ", txtBatchName.Text, _schoolYear, _semester, txtDescription.Text, _batch.Included_Class_ID, _batch.Description,_batch.UID);

                dataList.Add(data);

                string dataString = string.Join(" UNION ALL", dataList);

                sql = string.Format(@"
WITH data_row AS(			 
                {0}     
)
,update_data AS (
    Update $make.up.batch
    SET
        description = data_row.description        
    FROM data_row     
    WHERE $make.up.batch.uid = data_row.uid
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
	, '高中補考梯次更新補考說明' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{2}' AS client_info
	, '高中補考梯次更新'AS action_by   
	, ' 高中補考 學年度「'|| data_row.school_year||'」，學期「'|| data_row.semester||'」， 補考梯次「'|| data_row.makeup_batch||'」，補考說明 由「'|| data_row.old_description ||'」 更改為「'|| data_row.description ||'」。' AS description 
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

        // 將傳入的 梯次 的包含班級勾起來
        private void selectCheckedClass(string xml)
        {
            // 先全不選
            chkSelectAll.Checked = false;

            List<string> classIDList = new List<string>();

            XElement elmRoot = XElement.Parse(xml);

            foreach (XElement ele_class in elmRoot.Elements("ClassID"))
            {
                string classID= ele_class.Value;

                classIDList.Add(classID);
            }

            // 在清單內就勾起來
            foreach (ListViewItem Item in lstClass.Items)
            {
                if (classIDList.Contains("" + Item.Tag))
                {
                    Item.Checked = true;
                }                 
            }
        } 
    }
}
