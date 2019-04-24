using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MakeUp.HS
{
    [FISCA.UDT.TableName("make.up.data")]
    public class UDT_MakeUpData : FISCA.UDT.ActiveRecord
    {

        /// <summary>
        /// 參考補考梯次編號
        /// </summary>
        [FISCA.UDT.Field]
        public string Ref_MakeUp_Batch_ID { get; set; }

        /// <summary>
        /// 參考補考群組編號
        /// </summary>
        [FISCA.UDT.Field]
        public string Ref_MakeUp_Group_ID { get; set; }


        /// <summary>
        /// 科目
        /// </summary>
        [FISCA.UDT.Field]
        public string Subject { get; set; }

        /// <summary>
        /// 科目級別
        /// </summary>
        [FISCA.UDT.Field]
        public string Level { get; set; }

        /// <summary>
        /// 學分
        /// </summary>
        [FISCA.UDT.Field]
        public string Credit { get; set; }

        /// <summary>
        /// 成績分數
        /// </summary>
        [FISCA.UDT.Field]
        public string Score { get; set; }

        /// <summary>
        /// 成績分數
        /// </summary>
        [FISCA.UDT.Field]
        public string MakeUp_score { get; set; }

        /// <summary>
        /// 補考通過標準
        /// </summary>
        [FISCA.UDT.Field]
        public string Pass_Standard { get; set; }

    }
}
