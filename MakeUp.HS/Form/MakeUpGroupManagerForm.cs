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
using Framework;

namespace MakeUp.HS.Form
{
    public partial class MakeUpGroupManagerForm : FISCA.Presentation.Controls.BaseForm
    {
        private List<string> _BatchIDList;

        private List<UDT_MakeUpBatch> _batchList;

        // 原始的 groupList
        private List<UDT_MakeUpGroup> _groupListOri;

        private List<UDT_MakeUpGroup> _groupList;

        private List<K12.Data.TeacherRecord> _teacherList;

        // 抓補考梯次用　BackgroundWorker
        private BackgroundWorker _batchWorker;

        // 抓補考群組用　BackgroundWorker
        private BackgroundWorker _groupWorker;

        // 上傳更新用　BackgroundWorker
        private BackgroundWorker _updateWorker;

        // 學年度
        private string _schoolYear;

        // 學期
        private string _semester;

        // 動作 (管理補考群組、管理補考成績)
        private string _action;


        // 現在點在哪一小節
        private DevComponents.AdvTree.Node _node_now;


        // 目前選的單一梯次 
        private UDT_MakeUpBatch _selectedBatch;

        // 目前選的群組 List
        private List<UDT_MakeUpGroup> _selectedGroupList;

        // 目前選的單一群組 
        private UDT_MakeUpGroup _selectedGroup;

        // 群組 UID 畫面回來排序使用
        List<string> GroupUIDList = new List<string>();
                
        public MakeUpGroupManagerForm(string action)
        {
            _action = action;

            InitializeComponent();

            _batchList = new List<UDT_MakeUpBatch>();

            _groupList = new List<UDT_MakeUpGroup>();

            _selectedGroupList = new List<UDT_MakeUpGroup>();

            // 取得教師清單
            List<TeacherRecord> trList = K12.Data.Teacher.SelectAll();
            _teacherList = new List<TeacherRecord>();
            foreach(TeacherRecord tr in trList)
            {
                if (tr.Status == TeacherRecord.TeacherStatus.刪除)
                    continue;

                _teacherList.Add(tr);
            }

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
                // 封存不出現
                if (!string.IsNullOrWhiteSpace(batchRecord.is_archive))
                    continue;

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

                    NodeMouseDown(defaultNode, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
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

            lblIsDirty.Visible = false;


            // 解凍　畫面　控制項
            ResumeAllLayout();

            // 清除舊項目
            dataGridViewX1.Rows.Clear();

            foreach (UDT_MakeUpGroup groupRecord in _groupList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                // row 的 Tag 為 補考群組的系統編號
                row.Tag = groupRecord.UID;

                row.Cells[0].Value = groupRecord.MakeUp_Group;

                K12.Data.TeacherRecord tr = _teacherList.Find(t => t.ID == groupRecord.Ref_Teacher_ID);

                if (tr != null)
                {
                    if (!string.IsNullOrEmpty(tr.Nickname))
                    {
                        row.Cells[1].Value = tr.Name + "(" + tr.Nickname + ")";
                    }
                    else
                    {
                        row.Cells[1].Value = tr.Name;
                    }
                }
                else
                {
                    row.Cells[1].Value = "";
                }

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
"AND semester = '" + _semester + "' ORDER BY makeup_batch ASC";

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            //整理目前的補考梯次 資料
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    // 判斷是封存不顯示
                    if (row["is_archive"] != null )
                    {
                        if (row["is_archive"].ToString() == "是")
                            continue;
                    }

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
,$make.up.group.ref_makeup_batch_id
,$make.up.group.makeup_group
,$make.up.group.ref_teacher_id
,$make.up.group.description
,$make.up.group.makeup_date
,$make.up.group.makeup_time
,$make.up.group.makeup_place
,COUNT($make.up.data.uid) AS studentCount
FROM  $make.up.group
LEFT JOIN  $make.up.data ON  $make.up.data.ref_makeup_group_id :: BIGINT = $make.up.group.uid
WHERE  $make.up.group.ref_makeup_batch_id = '" + _selectedBatch.UID + @"'
GROUP BY  $make.up.group.uid ORDER BY makeup_group ASC";

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

                    //補考人數
                    group.StudentCount = "" + row["studentCount"]; ;

                    // 描述
                    group.Description = "" + row["description"];

                    // 補考日期
                    group.MakeUp_Date = "" + row["makeup_date"];

                    // 補考時間
                    group.MakeUp_Time = "" + row["makeup_time"];

                    // 補考地點
                    group.MakeUp_Place = "" + row["makeup_place"];

                    group.IsDirty = false;

                    // 把 補考梯次 配對
                    group.MakeUpBatch = _selectedBatch;

                    _groupList.Add(group);
                }
            }

            // 如果已有 UID List 排序
            if (GroupUIDList.Count > 0)
            {
                _groupList = _groupList.OrderBy(d => GroupUIDList.IndexOf(d.UID)).ToList();
            }
            

            //另存份  原本的list 作為比較
            _groupListOri = new List<UDT_MakeUpGroup>(_groupList.ToArray());

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

            if (!_groupWorker.IsBusy)
            {
                _groupWorker.RunWorkerAsync();
            }
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

            // 現在選的 node 的　Tag 就是　batch
            _selectedBatch = _batchList.Find(b => b.UID == "" + _node_now.Tag);

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

            if (lblIsDirty.Visible)
            {
                FISCA.Presentation.Controls.MsgBox.Show("編輯群組資料前，請先儲存。");
                return;
            }

            // 紀錄目前 UID
            GroupUIDList.Clear();
            foreach(DataGridViewRow drv in dataGridViewX1.Rows)
            {
                if (drv.IsNewRow)
                    continue;

                string uid = drv.Tag.ToString();
                GroupUIDList.Add(uid);
            }

            ////  找到選取到的 梯次
            _selectedGroup = _selectedGroupList.Find(x => x.UID == "" + cell.OwningRow.Tag);

            if (_action == "管理補考成績")
            {
                // 管理補考成績                
                InsertUpdateMakeUpGroupForm iumgf = new InsertUpdateMakeUpGroupForm(_schoolYear, _semester, "管理補考成績", _selectedGroup);
                if (iumgf.ShowDialog() == DialogResult.Yes)
                {
                    RefreshListView(); //重整畫面
                }

            }
            else
            {
                // 修改模式
                InsertUpdateMakeUpGroupForm iumgf = new InsertUpdateMakeUpGroupForm(_schoolYear, _semester, "修改群組", _selectedGroup);
                if (iumgf.ShowDialog() == DialogResult.Yes)
                {
                    RefreshListView(); //重整畫面
                }

            }

            //RefreshListView(); //重整畫面

        }

