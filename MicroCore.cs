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

namespace Dover.Framework
{
    class MicroCore
    {
        private SAPbobsCOM.Company company;
        private DatabaseConfiguration dbConf;
        private AssemblyManager assemblyLoader;
        private MicroCoreEventDispatcher dispatcher;
        private MicroBoot microBoot;
        private I18NService i18nService;

        public ILogger Logger { get; set; }

        public MicroCore(DatabaseConfiguration dbConf, SAPbobsCOM.Company company, AssemblyManager assemblyLoader,
            MicroCoreEventDispatcher dispatcher, MicroBoot microBoot, I18NService i18nService)
        {
            this.microBoot = microBoot;
            this.company = company;
            this.dbConf = dbConf;
            this.assemblyLoader = assemblyLoader;
            this.dispatcher = dispatcher;
            this.i18nService = i18nService;

            i18nService.ConfigureThreadI18n(System.Threading.Thread.CurrentThread);
        }

        internal void PrepareFramework()
        {
            try
            {
                Logger.Debug(Messages.PreparingFramework);
                dbConf.PrepareDatabase();

                if (InsideInception())
                    return;

                string appFolder = CheckAppFolder();
                Logger.Debug(DebugString.Format(Messages.CreatedAppFolder, appFolder));

                assemblyLoader.UpdateAssemblies(AssemblySource.Core, appFolder);
                assemblyLoader.UpdateAssemblies(AssemblySource.AddIn, appFolder);
                CopyInstallResources(appFolder, Environment.CurrentDirectory);

                dispatcher.RegisterEvents();

                microBoot.AppFolder = appFolder;
                microBoot.StartInception();
                microBoot.Boot();
                System.Windows.Forms.Application.Run();
            }
            catch (Exception e)
            {
                Logger.Fatal(String.Format(Messages.GeneralError, e.Message), e);
                Environment.Exit(10);
            }
        }

        private void CopyInstallResources(string appFolder, string sourceFolder)
        {
            string source, destination;
            source = Path.Combine(sourceFolder, "DoverInception.config");
            destination = Path.Combine(appFolder, "Dover.config");
            if (!File.Exists(destination) && File.Exists(source))
            {
                File.Copy(source, destination);
            }

            source = Path.Combine(sourceFolder, "DoverAddin.config");
            destination = Path.Combine(appFolder, "DoverAddin.config");
            if (!File.Exists(destination) && File.Exists(source))
            {
                File.Copy(source, destination);
            }

        }

        private string CheckAppFolder()
        {
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Dover";
            CreateIfNotExists(appFolder);
            appFolder = Path.Combine(appFolder, company.Server + "-" + company.CompanyDB);
            CreateIfNotExists(appFolder);
            return appFolder;
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
