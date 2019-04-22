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
    public partial class MakeUpBatchManagerForm : FISCA.Presentation.Controls.BaseForm
    {        
        private List<string> _BatchIDList;
        
        private BackgroundWorker _worker;

                      
        public MakeUpBatchManagerForm()
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

            // 預設為學校的當學年度學期
            cboSchoolYear.Text = School.DefaultSchoolYear;
            cbosemester.Text = School.DefaultSemester;

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;
            
            FillCboTemplate();
            
            picLoading.Visible = false;
        }

        private void GetESLTemplate()
        {
            picLoading.Visible = true;

            #region 取得補考梯次 
            
            string query = @"";

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            //整理目前的補考梯次 資料
            if (dt.Rows.Count > 0) 
            {
                

            }
            #endregion
        }


        private void FillCboTemplate()
        {
            picLoading.Visible = true;

            //cboSchoolYear.Items.Clear();

        }

        /// <summary>
        /// 更新ListView
        /// </summary>
        private void RefreshListView()
        {
            picLoading.Visible = true;

            if (cbosemester.SelectedItem == null) return;
                                   
            // 暫停畫面控制項
            
            cboSchoolYear.SuspendLayout();
            cbosemester.SuspendLayout();
            dataGridViewX1.SuspendLayout();

            _worker.RunWorkerAsync();
        }


        /// <summary>
        /// 依試別取得所有課程成績
        /// </summary>
        /// <param name="targetTermName"></param>
        private void LoadCourses(string targetTermName)
        {

        }


        /// <summary>
        /// 將補考梯次填入 DataGridView
        /// </summary>
        private void FillCourses()
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

        private void cboTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {           
            cbosemester.Enabled = true;
            
            FillcboExam();
        }

        private void FillcboExam()
        {
            //cbosemester.Items.Clear();

        }


        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 繼續 畫面控制項       
            picLoading.Visible = false;
                 
            cboSchoolYear.ResumeLayout();
            cbosemester.ResumeLayout();
            
            dataGridViewX1.ResumeLayout();
                        
            //FillCourses(GetDisplayDataGridViewList());

            FISCA.Presentation.MotherForm.SetStatusBarMessage("取得補考梯次完成");
            
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage(""+e.UserState, e.ProgressPercentage);
        }


        private void dataGridViewX1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (e.ColumnIndex < 0) return;
            if (e.RowIndex < 0) return;
            DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            

            //inputForm.ShowDialog();

            RefreshListView(); // 更改完成績後，重整畫面
        }

        private void MakeUpBatchManagerForm_Load(object sender, EventArgs e)
        {

        }
    }
}

