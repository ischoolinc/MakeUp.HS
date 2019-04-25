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
using SmartSchool.Customization.Data.StudentExtension;
using SmartSchool.Evaluation;
using SmartSchool;
using FISCA.Presentation;


namespace MakeUp.HS.Form
{
    public partial class GenMakeUpGroupForm : FISCA.Presentation.Controls.BaseForm
    {
        private BackgroundWorker _worker;

        private List<string> _classIDList;

        private UDT_MakeUpBatch _batch;

        // 學年度
        private string _schoolYear;

        // 學期
        private string _semester;

        public GenMakeUpGroupForm(UDT_MakeUpBatch batch)
        {
            InitializeComponent();

            _batch = batch;

            // 解析 classID
            _batch.ParseClassXMLIDList();

            _classIDList = _batch.classIDList;

            _schoolYear = _batch.School_Year;

            _semester = _batch.Semester;

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;

            _worker.RunWorkerAsync();
        }


        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _worker.ReportProgress(0);

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

                // 填上學生的修課紀錄
                helper.StudentHelper.FillAttendCourse(int.Parse(_schoolYear), int.Parse(_semester), classStudents);

                _worker.ReportProgress((int)(currentClass++ * 60.0 / totalClasses));
            }

            double currentStudent = 1;
            double totalStudents = allStudents.Count;

            #endregion


            #region 取得課程資訊 (授課教師)

            List<K12.Data.CourseRecord> courseList = K12.Data.Course.SelectBySchoolYearAndSemester(int.Parse(_schoolYear), int.Parse(_semester));

            #endregion

            Dictionary<string, Dictionary<string, string>> subjectInfo = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, List<Dictionary<string, string>>> subjectStudentList = new Dictionary<string, List<Dictionary<string, string>>>();

            // 補考群組Dict <課程名稱 +"_"+ 科目() +"_"+ 級別()+"_"+ 學分() +"_"+ "課程系統編號()" , UDT_MakeUpGroup>
            Dictionary<string, UDT_MakeUpGroup> makeUpGroupDict = new Dictionary<string, UDT_MakeUpGroup>();

            // 補考資料 List 
            List<UDT_MakeUpData> UDT_MakeUpDataList = new List<UDT_MakeUpData>();

            // 整理 補考群組 補考資料
            foreach (StudentRecord aStudent in allStudents)
            {
                List<StudentAttendCourseRecord> scattendList = aStudent.AttendCourseList;

                string className = aStudent.RefClass.ClassName;
                string seatNo = aStudent.SeatNo;
                string studentName = aStudent.StudentName;
                string studentNumber = aStudent.StudentNumber;

                foreach (SemesterSubjectScoreInfo info in aStudent.SemesterSubjectScoreList)
                {
                    if ("" + info.SchoolYear == _schoolYear && "" + info.Semester == _semester && !info.Pass)
                    {
                        if (info.Detail.GetAttribute("達補考標準") == "是")
                        {
                            StudentAttendCourseRecord scaRecord = scattendList.Find(sc => sc.Subject == info.Subject && sc.SubjectLevel == info.Level && sc.Credit == info.CreditDec());

                            //string makeUpGroupKey = scaRecord.CourseName + "_科目(" + scaRecord.Subject + ")_級別(" + scaRecord.SubjectLevel + ")_學分(" + scaRecord.Credit + ")_課程系統編號(" + scaRecord.CourseID + ")";
                            string makeUpGroupKey = scaRecord.CourseName + "_課程系統編號(" + scaRecord.CourseID + ")";

                            if (!makeUpGroupDict.ContainsKey(makeUpGroupKey))
                            {
                                UDT_MakeUpGroup makeUpGroupRecord = new UDT_MakeUpGroup();

                                makeUpGroupRecord.MakeUp_Group = makeUpGroupKey;

                                makeUpGroupRecord.Description = "";

                                makeUpGroupRecord.Ref_MakeUp_Batch_ID = _batch.UID;

                                K12.Data.CourseRecord courseRecord = courseList.Find(cr => cr.ID == "" + scaRecord.CourseID);

                                // 授課教師 要另外對照出來(原本 smart school API 沒支援)，目前這邊會先抓教師一
                                makeUpGroupRecord.Ref_Teacher_ID = courseRecord != null ? courseRecord.MajorTeacherID : "";

                                makeUpGroupDict.Add(makeUpGroupKey, makeUpGroupRecord);
                            }

                            UDT_MakeUpData makeUpData = new UDT_MakeUpData();

                            // 參考補考梯次ID
                            makeUpData.Ref_MakeUp_Batch_ID = _batch.UID;

                            // 參考補考群組ID (這個時候還不會有 因為補考群組也正要新增)
                            //makeUpData.Ref_MakeUp_Group_ID = "";

                            makeUpData.Ref_MakeUp_Group_Name = makeUpGroupKey;

                            // 參考學生系統編號
                            makeUpData.Ref_Student_ID = aStudent.StudentID;

                            // 科目
                            makeUpData.Subject = info.Subject;

                            // 級別
                            makeUpData.Level = info.Level;

                            // 學分
                            makeUpData.Credit = "" + info.CreditDec();

                            // 原始成績
                            makeUpData.Score = info.Detail.HasAttribute("原始成績") ? info.Detail.GetAttribute("原始成績") : "";

                            // 補考標準
                            makeUpData.Pass_Standard = info.Detail.HasAttribute("補考標準") ? info.Detail.GetAttribute("補考標準") : "";

                            UDT_MakeUpDataList.Add(makeUpData);
                        }
                    }
                }
                _worker.ReportProgress(60 + (int)(currentStudent++ * 20.0 / totalStudents));
            }


