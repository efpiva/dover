using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Attribute;
using AddOne.Framework.Model.SAP;
using AddOne.Framework.Monad;
using SAPbouiCOM;
using Castle.Core.Logging;
using AddOne.Framework.Factory;
using System.Reflection;
using System.IO;

namespace AddOne.Framework.DAO
{
    internal class BusinessOneUIDAOImpl : BusinessOneUIDAO
    {
        private Application application;

        public ILogger Logger { get; set; }

        public BusinessOneUIDAOImpl(Application application)
        {
            this.application = application;
        }

        internal override void ProcessMenuAttribute(List<MenuAttribute> menus)
        {
            UIApplication appCommand;
            List<ApplicationMenusActionMenu> actionMenus = new List<ApplicationMenusActionMenu>();

            menus.Sort();


            foreach (var menu in menus) 
            {

                if (application.Menus.Exists(menu.UniqueID)
                    && RemoveIfNotEqual(menu))
                    continue;

                if (!string.IsNullOrEmpty(menu.ValidateMethod) && menu.OriginalType != null
                    && NotAuthorized(menu))
                    continue;

                var actionMenu = new ApplicationMenusActionMenu();
                if (!string.IsNullOrEmpty(menu.i18n))
                    menu.String = GetLocalizedString(menu);

                Logger.Info(String.Format(Messages.MenuProcess, menu.String, menu.UniqueID));

                actionMenu.Checked = menu.Return(x => x.Checked, "0");
                actionMenu.Enabled = menu.Return(x => x.Enabled, "1");
                actionMenu.FatherUID = menu.FatherUID;
                if (menu.Image != null)
                    actionMenu.Image = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, menu.Image);
                actionMenu.String = menu.String;
                actionMenu.Type = ((int)menu.Type).ToString();
                actionMenu.UniqueID = menu.UniqueID;
                actionMenus.Add(actionMenu);
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
            Logger.Debug(String.Format(Messages.MenuStart, xml));
            try
            {
                application.LoadBatchActions(ref xml);
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.MenuError, e.Message), e);
                throw e;
            }
            string result = application.GetLastBatchResults();

            Logger.Debug(Messages.MenuEnd);
        }

        private bool RemoveIfNotEqual(MenuAttribute menu)
        {
            var sapMenu = application.Menus.Item(menu.UniqueID);
            bool same = sapMenu.Checked == menu.Return(x => x.Checked, "0").Equals("1")
                && sapMenu.Enabled == menu.Return(x => x.Checked, "0").Equals("0")
                && sapMenu.Image == menu.Image
                && sapMenu.String == menu.String
                && sapMenu.Type == menu.Type
                && sapMenu.UID == menu.UniqueID;

            if (!same)
                application.Menus.RemoveEx(menu.UniqueID);
            return same;
        }

        private string GetLocalizedString(MenuAttribute menu)
        {
            var index = menu.Return(x => x.i18n, String.Empty).LastIndexOf(".");
            if (index < 0)
            {
                Logger.Error(String.Format(Messages.MenuI18NFormatError, menu.i18n));
                return menu.Return(x => x.i18n, String.Empty);
            }
            string typeName = menu.i18n.Substring(0, index);
            index++;
            string propertyName = menu.i18n.Substring(index);
            var assembly = Assembly.GetEntryAssembly();

            // TODO: cache e resourceCulture.
            var resource = new System.Resources.ResourceManager(typeName, assembly);

            if (resource != null)
            {
                Logger.Debug(String.Format(Messages.GetLocalizedStringFoundResource, menu.i18n));
            } else
            {
                Logger.Debug(String.Format(Messages.GetLocalizedStringNotFoundResource, menu.i18n));
            }

            return resource.GetString(propertyName);
        }

        private bool NotAuthorized(MenuAttribute menu)
        {
            var obj = ContainerManager.Container.Resolve(menu.OriginalType);
            var ret = menu.With(x => x.OriginalType)
                .With(x => x.GetMethod(
                    menu.Return(y => y.ValidateMethod, string.Empty)))
                .With(x => x.Invoke(obj, null));

            if (!(ret is bool))
            {
                Logger.Error(String.Format(Messages.AuthorizationMessage, menu.OriginalType, menu.ValidateMethod));
                return false;
            }

            return !(bool)ret;
        }
    }
}
