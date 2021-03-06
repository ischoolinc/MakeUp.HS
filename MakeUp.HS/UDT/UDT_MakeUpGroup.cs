﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MakeUp.HS
{
    [FISCA.UDT.TableName("make.up.group")]
    public class UDT_MakeUpGroup : FISCA.UDT.ActiveRecord
    {

        /// <summary>
        /// 參考補考梯次編號
        /// </summary>
        [FISCA.UDT.Field]
        public string Ref_MakeUp_Batch_ID { get; set; }


        /// <summary>
        /// 補考群組
        /// </summary>
        [FISCA.UDT.Field]
        public string MakeUp_Group { get; set; }

        /// <summary>
        /// 閱卷老師
        /// </summary>
        [FISCA.UDT.Field]
        public string Ref_Teacher_ID { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [FISCA.UDT.Field]
        public string Description { get; set; }

        /// <summary>
        /// 補考日期
        /// </summary>
        [FISCA.UDT.Field]
        public string MakeUp_Date { get; set; }

        /// <summary>
        /// 補考時間
        /// </summary>
        [FISCA.UDT.Field]
        public string MakeUp_Time { get; set; }

        /// <summary>
        /// 補考場地
        /// </summary>
        [FISCA.UDT.Field]
        public string MakeUp_Place { get; set; }

        /// <summary>
        /// 取代原本 ActiveRecord 不能 Set 的 UID
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// 本群組　補考學生數　總計
        /// </summary>        
        public string StudentCount { get; set; }

        /// <summary>
        /// 本群組　於介面編輯的處理(新增、刪除、修改、合併)
        /// </summary>        
        public string Action { get; set; }


        /// <summary>
        /// 本群組　是否為第一個選擇的Row(合併其他Row)
        /// </summary>        
        public bool IsFirstSelectedRow { get; set; }


        /// <summary>
        /// 本群組　是否為有改變(判斷更新使用)
        /// </summary>        
        public bool IsDirty { get; set; }



        /// <summary>
        /// 新閱卷老師(新手動指定的)
        /// </summary>        
        public string New_Ref_Teacher_ID { get; set; }

        /// <summary>
        /// 閱卷老師全名
        /// </summary>        
        public string TeacherName { get; set; }


        /// <summary>
        /// 合併後新群組ID
        /// </summary>        
        public string New_Merge_Group_ID { get; set; }

        /// <summary>
        /// 合併後新群組名稱
        /// </summary>        
        public string New_Merge_Group_Name { get; set; }


        /// <summary>
        /// 本群組 所屬的補考梯次
        /// </summary>        
        public UDT_MakeUpBatch MakeUpBatch { get; set; }


    }
}