        /// <summary>
        /// 更新ListView
        /// </summary>
        private void RefreshListView()
        {
            picLoadingDgvXMakeUpGroup.Visible = true;

            _schoolYear = cboSchoolYear.Text;
            _semester = cbosemester.Text;

            // 暫停畫面控制項

            cboSchoolYear.SuspendLayout();
            cbosemester.SuspendLayout();
            dataGridViewX1.SuspendLayout();

            _groupWorker.RunWorkerAsync();
        }

        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            DevComponents.DotNetBar.Controls.DataGridViewX drvx = (DevComponents.DotNetBar.Controls.DataGridViewX)sender;

            int selectRowsTotalCount = 0;

            // 先數 這次使用者 全部一共選擇幾個項目
            foreach (DataGridViewRow row in drvx.Rows)
            {
                if (row.Selected)
                {
                    selectRowsTotalCount++;
                }
            }

            // 假如 選擇 Row的總數 為 1 代表 這是第一個選的 Row， 
            // 之後，要做補考群組合併 的補考群組，都會合到第一個補考群組
            if (selectRowsTotalCount == 1)
            {
                // 把之前 選的群組Group 都清空
                _selectedGroupList.Clear();

                // 先將所有的Row 都視為非第一個選 row
                foreach (UDT_MakeUpGroup group in _groupList)
                {
                    group.IsFirstSelectedRow = false;
                }

                foreach (DataGridViewRow row in drvx.Rows)
                {
                    if (row.Selected)
                    {
                        UDT_MakeUpGroup firstSelectedGroup = _groupList.Find(g => g.UID == "" + row.Tag);

                        // 用uid 找的到的 是原本就有的 補考群組
                        if (firstSelectedGroup != null)
                        {
                            firstSelectedGroup.IsFirstSelectedRow = true;
                            _selectedGroupList.Add(firstSelectedGroup);
                        }


                    }
                }
            }


