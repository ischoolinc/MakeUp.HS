using FISCA.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MakeUp.HS.DAO;
using SmartSchool.Customization.Data.StudentExtension;
using SmartSchool.Customization.Data;
using FISCA.Authentication;
using FISCA.LogAgent;
using FISCA.Presentation.Controls;

namespace MakeUp.HS.Form
{
    public partial class GenMakeUpGroupForm_N : FISCA.Presentation.Controls.BaseForm
    {
        private BackgroundWorker _worker;

        private List<string> _classIDList;

        private UDT_MakeUpBatch _batch;

        Dictionary<string, StudentInfo> StudentReExamReportDict;

        // 學年度
        private string _schoolYear;
        int SelectSchoolYear = 0;
        // 學期
        private string _semester;

        bool isRunSQL = false;


        public GenMakeUpGroupForm_N(UDT_MakeUpBatch batch)
        {
            InitializeComponent();

            // 解析 classID
            _batch = batch;
            _batch.ParseClassXMLIDList();
            _classIDList = _batch.classIDList;

            _schoolYear = _batch.School_Year;

            _semester = _batch.Semester;

            StudentReExamReportDict = new Dictionary<string, StudentInfo>();

            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.ProgressChanged += _worker_ProgressChanged;
            _worker.WorkerReportsProgress = true;
            _worker.RunWorkerAsync();
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage(string.Format("補考梯次:{0} 補考群組 產生中", _batch.MakeUp_Batch), e.ProgressPercentage);
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isRunSQL == false)
            {
                MsgBox.Show("本梯次班級沒有任何學生需要補考，故本梯次沒有任何補考群組。", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            MotherForm.SetStatusBarMessage(string.Format("補考梯次:{0} 補考群組 產生完成", _batch.MakeUp_Batch));

            this.Close();
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _worker.ReportProgress(1);

            StudentReExamReportDict.Clear();

            // 轉換學年度
            int.TryParse(_schoolYear, out SelectSchoolYear);


            // 透過班級ID取得學生ID
            List<string> StudentIDList = DataAccess.GetStudentIDByClassIDs(_classIDList);
            // 取得學生基本資料            
            AccessHelper accessHelper = new AccessHelper();
            List<StudentRecord> StudRecList = accessHelper.StudentHelper.GetStudents(StudentIDList);

            // 學年分項成績
            accessHelper.StudentHelper.FillSchoolYearEntryScore(true, StudRecList);

            // 學年科目成績
            accessHelper.StudentHelper.FillSchoolYearSubjectScore(true, StudRecList);

            // 學期科目成績
            accessHelper.StudentHelper.FillSemesterSubjectScore(true, StudRecList);

            _worker.ReportProgress(30);

            // 給取得學年科目總科目數與總節數使用
            List<YearSubjectInfo> tmpYearSubjectScoreList = new List<YearSubjectInfo>();
            Dictionary<string, List<SemsSubjectInfo>> tmpYearSemsSubjectScoreDict = new Dictionary<string, List<SemsSubjectInfo>>();

            // 整理資料
            foreach (StudentRecord studRec in StudRecList)
            {
                // 取得學生學號、班級、座號、姓名、年級
                StudentInfo si = new StudentInfo();
                si.StudentID = studRec.StudentID;
                si.StudentNumber = studRec.StudentNumber;
                si.SeatNo = studRec.SeatNo;
                si.StudentName = studRec.StudentName;

                if (studRec.RefClass != null)
                {
                    si.ClassName = studRec.RefClass.ClassName;
                    int GradeYear = 0;
                    int.TryParse(studRec.RefClass.GradeYear, out GradeYear);

                    si.GradeYear = GradeYear;
                    si.ClassID = studRec.RefClass.ClassID;

                    // 整理學生目前年級的相關學期、學年成績
                    foreach (SchoolYearEntryScoreInfo score in studRec.SchoolYearEntryScoreList)
                    {
                        //  if (score.GradeYear == GradeYear && score.Entry == "學業")
                        if (score.SchoolYear == SelectSchoolYear && score.Entry == "學業")
                            si.SchoolYearEntryScore = score.Score;
                    }

                    // 取得學年科目總科目數與總節數
                    tmpYearSubjectScoreList.Clear();
                    tmpYearSemsSubjectScoreDict.Clear();

                    // 學年科目成績
                    foreach (SchoolYearSubjectScoreInfo score in studRec.SchoolYearSubjectScoreList)
                    {
                        //if (score.GradeYear == GradeYear)
                        if (score.SchoolYear == SelectSchoolYear)
                        {
                            YearSubjectInfo ys = new YearSubjectInfo();
                            ys.SubjectName = score.Subject; // 科目名稱
                            ys.Score = score.Score; // 科目成績
                            decimal sscore, rescore;
                            if (decimal.TryParse(score.Detail.GetAttribute("結算成績"), out sscore))
                            {
                                ys.SScore = sscore;
                            }

                            if (decimal.TryParse(score.Detail.GetAttribute("補考成績"), out rescore))
                            {
                                ys.ReScore = rescore;
                            }

                            if (!tmpYearSemsSubjectScoreDict.ContainsKey(ys.SubjectName))
                                tmpYearSemsSubjectScoreDict.Add(ys.SubjectName, new List<SemsSubjectInfo>());

                            // 建立學年科目相關資料
                            tmpYearSubjectScoreList.Add(ys);
                        }
                    }

                    // 學期科目成績
                    foreach (SemesterSubjectScoreInfo_New score in studRec.SemesterSubjectScoreList)
                    {
                        //if (score.GradeYear == GradeYear)
                        if (score.SchoolYear == SelectSchoolYear)
                        {
                            //Console.WriteLine(score.Detail);
                            decimal passScore = 0;

                            // 取得學期科目成績
                            if (tmpYearSemsSubjectScoreDict.ContainsKey(score.Subject))
                            {
                                SemsSubjectInfo ss = new SemsSubjectInfo();
                                ss.Semester = score.Semester;
                                ss.SubjectName = score.Subject;
                                ss.Score = score.Score;
                                ss.Credit = score.CreditDec;
                                if (decimal.TryParse(score.Detail.GetAttribute("修課及格標準"), out passScore))
                                {

                                    ss.PassStandard = passScore;
                                }

                                ss.Required = score.Detail.GetAttribute("修課必選修");
                                ss.RequiredBy = score.Detail.GetAttribute("修課校部訂");
                                decimal rescoreS;
                                if (decimal.TryParse(score.Detail.GetAttribute("修課補考標準"), out rescoreS))
                                {
                                    ss.ReStandard = rescoreS;
                                }

                                tmpYearSemsSubjectScoreDict[score.Subject].Add(ss);
                            }

                            si.PassStandard = passScore;
                        }
                    }

                    //  資料排序
                    foreach (string name in tmpYearSemsSubjectScoreDict.Keys)
                    {
                        tmpYearSemsSubjectScoreDict[name].Sort((x, y) => x.Semester.CompareTo(y.Semester));
                    }

                    // 處理學年科目成績，是否取得與學分數
                    foreach (YearSubjectInfo ys in tmpYearSubjectScoreList)
                    {
                        if (tmpYearSemsSubjectScoreDict.ContainsKey(ys.SubjectName))
                        {
                            //if (ys.Score >= si.PassStandard)
                            //{
                            //    ys.IsPass = true;
                            //}
                            //else
                            //    ys.IsPass = false;

                            ys.Credit = 0;
                            foreach (SemsSubjectInfo ss in tmpYearSemsSubjectScoreDict[ys.SubjectName])
                            {
                                ys.Credit += ss.Credit;
                                if (ss.PassStandard.HasValue)
                                    ys.PassStandard = ss.PassStandard;

                                if (ss.ReStandard.HasValue)
                                    ys.ReStandard = ss.ReStandard;

                                if (ss.Required != "")
                                    ys.Required = ss.Required;

                                if (ss.RequiredBy != "")
                                    ys.RequiredBy = ss.RequiredBy;
                                
                                if (ys.RequiredBy == "部訂")
                                    ys.RequiredBy = "部定";
                            }

                            if (ys.Score >= ys.PassStandard)
                            {
                                ys.IsPass = true;
                            }
                            else
                                ys.IsPass = false;

                        }
                    }

                    // 設定初始化值
                    si.FailSubjectCount = si.SubjectCount = 0;
                    si.FailCreditCount = si.CreditCount = 0;
                    si.IsFailSubjectCountOverHalf = si.IsFailCreditCountOverHalf = si.IsFailEntryScore = false;

                    // 處理取得學年科目總科目數與總節數
                    foreach (YearSubjectInfo ys in tmpYearSubjectScoreList)
                    {
                        if (ys.IsPass == false)
                        {
                            // 學年科目不及格科目數與節數
                            si.FailSubjectCount++;
                            si.FailCreditCount += ys.Credit;

                            // 取得學年科目不及格與學期科目成績
                            si.YearSubjectScoreList.Add(ys);
                            if (tmpYearSemsSubjectScoreDict.ContainsKey(ys.SubjectName))
                            {
                                if (!si.YearSemsSubjectScoreDict.ContainsKey(ys.SubjectName))
                                    si.YearSemsSubjectScoreDict.Add(ys.SubjectName, new List<SemsSubjectInfo>());

                                foreach (SemsSubjectInfo ss in tmpYearSemsSubjectScoreDict[ys.SubjectName])
                                {
                                    si.YearSemsSubjectScoreDict[ys.SubjectName].Add(ss);
                                }
                            }
                        }

                        // 總科目數
                        si.SubjectCount++;
                        // 總科目節數
                        si.CreditCount += ys.Credit;

                        // 學年科目0分
                        if (ys.Score == 0)
                            si.YearSubjectZeroScore.Add(ys);
                    }

                    // 1. 學年學業總平均不及格。
                    if (si.SchoolYearEntryScore < si.PassStandard)
                    {
                        si.IsFailEntryScore = true;
                        si.FailRuleList.Add("學年學業總平均不及格");
                    }


                    // 2. 學年科目不及格數大於1/2。
                    if (si.FailSubjectCount * 2 > si.SubjectCount)
                    {
                        si.IsFailSubjectCountOverHalf = true;
                        si.FailRuleList.Add("學年科目不及格科目數大於總科目數1/2");
                    }


                    // 3. 學年科目不及格學分數大於1/2。
                    if (si.FailCreditCount * 2 > si.CreditCount)
                    {
                        si.IsFailCreditCountOverHalf = true;
                        si.FailRuleList.Add("學年科目不及格節數大於總節數1/2");
                    }


                    // 4. 學年科目有0分。
                    if (si.YearSubjectZeroScore.Count > 0)
                        si.FailRuleList.Add("學年科目有0分");

                }



                // 處理所有學年不及格就可以補考的學生(有不及格科目，且沒有學年科目0分)
                if (si.FailSubjectCount > 0 && si.YearSubjectZeroScore.Count == 0)
                {
                    if (!StudentReExamReportDict.ContainsKey(si.StudentID))
                        StudentReExamReportDict.Add(si.StudentID, si);
                }
            }

            _worker.ReportProgress(70);




            // 補考學生依班級座號排序後學生，學年科目成績有0分不能不考
            List<StudentInfo> StudentReportList2 = (from x in StudentReExamReportDict.Values orderby x.ClassName ascending, x.SeatNo ascending select x).ToList();


            // 整理分科目需要資料
            List<RptReStudSubjectInfo> RptStudSubjectList = new List<RptReStudSubjectInfo>();
            foreach (StudentInfo si in StudentReportList2)
            {
                foreach (YearSubjectInfo ys in si.YearSubjectScoreList)
                {
                    RptReStudSubjectInfo rs = new RptReStudSubjectInfo();
                    rs.ClassName = si.ClassName;
                    rs.ClassID = si.ClassID;
                    rs.SeatNo = si.SeatNo;
                    rs.StudentNumber = si.StudentNumber;
                    rs.StudentName = si.StudentName;
                    rs.SubjectName = ys.SubjectName;
                    rs.Credit = ys.Credit;
                    rs.Score = ys.Score;
                    rs.StudentID = si.StudentID;
                    rs.YSScore = ys;
                    RptStudSubjectList.Add(rs);
                }
            }

            // 分科目
            Dictionary<string, List<RptReStudSubjectInfo>> RptReStudSubjectYearDict = new Dictionary<string, List<RptReStudSubjectInfo>>();

            foreach (RptReStudSubjectInfo rs in RptStudSubjectList)
            {
                string key = rs.ClassID + "_" + rs.SubjectName;

                if (!RptReStudSubjectYearDict.ContainsKey(key))
                    RptReStudSubjectYearDict.Add(key, new List<RptReStudSubjectInfo>());
                RptReStudSubjectYearDict[key].Add(rs);
            }

            // 整理資料
            Dictionary<string, UDT_MakeUpGroup> makeUpGroupDict = new Dictionary<string, UDT_MakeUpGroup>();

            // 補考資料 List 
            List<UDT_MakeUpData> UDT_MakeUpDataList = new List<UDT_MakeUpData>();

            // 班級科目教師id
            Dictionary<string, string> ClassSubjectTeacherIDDict = DataAccess.GetCourseTeacherIDBySchoolYear(_schoolYear);

            foreach (string classID in RptReStudSubjectYearDict.Keys)
            {

                foreach (RptReStudSubjectInfo data in RptReStudSubjectYearDict[classID])
                {
                    string makeUpGroupKey = data.ClassName + "_" + data.SubjectName;
                    string tKey = data.ClassID + "_" + data.SubjectName;


                    if (!makeUpGroupDict.ContainsKey(makeUpGroupKey))
                    {
                        UDT_MakeUpGroup makeUpGroupRecord = new UDT_MakeUpGroup();

                        makeUpGroupRecord.MakeUp_Group = makeUpGroupKey;

                        makeUpGroupRecord.Description = "";

                        makeUpGroupRecord.MakeUp_Date = "";

                        makeUpGroupRecord.MakeUp_Time = "";

                        makeUpGroupRecord.MakeUp_Place = "";

                        // 教師id
                        makeUpGroupRecord.Ref_Teacher_ID = "";
                        if (ClassSubjectTeacherIDDict.ContainsKey(tKey))
                            makeUpGroupRecord.Ref_Teacher_ID = ClassSubjectTeacherIDDict[tKey];

                        makeUpGroupRecord.Ref_MakeUp_Batch_ID = _batch.UID;
                        makeUpGroupDict.Add(makeUpGroupKey, makeUpGroupRecord);
                    }

                    UDT_MakeUpData makeUpData = new UDT_MakeUpData();
                    // 參考補考梯次ID
                    makeUpData.Ref_MakeUp_Batch_ID = _batch.UID;

                    // 參考補考群組ID (這個時候還不會有 因為補考群組也正要新增)
                    //makeUpData.Ref_MakeUp_Group_ID = "";

                    makeUpData.Ref_MakeUp_Group_Name = makeUpGroupKey;

                    // 參考學生系統編號
                    makeUpData.Ref_Student_ID = data.StudentID;

                    // 科別
                    makeUpData.Department = "";

                    // 科目
                    makeUpData.Subject = data.SubjectName;

                    // 級別
                    makeUpData.Level = "";

                    // 學分
                    makeUpData.Credit = data.Credit + "";

                    // 校部定
                    makeUpData.C_Is_Required_By = data.YSScore.RequiredBy;

                    // 必選修
                    makeUpData.C_Is_Required = data.YSScore.Required;

                    // 原始成績
                    makeUpData.Score = "";
                    if (data.YSScore.SScore.HasValue)
                        makeUpData.Score = data.YSScore.SScore.Value + "";

                    // 補考成績
                    makeUpData.MakeUp_Score = "";
                    if (data.YSScore.ReScore.HasValue)
                        makeUpData.MakeUp_Score = data.YSScore.ReScore.Value + "";

                    // 及格標準
                    makeUpData.Pass_Standard = "";
                    if (data.YSScore.PassStandard.HasValue)
                        makeUpData.Pass_Standard = data.YSScore.PassStandard.Value + "";

                    // 補考標準
                    makeUpData.MakeUp_Standard = "";
                    if (data.YSScore.ReStandard.HasValue)
                        makeUpData.MakeUp_Standard = data.YSScore.ReStandard.Value + "";

                    //// 輸入小數位數限制
                    //makeUpData.DecimalNumber = info.Detail.HasAttribute("位數限制") ? info.Detail.GetAttribute("位數限制") : "";

                    //// 成績身分
                    //makeUpData.CalRole = info.Detail.HasAttribute("成績身分") ? info.Detail.GetAttribute("成績身分") : "";

                    UDT_MakeUpDataList.Add(makeUpData);

                }
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
                    ,'{4}'::TEXT AS makeup_date    
                    ,'{5}'::TEXT AS makeup_time
                    ,'{6}'::TEXT AS makeup_place
                ", makeUpGroupRecord.Ref_MakeUp_Batch_ID, makeUpGroupRecord.MakeUp_Group, makeUpGroupRecord.Ref_Teacher_ID, makeUpGroupRecord.Description, makeUpGroupRecord.MakeUp_Date, makeUpGroupRecord.MakeUp_Time, makeUpGroupRecord.MakeUp_Place);

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
                    ,'{6}'::TEXT AS c_is_required_by                                   
                    ,'{7}'::TEXT AS c_is_required                                   
                    ,'{8}'::TEXT AS score                                   
                    ,'{9}'::TEXT AS makeup_score                                   
                    ,'{10}'::TEXT AS pass_standard                                                                              
                    ,'{11}'::TEXT AS makeup_standard      
                    ,'{12}'::TEXT AS decimalnumber   
                    ,'{13}'::TEXT AS calrole  
                ", makeUpData.Ref_MakeUp_Batch_ID, makeUpData.Ref_MakeUp_Group_Name, makeUpData.Ref_Student_ID, makeUpData.Subject, makeUpData.Level, makeUpData.Credit, makeUpData.C_Is_Required_By, makeUpData.C_Is_Required, makeUpData.Score, makeUpData.MakeUp_Score, makeUpData.Pass_Standard, makeUpData.MakeUp_Standard, makeUpData.DecimalNumber, makeUpData.CalRole);

                UDT_MakeUpDataDataList.Add(data);
            }

            string makeUpDataDataString = string.Join(" UNION ALL", UDT_MakeUpDataDataList);


            string sql = string.Format(@"
WITH makeUpGroupData_row AS(			 
        {0}     
)
,makeUpDataData_row AS(
        {1}
)
,insert_makeUpGroupData AS (
INSERT INTO $make.up.group(
	ref_makeup_batch_id	
	,makeup_group
    ,ref_teacher_id
	,description	
    ,makeup_date	
    ,makeup_time	
    ,makeup_place	
)
SELECT 
	makeUpGroupData_row.ref_makeup_batch_id::TEXT AS ref_makeup_batch_id	
	,makeUpGroupData_row.makeup_group::TEXT AS makeup_group	
    ,makeUpGroupData_row.ref_teacher_id::TEXT AS ref_teacher_id	
	,makeUpGroupData_row.description::TEXT AS description		
    ,makeUpGroupData_row.makeup_date::TEXT AS makeup_date		
    ,makeUpGroupData_row.makeup_time::TEXT AS makeup_time		
    ,makeUpGroupData_row.makeup_place::TEXT AS makeup_place		
FROM
	makeUpGroupData_row
RETURNING $make.up.group.*
)
,insert_makeUpDataData AS(
INSERT INTO $make.up.data(
	ref_makeup_batch_id	
    ,ref_makeup_group_id
    ,ref_student_id
	,subject
    ,level
	,credit	
    ,c_is_required_by	
    ,c_is_required	
    ,score
    ,makeup_score
    ,pass_standard    
    ,makeup_standard
    ,decimalnumber
    ,calrole
)
SELECT 
	makeUpDataData_row.ref_makeup_batch_id::TEXT AS ref_makeup_batch_id	
	,insert_makeUpGroupData.uid::TEXT AS ref_makeup_group_id	
    ,makeUpDataData_row.ref_student_id::TEXT AS ref_student_id	
	,makeUpDataData_row.subject::TEXT AS subject		
    ,makeUpDataData_row.level::TEXT AS level    
    ,makeUpDataData_row.credit::TEXT AS credit
    ,makeUpDataData_row.c_is_required_by::TEXT AS c_is_required_by
    ,makeUpDataData_row.c_is_required::TEXT AS c_is_required
    ,makeUpDataData_row.score::TEXT AS score
    ,makeUpDataData_row.makeup_score::TEXT AS makeup_score
    ,makeUpDataData_row.pass_standard::TEXT AS pass_standard
    ,makeUpDataData_row.makeup_standard::TEXT AS makeup_standard
    ,makeUpDataData_row.decimalnumber::TEXT AS decimalnumber
    ,makeUpDataData_row.calrole::TEXT AS calrole
FROM
	makeUpDataData_row
LEFT JOIN insert_makeUpGroupData ON insert_makeUpGroupData.makeup_group = makeUpDataData_row.ref_makeup_group_name
)
,insert_makeUpGroupData_Log AS (
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
	'{2}'::TEXT AS actor
	, 'Record' AS action_type
	, '高中補考群組新增' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{3}' AS client_info
	, '高中補考群組新增'AS action_by   
	, ' 高中補考 學年度「'|| makeUpGroupData_row_Log.school_year||'」，學期「'|| makeUpGroupData_row_Log.semester||'」， 補考梯次「'|| makeUpGroupData_row_Log.makeup_batch||'」，新增補考群組 「'|| makeUpGroupData_row_Log.makeup_group ||'」，閱卷老師 「'|| COALESCE(makeUpGroupData_row_Log.teacher_name,'')  ||'」。' AS description 
FROM
(   
    SELECT
        $make.up.batch.school_year AS school_year
        ,$make.up.batch.semester AS semester
        ,$make.up.batch.makeup_batch AS makeup_batch
        ,makeUpGroupData_row.makeup_group AS makeup_group
        ,teacher.teacher_name AS teacher_name    
    FROM makeUpGroupData_row
    LEFT JOIN $make.up.batch ON $make.up.batch.uid :: TEXT = makeUpGroupData_row.ref_makeup_batch_id 
    LEFT JOIN teacher ON teacher.id :: TEXT = makeUpGroupData_row.ref_teacher_id 
) AS makeUpGroupData_row_Log
)
,insert_makeUpDataData_Log AS (
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
	'{2}'::TEXT AS actor
	, 'Record' AS action_type
	, '高中補考資料新增' AS action
	, ''::TEXT AS target_category
	, '' AS target_id
	, now() AS server_time
	, '{3}' AS client_info
	, '高中補考資料新增'AS action_by   
	, ' 高中補考 學年度「'|| makeUpDataData_row_Log.school_year||'」，學期「'|| makeUpDataData_row_Log.semester||'」， 補考梯次「'|| makeUpDataData_row_Log.makeup_batch||'」，補考群組 「'|| makeUpDataData_row_Log.makeup_group ||'」，閱卷老師 「'|| COALESCE(makeUpDataData_row_Log.teacher_name,'')  ||'」
    新增 補考資料 學生系統編號 「'|| makeUpDataData_row_Log.ref_student_id||'」 ，學號 「'|| makeUpDataData_row_Log.student_number||'」，學生姓名 「'|| makeUpDataData_row_Log.student_name||'」 
    ，科目 「'|| makeUpDataData_row_Log.subject||'」，級別 「'|| makeUpDataData_row_Log.level||'」，學分 「'|| makeUpDataData_row_Log.credit||'」，校部定 「'|| makeUpDataData_row_Log.c_is_required_by||'」，必選修 「'|| makeUpDataData_row_Log.c_is_required||'」      
    ，分數 「'|| makeUpDataData_row_Log.score||'」 ，補考分數 「'|| makeUpDataData_row_Log.makeup_score||'」 ，及格標準 「'|| makeUpDataData_row_Log.pass_standard||'」，補考標準 「'|| makeUpDataData_row_Log.makeup_standard||'」' AS description 
FROM
(   
    SELECT
        $make.up.batch.school_year AS school_year
        ,$make.up.batch.semester AS semester
        ,$make.up.batch.makeup_batch AS makeup_batch
        ,insert_makeUpGroupData.makeup_group AS makeup_group
        ,teacher.teacher_name AS teacher_name
        ,student.id AS ref_student_id
        ,student.student_number AS student_number
        ,student.name AS student_name
        ,makeUpDataData_row.subject AS subject
        ,makeUpDataData_row.level AS level
        ,makeUpDataData_row.credit AS credit
        ,makeUpDataData_row.credit AS c_is_required_by
        ,makeUpDataData_row.credit AS c_is_required
        ,makeUpDataData_row.score AS score
        ,makeUpDataData_row.makeup_score AS makeup_score
        ,makeUpDataData_row.pass_standard AS pass_standard
        ,makeUpDataData_row.makeup_standard AS makeup_standard
    FROM makeUpDataData_row
    LEFT JOIN insert_makeUpGroupData ON insert_makeUpGroupData.makeup_group = makeUpDataData_row.ref_makeup_group_name
    LEFT JOIN $make.up.batch ON $make.up.batch.uid :: TEXT = insert_makeUpGroupData.ref_makeup_batch_id 
    LEFT JOIN teacher ON teacher.id :: TEXT = insert_makeUpGroupData.ref_teacher_id 
    LEFT JOIN student ON student.id :: TEXT = makeUpDataData_row.ref_student_id 
) AS makeUpDataData_row_Log
)
SELECT 0


", makeUpGroupDataString, makeUpDataDataString, _actor, _client_info);

            FISCA.Data.QueryHelper qh = new FISCA.Data.QueryHelper();

            //執行sql
            isRunSQL = false;
            // 有補考群組才執行SQL
            if (makeUpGroupDataList.Count > 0)
            {
                isRunSQL = true;
                qh.Select(sql);
            }


            _worker.ReportProgress(100);
        }
    }
}
