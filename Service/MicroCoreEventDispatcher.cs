using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using AddOne.Framework.Attribute;
using AddOne.Framework.Factory;
using SAPbouiCOM.Framework;
using Castle.Core.Logging;
using System.ServiceModel;
using AddOne.Framework.IPC;

namespace AddOne.Framework.Service
{
    /// <summary>
    /// User by MicroCore. Just AppEvent (reboot / reload addins).
    /// </summary>
    public class MicroCoreEventDispatcher
    {
        public ILogger Logger { get; set; }
        private SAPbouiCOM.Application sapApp;
        private InceptionServer inceptionServer;
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

        internal void RegisterInception(AppDomain inception)
        {
            appEventHandler.Inception = inception;
        }
    }
}