            // 假如 選擇的 Row 總數 大於 1，代表使用者再選擇完第一個Row後
            // 還有再選擇其他的 Row，之前第一個Row(要合併其他人的， IsFirstSelectedRow =true) 就不必再加入List
            // 後面選的 Row(被合併的, IsFirstSelectedRow =false ) 再加List 即可
            if (selectRowsTotalCount > 1)
            {
                foreach (DataGridViewRow row in drvx.Rows)
                {
                    if (row.Selected)
                    {
                        UDT_MakeUpGroup selectedGroup = _groupList.Find(g => g.UID == "" + row.Tag);

                        // 非第一個選的 row ，且不在_selectedGroupList 中 就加
                        if (!selectedGroup.IsFirstSelectedRow && !_selectedGroupList.Contains(selectedGroup))
                        {
                            _selectedGroupList.Add(selectedGroup);
                        }


                    }
                }

            }





        }

        private void dataGridViewX1_MouseDown(object sender, MouseEventArgs e)
        {
            MenuItem[] menuItems = new MenuItem[0];

            MenuItem menuItems_insertNewGroup = new MenuItem("新增補考群組", MenuItemInsertGroup_Click);

            MenuItem menuItems_deleteGroup = new MenuItem("刪除補考群組", MenuItemDeleteGroup_Click);

            MenuItem menuItems_MergeGroup = new MenuItem("合併補考群組", MenuItemMergeGroup_Click);

            MenuItem menuItems_assingnTeacher = new MenuItem("指定閱卷老師", MenuItemAssingnTeacher_Click);

            menuItems = new MenuItem[] { menuItems_insertNewGroup, menuItems_deleteGroup, menuItems_MergeGroup, menuItems_assingnTeacher };

            // 只有再管理補考群組的模式下，點右鍵 才可以叫出選單
            if (e.Button == MouseButtons.Right && _action == "管理補考群組")
            {
                ContextMenu buttonMenu = new ContextMenu(menuItems);

                dataGridViewX1.ContextMenu = buttonMenu;

                dataGridViewX1.ContextMenu.Show(dataGridViewX1, e.Location);
            }
        }

        // 新增補考群組
        private void MenuItemInsertGroup_Click(Object sender, System.EventArgs e)
        {
            // 修改模式
            InsertUpdateMakeUpGroupForm iumgf = new InsertUpdateMakeUpGroupForm(_schoolYear, _semester, "新增群組", _selectedBatch);
            iumgf.ShowDialog();

            RefreshListView(); //重整畫面
        }

        // 刪除補考群組
        private void MenuItemDeleteGroup_Click(Object sender, System.EventArgs e)
        {
            if (_selectedGroupList.Find(g => g.MakeUp_Group == "未分群組") != null)
            {
                FISCA.Presentation.Controls.MsgBox.Show("【未分群組】 不得刪除。");
                return;
            }

            UDT_MakeUpGroup non_group = _groupList.Find(g => g.MakeUp_Group == "未分群組");

            foreach (UDT_MakeUpGroup selectGroup in _selectedGroupList)
            {
                // 把 刪除的學生人數 都數到未分群組
                if (non_group != null)
                {
                    non_group.StudentCount = "" + (int.Parse(non_group.StudentCount) + int.Parse(selectGroup.StudentCount));
                }

                selectGroup.IsDirty = true;

                selectGroup.Action = "刪除";
            }

            RefreshUIGroupListView();

        }

        // 合併補考群組
        private void MenuItemMergeGroup_Click(Object sender, System.EventArgs e)
        {
            if (_selectedGroupList.Find(g => g.MakeUp_Group == "未分群組") != null)
            {
                FISCA.Presentation.Controls.MsgBox.Show("【未分群組】 不得合併。");
                return;
            }

            if (_selectedGroupList.Count < 2)
            {
                FISCA.Presentation.Controls.MsgBox.Show("合併功能需選擇 大於1個群組。");
                return;
            }

            // 合併後的 群組id
            string new_merge_group_id = "";

            // 合併後的 群組名稱
            string new_merge_group_name = "";

            // 第一個群組(併別人的)
            UDT_MakeUpGroup firstGroup = _selectedGroupList.Find(g => g.IsFirstSelectedRow);

            new_merge_group_id = firstGroup.UID;

            new_merge_group_name = firstGroup.MakeUp_Group;


            foreach (UDT_MakeUpGroup selectGroup in _selectedGroupList)
            {
                // 非第一個選的 就是要被併的
                if (!selectGroup.IsFirstSelectedRow)
                {
                    // 把從其他群組併過來的 人數 加上
                    firstGroup.StudentCount = "" + (int.Parse(firstGroup.StudentCount) + int.Parse(selectGroup.StudentCount));

                    selectGroup.New_Merge_Group_ID = new_merge_group_id;

                    selectGroup.New_Merge_Group_Name = new_merge_group_name;

                    selectGroup.IsDirty = true;

                    selectGroup.Action = "合併";

                }
            }

            RefreshUIGroupListView();

        }


