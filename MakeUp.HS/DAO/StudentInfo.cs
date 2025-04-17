using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeUp.HS.DAO
{
    public class StudentInfo
    {
        // 學生系統編號
        public string StudentID { get; set; }

        public string ClassID { get; set; }

        // 學生姓名
        public string StudentName { get; set; }

        // 學號
        public string StudentNumber { get; set; }

        // 年級
        public int GradeYear { get; set; }

        // 班級
        public string ClassName { get; set; }

        // 座號
        public string SeatNo { get; set; }

        // 不及格規則
        public List<string> FailRuleList = new List<string>();

        // 及格標準
        public decimal PassStandard { get; set; }

        // 學年科目成績不及格
        public List<YearSubjectInfo> YearSubjectScoreList = new List<YearSubjectInfo>();

        // 學年學期科目成績對照
        public Dictionary<string, List<SemsSubjectInfo>> YearSemsSubjectScoreDict = new Dictionary<string, List<SemsSubjectInfo>>();

        // 學年學業總平均
        public decimal SchoolYearEntryScore { get; set; }

        // 不及格科目數
        public int FailSubjectCount { get; set; }

        // 科目總數
        public int SubjectCount { get; set; }

        // 不及格學分數
        public decimal FailCreditCount { get; set; }

        // 科目總學分數
        public decimal CreditCount { get; set; }

        // 學年科目0分數
        public List<YearSubjectInfo> YearSubjectZeroScore = new List<YearSubjectInfo>();

        // 學年科目是否不及格科目數大於1/2
        public bool IsFailSubjectCountOverHalf { get; set; }

        // 學年科目是否不及格科目節數大於1/2
        public bool IsFailCreditCountOverHalf { get; set; }

        // 學年分項成績是否不及格
        public bool IsFailEntryScore { get; set; }

    }
}
