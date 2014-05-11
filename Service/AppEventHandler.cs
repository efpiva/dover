using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;

namespace AddOne.Framework.Service
{
    internal class AppEventHandler
    {
        public ILogger Logger { get; set; }
        private AddinLoader addinLoader;
        private LicenseManager licenseManager;

        public AppEventHandler(AddinLoader addinLoader, LicenseManager licenseManager)
        {
            this.addinLoader = addinLoader;
            this.licenseManager = licenseManager;
        }

        internal void sapApp_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    System.Windows.Forms.Application.Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    try
                    {
                        Reboot();
                        // TODO: config thread i18n and menu i18n.
                        // this.LoadMenu();
                    }
                    catch (Exception er)
                    {
                        Logger.Error(Messages.EventLanguageChangedError, er);
                    }
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    try
                    {
                        Reboot();
                    }
                    catch (Exception er)
                    {
                        Logger.Error(Messages.EventCompanyChangedError, er);
                    }
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    System.Windows.Forms.Application.Exit();
                    break;
            }
        }

        private void Reboot()
        {
            try
            {
                Logger.Info(String.Format(Messages.Restarting, this.GetType().Assembly.GetName().Version));
                addinLoader.ShutdownAddins();
                var addins = licenseManager.ListAddins();
                addinLoader.LoadAddins(addins);
            }
            catch (Exception e)
            {
                Logger.Fatal(Messages.ErrorReboot, e);
                Environment.Exit(10);
            }
        }

    }
}