        private void MenuItemAssingnTeacher_Click(Object sender, System.EventArgs e)
        {
            AssignTeacherForm atf = new AssignTeacherForm();

            if (DialogResult.OK == atf.ShowDialog())
            {
                foreach (UDT_MakeUpGroup selectGroup in _selectedGroupList)
                {
                    // 和原本的教師id 不同 就指定成新的教師 id， 且標記 有改變
                    if (selectGroup.Ref_Teacher_ID != atf.assignteacherID)
                    {
                        selectGroup.New_Ref_Teacher_ID = atf.assignteacherID;

                        selectGroup.IsDirty = true;

                        selectGroup.Action = "更新";
                    }
                }

                RefreshUIGroupListView();
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewX1.Rows.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("目前無任何補考群組可以儲存。");
                return;
            }

            picLoadingDgvXMakeUpGroup.Visible = true;

            // LOG 資訊
            string _actor = DSAServices.UserAccount; ;
            string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            //拚SQL
            // 兜資料
            List<string> dataList = new List<string>();

            string sql = "";

            // 更新的
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                UDT_MakeUpGroup groupRecord = _groupList.Find(g => g.UID == "" + row.Tag);

                // 再原有補考群組清單找不到， 就是 新增的
                if (groupRecord == null)
                {

                }
                else
                {
                    // 沒有改變的項目 也不必儲存
                    if (!groupRecord.IsDirty)
                    {
                        continue;
                    }
                }

                // row 的 Tag 為 補考群組的系統編號
                groupRecord.UID = "" + row.Tag;

                groupRecord.MakeUp_Group = "" + row.Cells[0].Value;

                groupRecord.Description = "" + row.Cells[3].Value;



                string logDetail = @" 高中補考 學年度「" + _schoolYear +
                   @"」，學期「" + _semester + @"」， 補考梯次「 " + _selectedBatch.MakeUp_Batch + @"」， 補考群組「 " + groupRecord.MakeUp_Group + @"」
                    ";

                string old_teacher_name = "";

                string new_teacher_name = "";

                if (groupRecord.New_Ref_Teacher_ID != null)
                {
                    K12.Data.TeacherRecord old_tr = _teacherList.Find(t => t.ID == groupRecord.Ref_Teacher_ID);

                    K12.Data.TeacherRecord new_tr = _teacherList.Find(t => t.ID == groupRecord.New_Ref_Teacher_ID);

                    if (old_tr != null)
                    {
                        if (!string.IsNullOrEmpty(old_tr.Nickname))
                        {
                            old_teacher_name = old_tr.Name + "(" + old_tr.Nickname + ")";
                        }
                        else
                        {
                            old_teacher_name = old_tr.Name;
                        }
                    }
                    else
                    {
                        old_teacher_name = "";
                    }

                    if (new_tr != null)
                    {
                        if (!string.IsNullOrEmpty(new_tr.Nickname))
                        {
                            new_teacher_name = new_tr.Name + "(" + new_tr.Nickname + ")";
                        }
                        else
                        {
                            new_teacher_name = new_tr.Name;
                        }
                    }
                    else
                    {
                        new_teacher_name = "";
                    }
                }

                if (groupRecord.Ref_Teacher_ID != groupRecord.New_Ref_Teacher_ID)
                {
                    logDetail += "閱卷老師由「 " + old_teacher_name + @"」， 更改為「 " + new_teacher_name + @"」";

                    // 更新統一用Ref_Teacher_ID
                    groupRecord.Ref_Teacher_ID = groupRecord.New_Ref_Teacher_ID;
                }

                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_group
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description                                    
                    ,'{4}'::TEXT AS log_detail                    
                    ,'{5}'::TEXT AS ref_makeup_batch_id
                    ,'{6}'::TEXT AS ref_teacher_id
                    ,'{7}'::BIGINT AS uid
                    ,'{8}'::TEXT AS action
                    ,'{9}'::TEXT AS new_merge_group_id
                ", groupRecord.MakeUp_Group, _schoolYear, _semester, groupRecord.Description, logDetail, _selectedBatch.UID, groupRecord.Ref_Teacher_ID != null ? groupRecord.Ref_Teacher_ID : "", groupRecord.UID, groupRecord.Action, groupRecord.New_Merge_Group_ID);

                dataList.Add(data);
            }


