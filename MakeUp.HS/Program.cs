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

            {
                Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
                ribbon.Add(new RibbonFeature("BE538A8F-71BA-4979-A04A-32A8C239E716", "管理補考梯次"));

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["管理補考梯次"].Enable = UserAcl.Current["BE538A8F-71BA-4979-A04A-32A8C239E716"].Executable;
                               
                //MotherForm.RibbonBarItems["教務作業", "補考作業"]["管理補考梯次"].Image = Properties.Resources.admissions_zoom_64;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["管理補考梯次"].Size = RibbonBarButton.MenuButtonSize.Medium;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["管理補考梯次"].Click += delegate
                {
                    Form.MakeUpBatchManagerForm mubmf = new Form.MakeUpBatchManagerForm();

                    mubmf.ShowDialog();
                };
            }

            {
                Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
                ribbon.Add(new RibbonFeature("AE783777-B1F1-47F7-814B-887FC0C2460D", "管理補考群組"));
                
                MotherForm.RibbonBarItems["教務作業", "補考作業"]["管理補考群組"].Enable = UserAcl.Current["AE783777-B1F1-47F7-814B-887FC0C2460D"].Executable;

                //MotherForm.RibbonBarItems["教務作業", "補考作業"]["管理補考群組"].Image = Properties.Resources.admissions_zoom_64;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["管理補考群組"].Size = RibbonBarButton.MenuButtonSize.Medium;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["管理補考群組"].Click += delegate
                {

                };
            }

            {
                Catalog ribbon = RoleAclSource.Instance["教務作業"]["補考作業"];
                ribbon.Add(new RibbonFeature("5AA949A7-7535-42DD-A81C-D4E4DB2B677C", "產生補考公告"));

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["產生補考公告"].Enable = UserAcl.Current["5AA949A7-7535-42DD-A81C-D4E4DB2B677C"].Executable;


                //MotherForm.RibbonBarItems["教務作業", "補考作業"]["產生補考公告"].Image = Properties.Resources.admissions_zoom_64;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["產生補考公告"].Size = RibbonBarButton.MenuButtonSize.Medium;

                MotherForm.RibbonBarItems["教務作業", "補考作業"]["產生補考公告"].Click += delegate
                {

                };
            }


        }
    }
}
