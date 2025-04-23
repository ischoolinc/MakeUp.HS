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
    public partial class ExportMakeUpReportForm : FISCA.Presentation.Controls.BaseForm
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


        //  <BatchID,Batch>
        private Dictionary<string, UDT_MakeUpBatch> _MakeUpBatchDict = new Dictionary<string, UDT_MakeUpBatch>();

        // 補考 成績結構 <MakeUpGroupID,List<scoreItem>>
        private Dictionary<string, List<UDT_MakeUpData>> _scoreDict = new Dictionary<string, List<UDT_MakeUpData>>();


        public ExportMakeUpReportForm()
        {
            InitializeComponent();

           


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
                    COALESCE(is_archive, '') = '' 
                    AND 
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

            if ("" + selectedItem.Text == "")
            {
                MsgBox.Show("梯次別不得空白", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }


            _targetBatchID = "" + selectedItem.Tag;

            string printMode = "";

            if (radioBtnByGroup.Checked)
            {
                printMode = "依群組";
            }

            if (radioBtnByStudent.Checked)
            {
                printMode = "依學生";
            }

            #region 取得 該補考梯次 是否有補考群組，假如無，則不給列印補考公告
            string query = @"
                    SELECT* FROM  $make.up.group WHERE ref_makeup_batch_id = '" + _targetBatchID + "'" ;

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            if (dt.Rows.Count == 0)
            {
                MsgBox.Show("本梯次尚未建立補考群組，請至教務作業/補考作業 建立。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            #endregion

            

           // 在本視窗選完梯次後， 進入下一個視窗正式列印 分兩種模式 (依群組、依學生)
           ExportMakeUpReportSettingForm emursf = new ExportMakeUpReportSettingForm(_targetBatchID, printMode);

            emursf.ShowDialog();

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

        // 依群組列印 與 依學生列印 兩者為互斥
        private void radioBtnByGroup_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnByGroup.Checked)
            {
                radioBtnByStudent.Checked = !radioBtnByGroup.Checked;
            }
            else
            {
                radioBtnByStudent.Checked = !radioBtnByGroup.Checked;
            }        
        }

        // 依群組列印 與 依學生列印 兩者為互斥
        private void radioBtnByStudent_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnByStudent.Checked)
            {
                radioBtnByGroup.Checked = !radioBtnByStudent.Checked;
            }
            else
            {
                radioBtnByGroup.Checked = !radioBtnByStudent.Checked;
            }
        }
    }
}

