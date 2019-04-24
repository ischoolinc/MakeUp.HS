using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MakeUp.HS
{
    [FISCA.UDT.TableName("make.up.batch")]
    public class UDT_MakeUpBatch : FISCA.UDT.ActiveRecord
    {

        /// <summary>
        /// 補考梯次
        /// </summary>
        [FISCA.UDT.Field]
        public string MakeUp_Batch { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        [FISCA.UDT.Field]
        public string School_Year { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [FISCA.UDT.Field]
        public string Semester { get; set; }

        /// <summary>
        /// 補考說明
        /// </summary>
        [FISCA.UDT.Field]
        public string Description { get; set; }

        /// <summary>
        /// 包含班級id
        /// </summary>
        [FISCA.UDT.Field]
        public string Included_Class_ID { get; set; }


        // 取代原本 ActiveRecord 不能 Set 的 UID
        public string UID { get; set; }

    }
}
