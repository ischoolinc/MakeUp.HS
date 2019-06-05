﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MakeUp.HS
{
    // 2019/6/5 穎驊　新增此UDT 用來記錄高中補考的評分樣版
    [FISCA.UDT.TableName("makeup.hs.report.template")]
    public class UDT_ReportTemplate : FISCA.UDT.ActiveRecord
    {
        public UDT_ReportTemplate()
        {

        }

        /// <summary>
        /// 設定檔模式(依群組、依學生)
        /// </summary>
        [FISCA.UDT.Field]
        public string PrintMode { get; set; }
      
        /// <summary>
        /// 列印樣板
        /// </summary>
        [FISCA.UDT.Field]
        private string TemplateStream { get; set; }

        public Aspose.Words.Document Template { get; set; }
      
        /// <summary>
        /// 在儲存前，把資料填入儲存欄位中
        /// </summary>
        public void Encode()
        {           
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            this.Template.Save(stream, Aspose.Words.SaveFormat.Doc);
            this.TemplateStream = Convert.ToBase64String(stream.ToArray());
        }
        /// <summary>
        /// 在資料取出後，把資料從儲存欄位轉換至資料欄位
        /// </summary>
        public void Decode()
        {            
            this.Template = new Aspose.Words.Document(new MemoryStream(Convert.FromBase64String(this.TemplateStream)));
        }
    }
}
