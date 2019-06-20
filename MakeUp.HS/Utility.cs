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

                    if (decimal.TryParse(score.Detail.GetAttribute("原始成績"), out s))
                    {
                        if (resitLimit.ContainsKey(score.GradeYear)) makeUpStandard = resitLimit[score.GradeYear];
                        canResit = (s >= makeUpStandard);
                    }

                    if (applyLimit.ContainsKey(score.GradeYear)) passStandard = applyLimit[score.GradeYear];

                    score.Detail.SetAttribute("及格標準", passStandard.ToString());
                    score.Detail.SetAttribute("達補考標準", canResit ? "是" : "否");
                    score.Detail.SetAttribute("補考標準", makeUpStandard.ToString());
                    score.Detail.SetAttribute("位數限制", DecimalNumber);
                    score.Detail.SetAttribute("成績身分", calRole);
                }
            }
        }



    }
}

