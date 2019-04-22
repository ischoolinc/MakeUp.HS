using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MakeUp.HS
{
    [FISCA.UDT.TableName("make.up.batch")]
    public class UDT_MakeUpData : FISCA.UDT.ActiveRecord
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
        /// 梯次敘述
        /// </summary>
        [FISCA.UDT.Field]
        public string Description { get; set; }

        /// <summary>
        /// 包含班級id
        /// </summary>
        [FISCA.UDT.Field]
        public string Included_Class_ID { get; set; }
        
    }
}
