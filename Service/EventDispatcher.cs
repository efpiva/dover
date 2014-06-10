using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using AddOne.Framework.Attribute;
using AddOne.Framework.Factory;
using Castle.Core.Logging;
using AddOne.Framework.Form;

namespace AddOne.Framework.Service
{
    /// <summary>
    /// User just by Inception / addins. No App event.
    /// </summary>
    public class EventDispatcher
    {
        public ILogger Logger { get; set; }
        private Application sapApp;
        private MenuEventHandler menuHandler;

        public EventDispatcher(Application sapApp, MenuEventHandler menuHandler)
        {
            this.sapApp = sapApp;
            this.menuHandler = menuHandler;
        }

        internal void RegisterEvents()
        {
            sapApp.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(menuHandler.sapApp_MenuEvent);
        }

    }
}
