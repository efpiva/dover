using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Attribute;
using AddOne.Framework.Model.SAP;
using AddOne.Framework.Monad;
using SAPbouiCOM;

namespace AddOne.Framework.DAO
{
    class BusinessOneUIDAOImpl : BusinessOneUIDAO
    {
        private Application application;

        public BusinessOneUIDAOImpl(Application application)
        {
            this.application = application;
        }

        public void ProcessMenuAttribute(List<MenuAttribute> menus)
        {
            UIApplication appCommand;
            List<ApplicationMenusActionMenu> actionMenus = new List<ApplicationMenusActionMenu>();

            foreach (var menu in menus) 
            {
                var actionMenu = new ApplicationMenusActionMenu();
                actionMenu.Checked = menu.Checked;
                actionMenu.Enabled = menu.Enabled;
                actionMenu.FatherUID = menu.FatherUID;
                actionMenu.Image = menu.Image;
                actionMenu.String = menu.String;
                actionMenu.Type = menu.Type;
                actionMenu.UniqueID = menu.UniqueID;
            }
            appCommand = new UIApplication() {
                Menus = new ApplicationMenus[] {
                    new ApplicationMenus() {
                        action = new ApplicationMenusAction[] {
                            new ApplicationMenusAction() {
                                type = "add",
                                Menu = actionMenus.ToArray()
                    }}}}};

            string xml = appCommand.Serialize();
            application.LoadBatchActions(ref xml);
        }
    }
}
