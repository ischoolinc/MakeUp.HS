using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeUp.HS.Form
{
    public partial class MakeUpBatchManagerForm_Group : FISCA.Presentation.Controls.BaseForm
    {

        // 選學分或學時
        private string SelectItem = "";

        // 學期，用來判斷學時只有學期2才可以使用
        private string Semester = "";

        public MakeUpBatchManagerForm_Group(string selSemester)
        {
            InitializeComponent();
            Semester = selSemester;
        }

        private void MakeUpBatchManagerForm_Group_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;

            // 學時制第二學期使用判斷
            if (Semester == "2")
                buttonX2.Enabled = true;
            else
                buttonX2.Enabled = false;
        }

        public string GetSelectItem()
        {
            return SelectItem;
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            SelectItem = "";
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            SelectItem = "學分";
            this.DialogResult = DialogResult.Yes;
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            SelectItem = "學時";
            this.DialogResult = DialogResult.Yes;
        }
    }
}
