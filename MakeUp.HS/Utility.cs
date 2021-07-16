using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using SmartSchool.Customization.Data;
using SmartSchool.Customization.Data.StudentExtension;
using FISCA.DSAUtil;
using FISCA.Data;
using System.Data;

namespace MakeUp.HS
{
    public class Utility
    {

        public void FillSemesterSubjectScoreInfoWithResit(AccessHelper accesshelper, bool filterRepeat, List<StudentRecord> students)
        {
            //抓科目成績
            accesshelper.StudentHelper.FillSemesterSubjectScore(filterRepeat, students);
            foreach (StudentRecord var in students)
            {

                //及格標準<年及,及格標準>
                Dictionary<int, decimal> applyLimit = new Dictionary<int, decimal>();

                //補考標準<年及,及格標準>
                Dictionary<int, decimal> resitLimit = new Dictionary<int, decimal>();

                // 預設兩位輸入限制
                string DecimalNumber = "2";

                // 學生的成績身分
                string calRole = "";


                // 2019/07/02 穎驊註解， 此高中補考判斷邏輯 是從 原高中補考系統搬過來
                // 研究其精神後， 其為擇優判斷， 會先以 "預設"類別的分數 當頂， 然後不斷下行尋找符合的類別， 假如其標準較低， 則將之當作新標準
                // 此缺點為，如果有類別 的標準分數設定 較 "預設" 高， 將無法被正確顯示，
                // 與佳樺討論過後，基本上這狀況比較不會發生， 因為 特殊生分的學生(體保、原住民、身心障礙)的計算標準 只會更低 頂多打平
                // 所以不會有問題， 

                #region 處理計算規則
                XmlElement scoreCalcRule = SmartSchool.Evaluation.ScoreCalcRule.ScoreCalcRule.Instance.GetStudentScoreCalcRuleInfo(var.StudentID) == null ? null : SmartSchool.Evaluation.ScoreCalcRule.ScoreCalcRule.Instance.GetStudentScoreCalcRuleInfo(var.StudentID).ScoreCalcRuleElement;
                if (scoreCalcRule == null)
                {
                }
                else
                {
                    DSXmlHelper helper = new DSXmlHelper(scoreCalcRule);
                    foreach (XmlElement element in helper.GetElements("及格標準/學生類別"))
                    {
                        string cat = element.GetAttribute("類別");
                        bool useful = false;
                        //掃描學生的類別作比對
                        foreach (CategoryInfo catinfo in var.StudentCategorys)
                        {
                            if (catinfo.Name == cat || catinfo.FullName == cat)
                                useful = true;
                        }
                        //學生是指定的類別或類別為"預設"
                        if (cat == "預設" || useful)
                        {
                            decimal tryParseDecimal;
                            for (int gyear = 1; gyear <= 4; gyear++)
                            {
                                switch (gyear)
                                {
                                    case 1:
                                        if (decimal.TryParse(element.GetAttribute("一年級及格標準"), out tryParseDecimal))
                                        {
                                            if (!applyLimit.ContainsKey(gyear))
                                                applyLimit.Add(gyear, tryParseDecimal);
                                            if (applyLimit[gyear] > tryParseDecimal)
                                                applyLimit[gyear] = tryParseDecimal;
                                        }
                                        if (decimal.TryParse(element.GetAttribute("一年級補考標準"), out tryParseDecimal))
                                        {
                                            if (!resitLimit.ContainsKey(gyear))
                                                resitLimit.Add(gyear, tryParseDecimal);
                                            if (resitLimit[gyear] > tryParseDecimal)
                                                resitLimit[gyear] = tryParseDecimal;
                                        }
                                        break;
                                    case 2:
                                        if (decimal.TryParse(element.GetAttribute("二年級及格標準"), out tryParseDecimal))
                                        {
                                            if (!applyLimit.ContainsKey(gyear))
                                                applyLimit.Add(gyear, tryParseDecimal);
                                            if (applyLimit[gyear] > tryParseDecimal)
                                                applyLimit[gyear] = tryParseDecimal;
                                        }
                                        if (decimal.TryParse(element.GetAttribute("二年級補考標準"), out tryParseDecimal))
                                        {
                                            if (!resitLimit.ContainsKey(gyear))
                                                resitLimit.Add(gyear, tryParseDecimal);
                                            if (resitLimit[gyear] > tryParseDecimal)
                                                resitLimit[gyear] = tryParseDecimal;
                                        }
                                        break;
                                    case 3:
                                        if (decimal.TryParse(element.GetAttribute("三年級及格標準"), out tryParseDecimal))
                                        {
                                            if (!applyLimit.ContainsKey(gyear))
                                                applyLimit.Add(gyear, tryParseDecimal);
                                            if (applyLimit[gyear] > tryParseDecimal)
                                                applyLimit[gyear] = tryParseDecimal;
                                        }
                                        if (decimal.TryParse(element.GetAttribute("三年級補考標準"), out tryParseDecimal))
                                        {
                                            if (!resitLimit.ContainsKey(gyear))
                                                resitLimit.Add(gyear, tryParseDecimal);
                                            if (resitLimit[gyear] > tryParseDecimal)
                                                resitLimit[gyear] = tryParseDecimal;
                                        }
                                        break;
                                    case 4:
                                        if (decimal.TryParse(element.GetAttribute("四年級及格標準"), out tryParseDecimal))
                                        {
                                            if (!applyLimit.ContainsKey(gyear))
                                                applyLimit.Add(gyear, tryParseDecimal);
                                            if (applyLimit[gyear] > tryParseDecimal)
                                                applyLimit[gyear] = tryParseDecimal;
                                        }
                                        if (decimal.TryParse(element.GetAttribute("四年級補考標準"), out tryParseDecimal))
                                        {
                                            if (!resitLimit.ContainsKey(gyear))
                                                resitLimit.Add(gyear, tryParseDecimal);
                                            if (resitLimit[gyear] > tryParseDecimal)
                                                resitLimit[gyear] = tryParseDecimal;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                            calRole = cat;
                        }
                    }


                    // 抓取該學生 成績計算規則 的 科目成績計算位數 
                    foreach (XmlElement element in helper.GetElements("各項成績計算位數/科目成績計算位數"))
                    {
                        DecimalNumber = element.GetAttribute("位數");
                    }
                }
                #endregion



                foreach (SemesterSubjectScoreInfo score in var.SemesterSubjectScoreList)
                {
                    bool canResit = false;
                    decimal s = 0;

                    // 補考標準
                    decimal makeUpStandard = 40;

                    // 及格標準
                    decimal passStandard = 60;

                    decimal mScore;
                    decimal pScore;

                    if (decimal.TryParse(score.Detail.GetAttribute("原始成績"), out s))
                    {
                        if (resitLimit.ContainsKey(score.GradeYear)) makeUpStandard = resitLimit[score.GradeYear];
                        canResit = (s >= makeUpStandard);
                    }

                    if (applyLimit.ContainsKey(score.GradeYear)) passStandard = applyLimit[score.GradeYear];

                    // 檢查如果學生學期科目上有設定，使用學期科目成績上設定
                    if (decimal.TryParse(score.Detail.GetAttribute("修課及格標準"), out pScore))
                    {
                        passStandard = pScore;
                    }

                    if (decimal.TryParse(score.Detail.GetAttribute("修課補考標準"), out mScore))
                    {
                        makeUpStandard = mScore;
                    }

                    if (s > 0)
                    {
                        canResit = (s >= makeUpStandard);
                    }

                    score.Detail.SetAttribute("及格標準", passStandard.ToString());
                    score.Detail.SetAttribute("達補考標準", canResit ? "是" : "否");
                    score.Detail.SetAttribute("補考標準", makeUpStandard.ToString());
                    score.Detail.SetAttribute("位數限制", DecimalNumber);
                    score.Detail.SetAttribute("成績身分", calRole);
                }
            }
        }




        /// <summary>
        /// 透過 groupid取得高中補考模組 課程及格與補考標準
        /// </summary>
        /// <param name="GroupIDs"></param>
        /// <returns></returns>
        public Dictionary<string, DataRow> GetStudentMakeupPassScoreByGroupIDs(List<string> GroupIDs)
        {
            //       -- 取得高中補考模組 課程及格與補考標準

            Dictionary<string, DataRow> value = new Dictionary<string, DataRow>();

            if (GroupIDs.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                string strSQL = @"
SELECT ref_makeup_group_id AS group_id
, student.id AS student_id
,course.subject
,course.subj_level
,sc_attend.passing_standard
,sc_attend.makeup_standard 
 FROM 
student 
INNER JOIN 
sc_attend 
ON student.id = sc_attend.ref_student_id 
INNER JOIN course 
ON sc_attend.ref_course_id = course.id 
INNER JOIN $make.up.data 
ON student.id = $make.up.data.ref_student_id :: BIGINT AND course.subject = $make.up.data.subject AND (course.subj_level::TEXT = $make.up.data.level OR (course.subj_level is null AND $make.up.data.level ='') )  
WHERE ref_makeup_group_id in(" + string.Join(",", GroupIDs.ToArray()) + @")
";

                DataTable dt = qh.Select(strSQL);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string key = dr["group_id"] + "_" + dr["student_id"] + "_" + dr["subject"] + "_" + dr["subj_level"];

                        if (!value.ContainsKey(key))
                            value.Add(key, dr);
                    }
                }

            }


            return value;
        }


    }
}