            // LOG 資訊
            string _actor = DSAServices.UserAccount; ;
            string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            //  拚 SQL
            List<string> makeUpGroupDataList = new List<string>();

            foreach (string groupKey in makeUpGroupDict.Keys)
            {

                UDT_MakeUpGroup makeUpGroupRecord = makeUpGroupDict[groupKey];

                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS ref_makeup_batch_id
                    ,'{1}'::TEXT AS makeup_group
                    ,'{2}'::TEXT AS ref_teacher_id
                    ,'{3}'::TEXT AS description                                   
                ", makeUpGroupRecord.Ref_MakeUp_Batch_ID, makeUpGroupRecord.MakeUp_Group, makeUpGroupRecord.Ref_Teacher_ID, makeUpGroupRecord.Description);

                makeUpGroupDataList.Add(data);
            }

            string makeUpGroupDataString = string.Join(" UNION ALL", makeUpGroupDataList);

            List<string> UDT_MakeUpDataDataList = new List<string>();


            foreach (UDT_MakeUpData makeUpData in UDT_MakeUpDataList)
            {
                string data = string.Format(@"
                SELECT
                    '{0}'::TEXT AS ref_makeup_batch_id
                    ,'{1}'::TEXT AS ref_makeup_group_name
                    ,'{2}'::TEXT AS ref_student_id
                    ,'{3}'::TEXT AS subject                                   
                    ,'{4}'::TEXT AS level                                   
                    ,'{5}'::TEXT AS credit                                   
                    ,'{3}'::TEXT AS score                                   
                    ,'{3}'::TEXT AS makeup_score                                   
                    ,'{3}'::TEXT AS pass_standard                                                                              
                ", makeUpData.Ref_MakeUp_Batch_ID, makeUpData.Ref_MakeUp_Group_Name, makeUpData.Ref_Student_ID, makeUpData.Subject, makeUpData.Level, makeUpData.Credit, makeUpData.Score, makeUpData.MakeUp_score, makeUpData.Pass_Standard);

                UDT_MakeUpDataDataList.Add(data);
            }

            string makeUpDataDataString = string.Join(" UNION ALL", UDT_MakeUpDataDataList);


            string sql = string.Format(@"
WITH makeUpGroupData_row AS(			 
                {0}     
)
,insert_data AS (
INSERT INTO $make.up.group(
	ref_makeup_batch_id	
	,makeup_group
    ,ref_teacher_id
	,description	
)
SELECT 
	makeUpGroupData_row.ref_makeup_batch_id::TEXT AS ref_makeup_batch_id	
	,makeUpGroupData_row.makeup_group::TEXT AS makeup_group	
    ,makeUpGroupData_row.ref_teacher_id::TEXT AS ref_teacher_id	
	,makeUpGroupData_row.description::TEXT AS description		
FROM
	makeUpGroupData_row
--RETURNING $make.up.group.*
)
SELECT 0
", makeUpGroupDataString, _actor, _client_info);


            K12.Data.UpdateHelper uh = new K12.Data.UpdateHelper();

            //執行sql
            uh.Execute(sql);

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MotherForm.SetStatusBarMessage(string.Format("補考梯次:{0} 補考群組 產生完成", _batch.MakeUp_Batch));

            this.Close();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage(string.Format("補考梯次:{0} 補考群組 產生中", _batch.MakeUp_Batch), e.ProgressPercentage);
        }


    }
}

