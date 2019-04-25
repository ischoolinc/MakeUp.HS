using System;
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

   
        
    }
}
