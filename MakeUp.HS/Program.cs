using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FISCA.Presentation;
using K12.Presentation;
using FISCA.Permission;
using FISCA;

namespace MakeUp.HS
{    
    public class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {
            FISCA.UDT.AccessHelper accessHelper = new FISCA.UDT.AccessHelper();

            // 先將UDT 選起來，如果是第一次開啟沒有話就會新增
            accessHelper.Select<UDT_MakeUpBatch>();
            accessHelper.Select<UDT_MakeUpGroup>();
            accessHelper.Select<UDT_MakeUpData>();
            accessHelper.Select<UDT_ReportTemplate>();


            MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"].Size = RibbonBarButton.MenuButtonSize.Large;

            MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"].Image = Properties.Resources.calc_64;

            {
                Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
                ribbon.Add(new RibbonFeature("BE538A8F-71BA-4979-A04A-32A8C239E716", "管理補考梯次"));

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["管理補考梯次"].Enable = UserAcl.Current["BE538A8F-71BA-4979-A04A-32A8C239E716"].Executable;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["管理補考梯次"].Click += delegate
                {
                    Form.MakeUpBatchManagerForm mubmf = new Form.MakeUpBatchManagerForm();

                    mubmf.ShowDialog();
                };
            }

            {
                Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
                ribbon.Add(new RibbonFeature("AE783777-B1F1-47F7-814B-887FC0C2460D", "管理補考群組"));

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["管理補考群組"].Enable = UserAcl.Current["AE783777-B1F1-47F7-814B-887FC0C2460D"].Executable;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["管理補考群組"].Click += delegate
                {
                    Form.MakeUpGroupManagerForm mugmf = new Form.MakeUpGroupManagerForm("管理補考群組");

                    mugmf.ShowDialog();
                };
            }

            {
                Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
                ribbon.Add(new RibbonFeature("5AA949A7-7535-42DD-A81C-D4E4DB2B677C", "產生補考公告"));

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["產生補考公告"].Enable = UserAcl.Current["5AA949A7-7535-42DD-A81C-D4E4DB2B677C"].Executable;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["產生補考公告"].Click += delegate
                {
                    Form.ExportMakeUpReportForm emurf = new Form.ExportMakeUpReportForm();

                    emurf.ShowDialog();
                };
            }

            {
                Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
                ribbon.Add(new RibbonFeature("6AED85C7-F6CF-49A5-8AAC-C97CF7127AEB", "補考成績輸入狀況"));

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["補考成績輸入狀況"].Enable = UserAcl.Current["6AED85C7-F6CF-49A5-8AAC-C97CF7127AEB"].Executable;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["補考成績輸入狀況"].Click += delegate
                {
                    Form.MakeUpScoreStatusForm myssf = new Form.MakeUpScoreStatusForm();

                    myssf.ShowDialog();
                };
            }



            // 2019/06/03 穎驊註解， 後來有了 補考成績輸入狀況 檢查， 就把 管理成績輸入 介面一併 併過了去了， 這個專門輸入成績的流程 先隱藏
            //{
            //    Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
            //    ribbon.Add(new RibbonFeature("47A870E8-0C03-4DE0-A85E-C2B4551351C8", "管理補考成績"));

            //    MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["管理補考成績"].Enable = UserAcl.Current["47A870E8-0C03-4DE0-A85E-C2B4551351C8"].Executable;

            //    MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["管理補考成績"].Click += delegate
            //    {
            //        Form.MakeUpGroupManagerForm mugmf = new Form.MakeUpGroupManagerForm("管理補考成績");

            //        mugmf.ShowDialog();
            //    };
            //}


            {
                Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
                ribbon.Add(new RibbonFeature("E3D987DC-E75C-4472-BAB8-C58EEAA844F9", "產生學期科目成績匯入檔"));

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["產生學期科目成績匯入檔"].Enable = UserAcl.Current["E3D987DC-E75C-4472-BAB8-C58EEAA844F9"].Executable;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["補考作業"]["產生學期科目成績匯入檔"].Click += delegate
                {
                    Form.ExportMakeUpScoreForm emusf = new Form.ExportMakeUpScoreForm();

                    emusf.ShowDialog();
                };
            }


        }
    }
}
