using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeUp.HS.DAO
{
    public class YearSubjectInfo
    {
        // 科目名稱
        public string SubjectName { get; set; }
        // 成績
        public decimal Score { get; set; }
        // 節數
        public decimal Credit { get; set; }

        // 是否及格
        public bool IsPass { get; set; }

        // 及格標準
        public decimal? PassStandard { get; set; }

        // 補考標準
        public decimal? ReStandard { get; set; }

        // 修課校部訂
        public string RequiredBy { get; set; }

        // 修課必選修
        public string Required { get; set; }

        // 原始成績
        public decimal? SScore { get; set; }

        // 補考成績
        public decimal? ReScore { get; set; }
    }
}
