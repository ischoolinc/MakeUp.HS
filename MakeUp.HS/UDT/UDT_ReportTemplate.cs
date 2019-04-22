using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ESL_System
{    
    [FISCA.UDT.TableName("make.up.batch")]
    public class UDT_MakeUpBatch : FISCA.UDT.ActiveRecord
    {
        
        /// <summary>
        /// 參考的評分樣版 ID
        /// </summary>
        [FISCA.UDT.Field]
        public string Ref_exam_Template_ID { get; set; }

        /// <summary>
        /// 設定檔名稱
        /// </summary>
        [FISCA.UDT.Field]
        public string Name { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        [FISCA.UDT.Field]
        public string SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [FISCA.UDT.Field]
        public string Semester { get; set; }

        /// <summary>
        /// 試別
        /// </summary>
        [FISCA.UDT.Field]
        public string Exam { get; set; }

              
    }
}
