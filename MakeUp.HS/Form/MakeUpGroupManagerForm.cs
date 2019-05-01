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
    public partial class MakeUpGroupManagerForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<string> _BatchIDList;

        private List<UDT_MakeUpBatch> _batchList;

        private List<UDT_MakeUpGroup> _groupList;

        private List<K12.Data.TeacherRecord> _teacherList;
       
        // 抓梯次用　BackgroundWorker
        private BackgroundWorker _batchWorker;

        // 抓群組用　BackgroundWorker
        private BackgroundWorker _groupWorker;

        // 上傳更新用　BackgroundWorker
        private BackgroundWorker _updateWorker;

        // 學年度
        private string _schoolYear;

        // 學期
        private string _semester;


        // 現在點在哪一小節
        private DevComponents.AdvTree.Node _node_now;

        // 目前選的梯次 id
        private string _selectedBatchID;

        // 目前選的群組 List
        private List<UDT_MakeUpGroup> _selectedGroupList;

        // 目前選的群組 id
        private string _selectedGroupID;


        public MakeUpGroupManagerForm()
        {
            InitializeComponent();

            _batchList = new List<UDT_MakeUpBatch>();

            _groupList = new List<UDT_MakeUpGroup>();

            // 取得教師清單
            _teacherList = K12.Data.Teacher.SelectAll();

            _batchWorker = new BackgroundWorker();
            _batchWorker.DoWork += new DoWorkEventHandler(BatchWorker_DoWork);
            _batchWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BatchWorker_RunWorkerCompleted);
            _batchWorker.ProgressChanged += new ProgressChangedEventHandler(BatchWorker_ProgressChanged);
            _batchWorker.WorkerReportsProgress = true;

            _groupWorker = new BackgroundWorker();
            _groupWorker.DoWork += new DoWorkEventHandler(GroupWorker_DoWork);
            _groupWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GroupWorker_RunWorkerCompleted);
            _groupWorker.ProgressChanged += new ProgressChangedEventHandler(GroupWorker_ProgressChanged);
            _groupWorker.WorkerReportsProgress = true;


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



        private void BatchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetMakeUpBatch();
        }

        private void BatchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            picLoadingAdvTreeMakeUpBatch.Visible = false;

            // 解凍　畫面　控制項
            ResumeAllLayout();

            // 清除舊項目
            advTreeMakeUpBatch.Nodes.Clear();

            // 清除舊項目
            dataGridViewX1.Rows.Clear();

            string semesterNodeString = _schoolYear + "學年度第" + _semester + "學期";

            DevComponents.AdvTree.Node semesterNode = new DevComponents.AdvTree.Node(semesterNodeString);

            // 父 Node 不可選，也不可拖曳
            semesterNode.Selectable = false;
            semesterNode.DragDropEnabled = false;


            // 將 補考梯次　子 node　批次 加入 　學年度學期父 node
            foreach (UDT_MakeUpBatch batchRecord in _batchList)
            {
                string batchNodeString = batchRecord.MakeUp_Batch;

                DevComponents.AdvTree.Node batchNode = new DevComponents.AdvTree.Node(batchNodeString);

                // 補考梯次　子 node 可以選，但也不可拖曳 
                batchNode.DragDropEnabled = false;

                // Node 的 Tag 為 補考梯次的系統編號
                batchNode.Tag = batchRecord.UID;

                batchNode.NodeMouseDown += new System.Windows.Forms.MouseEventHandler(NodeMouseDown);
                
                semesterNode.Nodes.Add(batchNode);
            }

            //  預設將node 展開
            semesterNode.ExpandAll();

            advTreeMakeUpBatch.Nodes.Add(semesterNode);

            FISCA.Presentation.MotherForm.SetStatusBarMessage("取得補考梯次完成");

            //  假如有補考梯次，主動觸發　滑鼠點擊　第一個項目　，　帶出　補考群組
            if (advTreeMakeUpBatch.Nodes.Count > 0)
            {
                if (advTreeMakeUpBatch.Nodes[0].Nodes.Count > 0)
                {
                    DevComponents.AdvTree.Node defaultNode = advTreeMakeUpBatch.Nodes[0].Nodes[0];

                    defaultNode.SetSelectedCell(defaultNode.Cells[0], DevComponents.AdvTree.eTreeAction.Mouse);
                   
                    NodeMouseDown(defaultNode, new MouseEventArgs(MouseButtons.Left,1,0,0,0));
                }
            }
        }

        private void BatchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }

        private void GroupWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetMakeUpGroup();
        }

        private void GroupWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            picLoadingDgvXMakeUpGroup.Visible = false;

            // 解凍　畫面　控制項
            ResumeAllLayout();

            // 清除舊項目
            dataGridViewX1.Rows.Clear();

            foreach (UDT_MakeUpGroup groupRecord in _groupList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                // row 的 Tag 為 補考梯次的系統編號
                row.Tag = groupRecord.UID;

                row.Cells[0].Value = groupRecord.MakeUp_Group;
               
                row.Cells[1].Value = groupRecord.Ref_Teacher_ID;

                row.Cells[2].Value = groupRecord.StudentCount;

                row.Cells[3].Value = groupRecord.Description;

                dataGridViewX1.Rows.Add(row);
            }


            FISCA.Presentation.MotherForm.SetStatusBarMessage("取得補考群組完成");
        }

        private void GroupWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }



        private void cboSchoolYear_SelectedIndexChanged_1(object sender, EventArgs e)
        {

            if (cbosemester.Text == "")
            {
                return;
            }

            RefreshAdvTreeMakeUpBatchView();
        }

        private void cbosemester_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cboSchoolYear.Text == "")
            {
                return;
            }

            RefreshAdvTreeMakeUpBatchView();
        }

        private void GetMakeUpBatch()
        {
            _batchList.Clear();

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

                    _batchList.Add(batch);
                }
            }
            #endregion
        }

        private void GetMakeUpGroup()
        {
            _groupList.Clear();

            #region 取得補考群組

            string query = @"
SELECT 
$make.up.group.uid
,$make.up.group.makeup_group
,$make.up.group.ref_teacher_id
,$make.up.group.description
,COUNT($make.up.data.uid) AS studentCount
FROM  $make.up.group
LEFT JOIN  $make.up.data ON  $make.up.data.ref_makeup_group_id :: BIGINT = $make.up.group.uid
WHERE  $make.up.group.ref_makeup_batch_id = '" + _selectedBatchID + @"'
GROUP BY  $make.up.group.uid ";

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

                    K12.Data.TeacherRecord tr = _teacherList.Find(t => t.ID == "" + row["ref_teacher_id"]);

                    //閱卷老師
                    group.Ref_Teacher_ID = tr!=null ? tr.Name :"";

                    //補考人數
                    group.StudentCount = "" + row["studentCount"]; ;

                    // 描述
                    group.Description = "" + row["description"];
                    
                    _groupList.Add(group);
                }
            }
            #endregion
        }


        /// <summary>
        /// 更新advTree　補考梯次
        /// </summary>
        private void RefreshAdvTreeMakeUpBatchView()
        {
            picLoadingAdvTreeMakeUpBatch.Visible = true;

            _schoolYear = cboSchoolYear.Text;
            _semester = cbosemester.Text;

            // 暫停畫面控制項
            SuspendAllLayout();

            _batchWorker.RunWorkerAsync();
        }


        /// <summary>
        /// 更新dataGridViewX1　補考群組
        /// </summary>
        private void RefreshDataGridViewXMakeUpGroupView()
        {
            picLoadingDgvXMakeUpGroup.Visible = true;

            // 暫停畫面控制項
            SuspendAllLayout();

            _groupWorker.RunWorkerAsync();
        }


        // 2019/05/01 穎驊 註記， 發現 panel 在  expand 的前後，         
        //會強制將內容原本已經 設定好Visible 控制項，通通變成不可見、可見，　因此要另外做調整。
        private void expandablePanel1_ExpandedChanged(object sender, DevComponents.DotNetBar.ExpandedChangeEventArgs e)
        {
            picLoadingAdvTreeMakeUpBatch.Visible = false;

        }

        // 點下項目後 更新補考群組
        private void NodeMouseDown(object sender, MouseEventArgs e)
        {
            _node_now = (DevComponents.AdvTree.Node)sender;

            // 現在選的 node 的　Tag 就是　batchId
            _selectedBatchID = "" + _node_now.Tag;

            RefreshDataGridViewXMakeUpGroupView();
        }

        // 凍結　畫面上所有　控制項
        private void SuspendAllLayout()
        {
            advTreeMakeUpBatch.SuspendLayout();
            cboSchoolYear.SuspendLayout();
            cbosemester.SuspendLayout();
            dataGridViewX1.SuspendLayout();
        }


        // 解凍　畫面上所有　控制項
        private void ResumeAllLayout()
        {
            advTreeMakeUpBatch.ResumeLayout();
            cboSchoolYear.ResumeLayout();
            cbosemester.ResumeLayout();
            dataGridViewX1.ResumeLayout();
        }



        private void dataGridViewX1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (e.ColumnIndex < 0) return;
            if (e.RowIndex < 0) return;
            DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            ////  找到選取到的 梯次
            //_selectedBatch = _BatchList.Find(x => x.UID == "" + cell.OwningRow.Tag);

            //// 修改模式
            //InsertUpdateMakeUpBatchForm iumbf = new InsertUpdateMakeUpBatchForm("修改", cboSchoolYear.Text, cbosemester.Text, _selectedBatch);
            //iumbf.ShowDialog();

            //RefreshListView(); //重整畫面

        }

        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            DevComponents.DotNetBar.Controls.DataGridViewX drvx = (DevComponents.DotNetBar.Controls.DataGridViewX)sender;

            foreach (DataGridViewRow row in drvx.Rows)
            {
                if (row.Selected)
                {
                    //_selectedGroupList.Add();

                }
            }
        }
    }
}

