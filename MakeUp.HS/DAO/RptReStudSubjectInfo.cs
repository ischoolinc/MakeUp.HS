using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeUp.HS.DAO
{
    public class RptReStudSubjectInfo
    {
        // 學生系統編號
        public string StudentID { get; set; }

        // 班級
        public string ClassName { get; set; }

        // 座號
        public string SeatNo { get; set; }

        // 學號 
        public string StudentNumber { get; set; }

        // 姓名
        public string StudentName { get; set; }

        // 科目名稱
        public string SubjectName { get; set; }

        // 學分
        public decimal Credit { get; set; }

        // 成績
        public decimal Score { get; set; }

        // 班級ID
        public string ClassID { get; set; }

        // 學年成績
        public YearSubjectInfo YSScore { get; set; }
    }
}
