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
        /// 必選修
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


        // 取代原本 ActiveRecord 不能 Set 的 UID
        public string UID { get; set; }

        /// <summary>
        /// 參考補考群組名稱(非UDT 欄位，此屬性為做資料使用)
        /// </summary>
        public string Ref_MakeUp_Group_Name { get; set; }

        /// <summary>
        /// 學生姓名(非UDT 欄位，此屬性為UI介面資料使用)
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 科別
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 學生班級名稱(非UDT 欄位，此屬性為UI介面資料使用)
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 學生座號(非UDT 欄位，此屬性為UI介面資料使用)
        /// </summary>
        public string Seat_no { get; set; }

        /// <summary>
        /// 學生學號(非UDT 欄位，此屬性為UI介面資料使用)
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// 成績位數小數限制(非UDT 欄位，此屬性為管理補考成績輸入時的限制，對應學生 成績計算規則「學期科目成績小數位數」)
        /// </summary>
        public int DecimalNumber { get; set; }

        /// <summary>
        /// 有新輸入的補考成績(非UDT 欄位，此屬性為UI介面資料使用)
        /// </summary>
        public bool HasNewMakeUpScore { get; set; }


        /// <summary>
        /// 新輸入補考成績(非UDT 欄位，此屬性為UI介面資料使用)
        /// </summary>
        public string New_MakeUp_Score { get; set; }




    }
}
