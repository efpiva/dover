using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using AddOne.Framework.Attribute;
using AddOne.Framework.Factory;
using SAPbouiCOM.Framework;
using Castle.Core.Logging;

namespace AddOne.Framework.Service
{
    internal class EventDispatcher
    {
        public ILogger Logger { get; set; }
        private SAPbouiCOM.Application sapApp;
        private MenuEventHandler menuHandler;
        private AppEventHandler appHandler;

        public EventDispatcher(SAPbouiCOM.Application sapApp, MenuEventHandler menuHandler, AppEventHandler appHandler)
        {
            this.sapApp = sapApp;
            this.menuHandler = menuHandler;
            this.appHandler = appHandler;
        }

        internal void RegisterEvents()
        {
            sapApp.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(menuHandler.sapApp_MenuEvent);
            sapApp.AppEvent += new _IApplicationEvents_AppEventEventHandler(appHandler.sapApp_AppEvent);
        }

    }
}
