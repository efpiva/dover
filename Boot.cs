/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
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
        private AddinLoader addinLoader;
        private AddinManager addinManager;
        private EventDispatcher dispatcher;
        private FormEventHandler formEventHandler;

        public Boot(LicenseManager licenseValidation, AddinManager addinManager, AddinLoader addinLoader,
            EventDispatcher dispatcher, FormEventHandler formEventHandler, I18NService i18nService)
        {
            this.licenseManager = licenseValidation;
            this.addinManager = addinManager;
            this.dispatcher = dispatcher;
            this.formEventHandler = formEventHandler;
            this.addinLoader = addinLoader;

            i18nService.ConfigureThreadI18n(System.Threading.Thread.CurrentThread);
        }

        internal void StartUp()
        {
            string moduleName = this.GetType().Assembly.GetName().Name;
            try
            {
                Logger.Info(String.Format(Messages.Starting, moduleName, this.GetType().Assembly.GetName().Version));
                var addins = licenseManager.ListAddins();
                dispatcher.RegisterEvents();
                StartFrameworkUI(); // load admin forms.
                addinManager.LoadAddins(addins);
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

        internal bool StartThis()
        {
            string thisAsmName = (string)AppDomain.CurrentDomain.GetData("assemblyName");
            try
            {
                Assembly thisAsm = AppDomain.CurrentDomain.Load(thisAsmName);
                Logger.Info(String.Format(Messages.Starting, thisAsmName, thisAsm.GetName().Version));

                addinLoader.StartThis();
                dispatcher.RegisterEvents();
                formEventHandler.RegisterForms();
                return true;
            }
            catch (Exception e)
            {
                Logger.Fatal(string.Format(Messages.ErrorStartup, thisAsmName), e);
                return false;
            }
        }
    }
}
