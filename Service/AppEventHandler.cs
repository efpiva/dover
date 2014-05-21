using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using System.ServiceModel;
using AddOne.Framework.IPC;
using AddOne.Framework.Monad;

namespace AddOne.Framework.Service
{
    public class AppEventHandler
    {
        private MicroBoot microBoot;
        private InceptionServer _inceptionServer;
        private InceptionServer inceptionServer
        {
            get
            {
                if (_inceptionServer == null)
                {
                    _inceptionServer = CreateConnection();
                }
                return _inceptionServer;
            }
        }

        internal AppDomain Inception { get; set; }
        public ILogger Logger { get; set; }

        public AppEventHandler(MicroBoot microBoot)
        {
            this.microBoot = microBoot;
        }

        internal void sapApp_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            try
            {
                switch (EventType)
                {
                    case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                        Logger.Info(Messages.Shutdown);
                        inceptionServer.Do(x => x.ShutdownAddins());
                        AppDomain.Unload(Inception);
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
                        Logger.Info(Messages.Shutdown);
                        inceptionServer.Do(x => x.ShutdownAddins());
                        AppDomain.Unload(Inception);
                        System.Windows.Forms.Application.Exit();
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                Environment.Exit(20);
            }
        }

        private void Reboot()
        {
            try
            {
                Logger.Info(String.Format(Messages.Restarting, this.GetType().Assembly.GetName().Version));
                Logger.Info(Messages.Shutdown);
                inceptionServer.Do(x => x.ShutdownAddins());
                AppDomain.Unload(Inception);
                microBoot.StartInception();
                Inception = microBoot.Inception;
                microBoot.Boot();
            }
            catch (Exception e)
            {
                Logger.Fatal(Messages.ErrorReboot, e);
                Environment.Exit(10);
            }
        }

        private InceptionServer CreateConnection()
        {
            string pipeName = (string)AppDomain.CurrentDomain.GetData("AddOnePIPE");
            ChannelFactory<InceptionServer> pipeFactory =
        new ChannelFactory<InceptionServer>(
          new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),
          new EndpointAddress(
            "net.pipe://localhost/" + pipeName + "/InceptionServer"));
            return pipeFactory.CreateChannel();
        }

    }
}
