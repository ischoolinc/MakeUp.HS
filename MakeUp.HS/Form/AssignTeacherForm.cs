using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using K12.Data;


namespace MakeUp.HS.Form
{
    public partial class AssignTeacherForm : FISCA.Presentation.Controls.BaseForm
    {

        // 教師清單
        private List<K12.Data.TeacherRecord> _teacherList;

        public string assignteacherID;

        public AssignTeacherForm()
        {
            InitializeComponent();

            _teacherList = K12.Data.Teacher.SelectAll();

            // 老師 依教師姓名排序
            _teacherList.Sort((x, y) => { return x.Name.CompareTo(y.Name); });

            //將教師加入清單
            foreach (K12.Data.TeacherRecord teacher in _teacherList)
            {
                // 老師全名 
                cboTeacher.Items.Add(teacher.Name + "(" + teacher.Nickname + ")");
            }

            // 預設選第一個老師
            //cboTeacher.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            // 假如沒有選擇老師，其ID 為空白
            if (cboTeacher.Text == "")
            {
                assignteacherID = "";
            }
            else
            {
                assignteacherID = _teacherList.Find(t => (t.Name + "(" + t.Nickname + ")" == cboTeacher.Text)).ID;
            }
            
            this.DialogResult = DialogResult.OK;            
        }
    }
}
