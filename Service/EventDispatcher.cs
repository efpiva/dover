using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using Dover.Framework.Attribute;
using Dover.Framework.Factory;
using Castle.Core.Logging;
using Dover.Framework.Form;

namespace Dover.Framework.Service
{
    /// <summary>
    /// User just by Inception / addins. No App event.
    /// </summary>
    public class EventDispatcher
    {
        public ILogger Logger { get; set; }
        private SAPbouiCOM.Application sapApp;
        private MenuEventHandler menuHandler;

        public EventDispatcher(SAPbouiCOM.Application sapApp, MenuEventHandler menuHandler)
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
