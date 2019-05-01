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
    public partial class MakeUpBatchManagerForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<string> _BatchIDList;

        private List<UDT_MakeUpBatch> _BatchList;

        private BackgroundWorker _worker;

        // 學年度
        private string _schoolYear;

        // 學期
        private string _semester;

        // 目前選的梯次
        private UDT_MakeUpBatch _selectedBatch;


        public MakeUpBatchManagerForm()
        {
            InitializeComponent();

            _BatchList = new List<UDT_MakeUpBatch>();

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
            
        }

        private void GetMakeUpBatch()
        {
            _BatchList.Clear();

            #region 取得補考梯次 

            string query = @"
SELECT 
* 
FROM $make.up.batch
WHERE
school_year = '" + _schoolYear + "'" +
"AND semester = '" + _semester + "'";

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            //整理目前的補考梯次 資料
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    UDT_MakeUpBatch batch = new UDT_MakeUpBatch();

                    batch.UID = "" + row["uid"];

                    //補考梯次
                    batch.MakeUp_Batch = "" + row["makeup_batch"];

                    //學年度
                    batch.School_Year = "" + row["school_year"];

                    //學期
                    batch.Semester = "" + row["semester"];

                    //補考說明
                    batch.Description = "" + row["description"];

                    //包含班級id
                    batch.Included_Class_ID = "" + row["included_class_id"];

                    _BatchList.Add(batch);
                }
            }
            #endregion
        }

        /// <summary>
        /// 更新ListView
        /// </summary>
        private void RefreshListView()
        {
            picLoading.Visible = true;

            _schoolYear = cboSchoolYear.Text;
            _semester = cbosemester.Text;

            // 暫停畫面控制項

            cboSchoolYear.SuspendLayout();
            cbosemester.SuspendLayout();
            dataGridViewX1.SuspendLayout();

            _worker.RunWorkerAsync();
        }


        /// <summary>
        /// 將補考梯次填入 DataGridView
        /// </summary>
        private void FillMakeUpBatch()
        {

            dataGridViewX1.SuspendLayout();
            dataGridViewX1.Rows.Clear();
            //dataGridViewX1.Rows.AddRange(list.ToArray());
            dataGridViewX1.ResumeLayout();
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
        /// 按下「重新整理」時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {

            RefreshListView();
        }


        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetMakeUpBatch();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 清除舊項目
            dataGridViewX1.Rows.Clear();

            foreach (UDT_MakeUpBatch batchRecord in _BatchList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                // row 的 Tag 為 補考梯次的系統編號
                row.Tag = batchRecord.UID;

                row.Cells[0].Value = batchRecord.MakeUp_Batch;

                // 解析班級XML className
                batchRecord.ParseClassXMLNameString();

                row.Cells[1].Value = batchRecord.totalclassName;

                row.Cells[2].Value = batchRecord.Description;

                dataGridViewX1.Rows.Add(row);
            }


            // 繼續 畫面控制項       
            picLoading.Visible = false;

            cboSchoolYear.ResumeLayout();
            cbosemester.ResumeLayout();

            dataGridViewX1.ResumeLayout();

            FISCA.Presentation.MotherForm.SetStatusBarMessage("取得補考梯次完成");

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

            //  找到選取到的 梯次
            _selectedBatch = _BatchList.Find(x => x.UID == "" + cell.OwningRow.Tag);

            // 修改模式
            InsertUpdateMakeUpBatchForm iumbf = new InsertUpdateMakeUpBatchForm("修改", cboSchoolYear.Text, cbosemester.Text, _selectedBatch);
            iumbf.ShowDialog();

            RefreshListView(); //重整畫面
        }

        private void MakeUpBatchManagerForm_Load(object sender, EventArgs e)
        {

        }


        private void cbosemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSchoolYear.Text == "")
            {
                return;
            }



            RefreshListView();
        }

        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbosemester.Text == "")
            {
                return;
            }

            RefreshListView();
        }

        private void btnInsertBatch_Click(object sender, EventArgs e)
        {
            // 新增模式
            InsertUpdateMakeUpBatchForm iumbf = new InsertUpdateMakeUpBatchForm("新增", cboSchoolYear.Text, cbosemester.Text);
            iumbf.ShowDialog();

            RefreshListView();
        }

        private void btnGenMakeUpGroup_Click(object sender, EventArgs e)
        {
            // 傳入目前選的梯次 來產生群組
            GenMakeUpGroupForm gmugf = new GenMakeUpGroupForm(_selectedBatch);

            gmugf.ShowDialog();
        }



        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            // 有選擇梯次 才可以產生補考群組
            if (dataGridViewX1.SelectedRows.Count > 0)
            {
                btnGenMakeUpGroup.Enabled = true;

                // 一次只能選一條 Row ，所以是第一個
                _selectedBatch = _BatchList.Find(x => x.UID == "" + dataGridViewX1.SelectedRows[0].Tag); 

            }
            else
            {
                btnGenMakeUpGroup.Enabled = false;
            }

        }

        //刪除 補考梯次
        private void MenuItemDelete_Click(Object sender, System.EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("是否要刪除本補考梯次?", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                // LOG 資訊
                string _actor = DSAServices.UserAccount; ;
                string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

                //拚SQL
                // 兜資料
                List<string> dataList = new List<string>();

                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_batch
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description
                    ,'{4}'::TEXT AS included_class_id                    
                    ,'{5}'::BIGINT AS uid
                ", _selectedBatch.MakeUp_Batch, _schoolYear, _semester, _selectedBatch.Description, _selectedBatch.Included_Class_ID, _selectedBatch.UID);
                dataList.Add(data);

                string dataString = string.Join(" UNION ALL", dataList);
                ;

                string sql = string.Format(@"
WITH data_row AS
(
    {0}
)
,delete_data AS(			 
DELETE
FROM $make.up.batch
WHERE $make.up.batch.uid IN
(
    SELECT 
        data_row.uid 
    FROM data_row
)
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
	, '刪除高中補考梯次' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{2}' AS client_info
	, '刪除高中補考梯次'AS action_by   
	, '刪除 高中補考 學年度「'|| data_row.school_year||'」，學期「'|| data_row.semester||'」， 補考梯次「'|| data_row.makeup_batch||'」，補考說明 「'|| data_row.description ||'」。' AS description 
FROM
	data_row

", dataString, _actor, _client_info);
            

            K12.Data.UpdateHelper uh = new UpdateHelper();

                //執行sql
                uh.Execute(sql);

                FISCA.Presentation.Controls.MsgBox.Show("刪除成功。");

                // 刷新畫面
                RefreshListView();
            }           
        }

        private void dataGridViewX1_MouseDown(object sender, MouseEventArgs e)
        {
            MenuItem[] menuItems = new MenuItem[0];

            MenuItem menuItems_delete = new MenuItem("刪除", MenuItemDelete_Click);

            menuItems = new MenuItem[] { menuItems_delete };

            if (e.Button == MouseButtons.Right && dataGridViewX1.SelectedRows.Count > 0)
            {
                ContextMenu buttonMenu = new ContextMenu(menuItems);

                dataGridViewX1.ContextMenu = buttonMenu;

                dataGridViewX1.ContextMenu.Show(dataGridViewX1, e.Location);
            }

        }
    }
}

