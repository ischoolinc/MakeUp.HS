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
        /// 參考學生系統編號
        /// </summary>
        [FISCA.UDT.Field]
        public string Ref_Student_ID { get; set; }

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
        /// 校部定
        /// </summary>
        [FISCA.UDT.Field]
        public string C_Is_Required_By { get; set; }

        /// <summary>
        /// 必修
        /// </summary>
        [FISCA.UDT.Field]
        public string C_Is_Required { get; set; }

        /// <summary>
        /// 成績分數
        /// </summary>
        [FISCA.UDT.Field]
        public string Score { get; set; }

        /// <summary>
        /// 補考分數
        /// </summary>
        [FISCA.UDT.Field]
        public string MakeUp_Score { get; set; }

        /// <summary>
        /// 及格標準
        /// </summary>
        [FISCA.UDT.Field]
        public string Pass_Standard { get; set; }

        /// <summary>
        /// 補考標準
        /// </summary>
        [FISCA.UDT.Field]
        public string MakeUp_Standard { get; set; }


        /// <summary>
        /// 參考補考群組名稱(非UDT 欄位，此屬性為做資料使用)
        /// </summary>
        public string Ref_MakeUp_Group_Name { get; set; }

    }
}
