using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Castle.Core.Logging;
using Dover.Framework.Service;

namespace Dover.Framework
{
    internal class Boot
    {
        public ILogger Logger { get; set; }

        private LicenseManager licenseManager;
        private AddinManager addinLoader;
        private EventDispatcher dispatcher;
        private FormEventHandler formEventHandler;

        public Boot(LicenseManager licenseValidation, AddinManager addinLoader, EventDispatcher dispatcher,
            FormEventHandler formEventHandler, I18NService i18nService)
        {
            this.licenseManager = licenseValidation;
            this.addinLoader = addinLoader;
            this.dispatcher = dispatcher;
            this.formEventHandler = formEventHandler;

            i18nService.ConfigureThreadI18n(System.Threading.Thread.CurrentThread);
        }

        internal void StartUp()
        {
            string moduleName = this.GetType().Assembly.GetName().Name;
            try
            {
                Logger.Info(String.Format(Messages.Starting, moduleName, this.GetType().Assembly.GetName().Version));
                var addins = licenseManager.ListAddins();
                addinLoader.LoadAddins(addins);
                dispatcher.RegisterEvents();
                StartFrameworkUI(); // load admin forms.
                System.Windows.Forms.Application.Run();
            }
            catch (Exception e)
            {
                Logger.Fatal(string.Format(Messages.ErrorStartup, moduleName), e);
                Environment.Exit(10);
            }
        }

        private void StartFrameworkUI()
        {
            addinLoader.StartMenu(Assembly.GetExecutingAssembly());
            formEventHandler.RegisterForms();
        }

        internal void StartThis()
        {
            string thisAsmName = (string)AppDomain.CurrentDomain.GetData("assemblyName");
            try
            {
                Assembly thisAsm = AppDomain.CurrentDomain.Load(thisAsmName);
                Logger.Info(String.Format(Messages.Starting, thisAsmName, thisAsm.GetName().Version));
                addinLoader.StartThis();
                dispatcher.RegisterEvents();
                formEventHandler.RegisterForms();
            }
            catch (Exception e)
            {
                Logger.Fatal(string.Format(Messages.ErrorStartup, thisAsmName), e);
                Environment.Exit(10);
            }
        }
    }
}
