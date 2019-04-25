using System.Xml;
using System.Data;
using FISCA.Data;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using FISCA.Presentation.Controls;
using FISCA.Authentication;
using FISCA.LogAgent;
using SmartSchool.Customization.Data;
using SmartSchool.Evaluation;

namespace MakeUp.HS.Form
{
    public partial class GenMakeUpGroupForm : FISCA.Presentation.Controls.BaseForm
    {
        private BackgroundWorker _worker;

        private List<string> _classIDList;

        public GenMakeUpGroupForm(UDT_MakeUpBatch batch)
        {
            InitializeComponent();

            // 解析 classID
            batch.ParseClassXMLIDList();

            _classIDList = batch.classIDList;

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;

            _worker.RunWorkerAsync();
        }


        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            #region 取得所有學生以及補考資訊

            AccessHelper helper = new AccessHelper();
            List<StudentRecord> allStudents = new List<StudentRecord>();
            List<ClassRecord> allClasses = helper.ClassHelper.GetClass(_classIDList);
            WearyDogComputer computer = new WearyDogComputer();

            double currentClass = 1;
            double totalClasses = allClasses.Count;

            foreach (ClassRecord aClass in allClasses)
            {
                List<StudentRecord> classStudents = aClass.Students;
                computer.FillSemesterSubjectScoreInfoWithResit(helper, true, classStudents);
                allStudents.AddRange(classStudents);                
            }

            double currentStudent = 1;
            double totalStudents = allStudents.Count;

            #endregion

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState, e.ProgressPercentage);
        }


    }
}