            foreach (UDT_MakeUpGroup group in _groupList)
            {
                // 刪除的
                if (group.Action == "刪除")
                {
                    string logDetail = @" 高中補考 學年度「" + _schoolYear +
                  @"」，學期「" + _semester + @"」， 補考梯次「 " + _selectedBatch.MakeUp_Batch + @"」， 補考群組「 " + group.MakeUp_Group + @"」
                    ";

                    logDetail += " 刪除， 其下的補考學生將分派至【未分群組】";

                    string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_group
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description                                    
                    ,'{4}'::TEXT AS log_detail                    
                    ,'{5}'::TEXT AS ref_makeup_batch_id
                    ,'{6}'::TEXT AS ref_teacher_id
                    ,'{7}'::BIGINT AS uid
                    ,'{8}'::TEXT AS action
                    ,'{9}'::TEXT AS new_merge_group_id
                ", group.MakeUp_Group, _schoolYear, _semester, group.Description, logDetail, _selectedBatch.UID, group.Ref_Teacher_ID != null ? group.Ref_Teacher_ID : "", group.UID, group.Action, group.New_Merge_Group_ID);

                    dataList.Add(data);
                }

                // 合併的
                if (group.Action == "合併")
                {
                    string logDetail = @" 高中補考 學年度「" + _schoolYear +
                  @"」，學期「" + _semester + @"」， 補考梯次「 " + _selectedBatch.MakeUp_Batch + @"」， 補考群組「 " + group.MakeUp_Group + @"」
                    ";

                    logDetail += " 合併 至【" + group.New_Merge_Group_Name + "】群組。";

                    string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS makeup_group
                    ,'{1}'::TEXT AS school_year
                    ,'{2}'::TEXT AS semester
                    ,'{3}'::TEXT AS description                                    
                    ,'{4}'::TEXT AS log_detail                    
                    ,'{5}'::TEXT AS ref_makeup_batch_id
                    ,'{6}'::TEXT AS ref_teacher_id
                    ,'{7}'::BIGINT AS uid
                    ,'{8}'::TEXT AS action
                    ,'{9}'::TEXT AS new_merge_group_id
                ", group.MakeUp_Group, _schoolYear, _semester, group.Description, logDetail, _selectedBatch.UID, group.Ref_Teacher_ID != null ? group.Ref_Teacher_ID : "", group.UID, group.Action, group.New_Merge_Group_ID);

                    dataList.Add(data);
                }


            }





            string dataString = string.Join(" UNION ALL", dataList);

