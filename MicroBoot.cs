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
using Dover.Framework.Factory;
using System.Threading;
using Dover.Framework.Remoting;
using Dover.Framework.Service;
using Dover.Framework.Interface;

namespace Dover.Framework
{
    public class MicroBoot
    {
        internal string AppFolder;
        private I18NService I18NService;

        internal AppDomain Inception { get; set; }
        internal IAddinManager InceptionAddinManager { get; set; }

        public MicroBoot(I18NService I18NService)
        {
            this.I18NService = I18NService;
        }

        internal void StartInception()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "Dover.Inception";
            setup.ApplicationBase = AppFolder;
            Inception = AppDomain.CreateDomain("Dover.Inception", null, setup);
            Environment.CurrentDirectory = AppFolder;
        }

        internal void Boot()
        {
            var thread = new Thread(new ThreadStart(this.PrivateBoot));
            thread.SetApartmentState(ApartmentState.STA);
            I18NService.ConfigureThreadI18n(thread);
            thread.Start();
        }

        private void PrivateBoot()
        {
            try
            {
                IApplication app = (IApplication)Inception.CreateInstanceAndUnwrap("Framework", "Dover.Framework.Application");
                SAPServiceFactory.PrepareForInception(Inception); // need to be after Application creation because of assembly resolving from embedded resources.
                Inception.SetData("assemblyName", "Framework"); // Used to get current AssemblyName for logging and reflection            
                InceptionAddinManager = app.Resolve<IAddinManager>();
                Sponsor<IApplication> appSponsor = new Sponsor<IApplication>(app);
                Sponsor<IAddinManager> addInManagerSponsor = new Sponsor<IAddinManager>(InceptionAddinManager);
                app.RunInception();
                AppDomain.Unload(Inception); // release AppDomain on shutdown.
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("{0}\n{1}", e.Message, e.StackTrace));
                throw e;
            }
        }
    }
}
