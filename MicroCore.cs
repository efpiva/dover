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
using SAPbouiCOM;
using SAPbobsCOM;
using System.IO;
using Dover.Framework.Service;
using Castle.Core.Logging;
using Dover.Framework.Model;
using Dover.Framework.Factory;
using Dover.Framework.Log;
using System.Reflection;
using Dover.Framework.Attribute;
using System.Threading;

namespace Dover.Framework
{
    internal class MicroCore
    {
        private DatabaseConfiguration dbConf;
        private AssemblyManager assemblyLoader;
        private MicroCoreEventDispatcher dispatcher;
        private MicroBoot microBoot;
        private I18NService i18nService;
        internal static bool reboot = true; // Used to signal a reboot by AppEvent.
        private int rebootCount = 0;

        public ILogger Logger { get; set; }

        public MicroCore(DatabaseConfiguration dbConf, AssemblyManager assemblyLoader,
            MicroCoreEventDispatcher dispatcher, MicroBoot microBoot, I18NService i18nService)
        {
            this.microBoot = microBoot;
            this.dbConf = dbConf;
            this.assemblyLoader = assemblyLoader;
            this.dispatcher = dispatcher;
            this.i18nService = i18nService;

            i18nService.ConfigureThreadI18n(System.Threading.Thread.CurrentThread);
            microBoot.coreShutdownEvent = new ManualResetEvent(false);
        }

        internal void PrepareFramework()
        {
            try
            {
                while (reboot)
                {
                    microBoot.coreShutdownEvent.Reset();
                    reboot = false;
                    Logger.Debug(Messages.PreparingFramework);
                    dbConf.PrepareDatabase();

                    if (InsideInception())
                        return;

                    string appFolder = CheckAppFolder();
                    Logger.Debug(DebugString.Format(Messages.CreatedAppFolder, appFolder));

                    assemblyLoader.UpdateFrameworkAssemblies(appFolder);
                    assemblyLoader.UpdateAddinsDBAssembly();

                    if (rebootCount == 0)
                        dispatcher.RegisterEvents();

                    microBoot.AppFolder = appFolder;
                    microBoot.StartInception();
                    microBoot.Boot();
                    microBoot.coreShutdownEvent.WaitOne();
                    rebootCount++;
                }
                reboot = true; // in case we need to start it again (i.e. unit test).
                ContainerManager.Container.Dispose();
                SAPServiceFactory.LogOff();
            }
            catch (Exception e)
            {
                Logger.Fatal(String.Format(Messages.GeneralError, e.Message), e);
                Environment.Exit(10);
            }
        }

        private void CopyResource(string destination, string resource)
        {
            using (var doverConfig = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                if (!File.Exists(destination) && doverConfig != null)
                {
                    using(var destinationStream = new FileStream(destination, FileMode.CreateNew))
                    {
                        byte[] buffer = new byte[1000];
                        int buffLen;
                        while ((buffLen = doverConfig.Read(buffer, 0, 1000)) > 0)
                        {
                            destinationStream.Write(buffer, 0, buffLen);
                        }

                    }
                }
            }
        }

        private string CheckAppFolder()
        {
            string appFolder = assemblyLoader.GetDoverDirectory();

            string frameworkFolder = Path.Combine(appFolder, "Framework");
            CreateIfNotExists(frameworkFolder);
            string cacheFolder = Path.Combine(appFolder, "..", "Cache");
            CreateIfNotExists(cacheFolder);

            return frameworkFolder;
        }

        private void CreateIfNotExists(string appFolder)
        {
            if (System.IO.Directory.Exists(appFolder) == false)
            {
                System.IO.Directory.CreateDirectory(appFolder);
            }
        }

        private bool InsideInception()
        {
            return AppDomain.CurrentDomain.FriendlyName == "Dover.Inception"
                || AppDomain.CurrentDomain.FriendlyName == "Dover.AddIn";
        }
    }
}
