using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using Dover.Framework.Attribute;
using Dover.Framework.Factory;
using SAPbouiCOM.Framework;
using Castle.Core.Logging;
using System.ServiceModel;

namespace Dover.Framework.Service
{
    /// <summary>
    /// User by MicroCore. Just AppEvent (reboot / reload addins).
    /// </summary>
    public class MicroCoreEventDispatcher
    {
        public ILogger Logger { get; set; }
        private SAPbouiCOM.Application sapApp;
        private AppEventHandler appEventHandler;

        public MicroCoreEventDispatcher(SAPbouiCOM.Application sapApp, AppEventHandler appEventHandler)
        {
            this.sapApp = sapApp;
            this.appEventHandler = appEventHandler;
        }

        internal void RegisterEvents()
        {
            sapApp.AppEvent += new _IApplicationEvents_AppEventEventHandler(appEventHandler.sapApp_AppEvent);
        }
    }
}