            sql = string.Format(@"
WITH data_row AS(			 
                {0}     
)
,update_group_data AS (
    Update $make.up.group
    SET
        makeup_group = data_row.makeup_group
        ,description = data_row.description        
        ,ref_teacher_id = data_row.ref_teacher_id    
    FROM data_row     
    WHERE $make.up.group.uid = data_row.uid  
    AND data_row.action ='更新'
)
,delete_group_data AS (
    DELETE 
    FROM  $make.up.group    
    WHERE $make.up.group.uid IN
    (   
        SELECT data_row.uid
		FROM data_row
        WHERE data_row.action ='刪除' OR data_row.action ='合併'
    )    
),check_non_group_id AS
(
    --確認【未分群組】 的id 如果沒有 就幫他建立新的
    INSERT INTO $make.up.group
        (Ref_MakeUp_Batch_ID, MakeUp_Group)
    SELECT '{3}', '未分群組'
    WHERE
        NOT EXISTS (
            SELECT 
            * 
            FROM $make.up.group 
            WHERE
                Ref_MakeUp_Batch_ID = '{3}'
                AND MakeUp_Group ='未分群組'               
        )
     RETURNING *
),non_group_id AS
(   
    SELECT 
         * 
    FROM check_non_group_id  
    UNION ALL
    SELECT 
         * 
    FROM $make.up.group  
    WHERE
        Ref_MakeUp_Batch_ID = '{3}'
        AND MakeUp_Group ='未分群組'
),update_delete_data_data AS (
    Update $make.up.data
    SET
        Ref_MakeUp_Group_ID = non_group_id.uid       
    FROM data_row    
    LEFT JOIN non_group_id ON data_row.action ='刪除'
    WHERE $make.up.data.Ref_MakeUp_Group_ID ::BIGINT = data_row.uid  
    AND data_row.action ='刪除'
)
,update_merge_data_data AS (
    Update $make.up.data
    SET
        Ref_MakeUp_Group_ID = data_row.new_merge_group_id       
    FROM data_row    
    WHERE $make.up.data.Ref_MakeUp_Group_ID ::BIGINT = data_row.uid  
    AND data_row.action ='合併'
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
	, '編輯高中補考群組' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{2}' AS client_info
	, '編輯高中補考群組'AS action_by   
	,data_row.log_detail  AS description 
FROM
	data_row

", dataString, _actor, _client_info, _selectedBatch.UID);

            //沒有資料不處理
            if (!string.IsNullOrWhiteSpace(dataString))
            {
                try
                {
                    K12.Data.UpdateHelper uh = new UpdateHelper();

                    //執行sql
                    uh.Execute(sql);


                    FISCA.Presentation.Controls.MsgBox.Show("儲存成功。");


                    // 儲存完畢 重新整理 介面
                    RefreshListView();
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("儲存失敗," + ex.Message);
                    picLoadingDgvXMakeUpGroup.Visible = false;
                }
            }
            else
            {
                picLoadingDgvXMakeUpGroup.Visible = false;
            }


        }


        // 更新 補考群組畫面， 僅是介面上的更新，並沒有做任何的儲存。
        private void RefreshUIGroupListView()
        {

            // 清除舊項目
            dataGridViewX1.Rows.Clear();

            foreach (UDT_MakeUpGroup groupRecord in _groupList)
            {

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                // row 的 Tag 為 補考群組的系統編號
                row.Tag = groupRecord.UID;

                row.Cells[0].Value = groupRecord.MakeUp_Group;

                if (groupRecord.New_Ref_Teacher_ID != null)
                {
                    K12.Data.TeacherRecord tr = _teacherList.Find(t => t.ID == groupRecord.New_Ref_Teacher_ID);

                    if (tr != null)
                    {
                        if (!string.IsNullOrEmpty(tr.Nickname))
                        {
                            row.Cells[1].Value = tr.Name + "(" + tr.Nickname + ")";
                        }
                        else
                        {
                            row.Cells[1].Value = tr.Name;
                        }
                    }
                    else
                    {
                        row.Cells[1].Value = "";
                    }
                }
                else
                {
                    K12.Data.TeacherRecord tr = _teacherList.Find(t => t.ID == groupRecord.Ref_Teacher_ID);

                    if (tr != null)
                    {
                        if (!string.IsNullOrEmpty(tr.Nickname))
                        {
                            row.Cells[1].Value = tr.Name + "(" + tr.Nickname + ")";
                        }
                        else
                        {
                            row.Cells[1].Value = tr.Name;
                        }
                    }
                    else
                    {
                        row.Cells[1].Value = "";
                    }
                }

                row.Cells[2].Value = groupRecord.StudentCount;

                row.Cells[3].Value = groupRecord.Description;

                // 只要有一筆 改變， 就顯示 『未儲存』
                if (groupRecord.IsDirty)
                {
                    lblIsDirty.Visible = true;
                }

                // 假如 補考群組 是準備要刪除、合併的 就不顯示了
                if (groupRecord.Action == "刪除" || groupRecord.Action == "合併")
                {
                    continue;
                }

                dataGridViewX1.Rows.Add(row);

            }

        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {

            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet ws = book.Worksheets[book.Worksheets.Add()];
            ws.Name = _selectedBatch.MakeUp_Batch;

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
            sd.FileName = "高中補考群組資料匯出";
            sd.Filter = "Excel檔案(*.xlsx)|*.xlsx";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                DialogResult result = new DialogResult();

                try
                {
                    book.Save(sd.FileName, SaveFormat.Xlsx);
                    result = FISCA.Presentation.Controls.MsgBox.Show("檔案儲存完成，是否開啟檔案?", "是否開啟", MessageBoxButtons.YesNo);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("儲存失敗。" + ex.Message);
                }

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                    catch (Exception ex)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("開啟檔案發生失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }



            }


        }
    }
}

