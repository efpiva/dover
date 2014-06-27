using System;
using System.Collections.Generic;
using System.IO;
using AddOne.Framework.Attribute;
using AddOne.Framework.Factory;
using AddOne.Framework.Model.SAP;
using AddOne.Framework.Monad;
using AddOne.Framework.Service;
using Castle.Core.Logging;
using SAPbouiCOM;
using System.Xml.Linq;
using AddOne.Framework.Log;

namespace AddOne.Framework.DAO
{
    public class BusinessOneUIDAOImpl : BusinessOneUIDAO
    {
        private Application application;
        private I18NService i18NService;

        public ILogger Logger { get; set; }

        public BusinessOneUIDAOImpl(Application application, I18NService addIni18n)
        {
            this.application = application;
            this.i18NService = addIni18n;
        }

        public override void ProcessMenuAttribute(List<MenuAttribute> menus)
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
                    menu.String = i18NService.GetLocalizedString(menu.i18n);

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
            Logger.Debug(DebugString.Format(Messages.MenuStart, xml));
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
            string sapMenuFileName = string.Empty;
            var sapMenu = application.Menus.Item(menu.UniqueID);
            if (sapMenu.Image != null)
                sapMenuFileName = Path.GetFileName(sapMenu.Image);

            bool same = sapMenu.Checked == menu.Return(x => x.Checked, "0").Equals("1")
                && sapMenu.Enabled == menu.Return(x => x.Enabled, "0").Equals("1")
                && sapMenuFileName == menu.Return(x => x.Image, string.Empty)
                && sapMenu.String == menu.String
                && sapMenu.Type == menu.Type
                && sapMenu.UID == menu.UniqueID;

            if (!same)
                application.Menus.RemoveEx(menu.UniqueID);
            return same;
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

        internal override void LoadBatchAction(string xml)
        {
            application.LoadBatchActions(xml);
            ParseBatchResult(application.GetLastBatchResults());
        }
        internal override IForm LoadFormBatchAction(string xml)
        {
            int formCount = application.Forms.Count;
            application.LoadBatchActions(xml);
            // TODO parse return and show error.
            if (application.Forms.Count == (formCount+1)) // a form was created from XMl
                return application.Forms.Item(formCount);
            return null;
        }

        internal override IForm GetFormByUID(string formUID)
        {
            return application.Forms.Item(formUID);
        }

        private void ParseBatchResult(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            var errors = doc.Element("result").Element("errors").Elements("error");
            foreach (var error in errors)
            {
                string code = error.Attribute("code").With(x => x.Value);
                string msg = error.Attribute("descr").With(x => x.Value);
                if (code == null && msg != null)
                {
                    Logger.Error(string.Format(Messages.UIAPIError, msg));
                }
                else if (code != null && msg != null)
                {
                    Logger.Error(string.Format(Messages.UIAPICodeAndError, code, msg));
                }
            }   
        }
    }
}
