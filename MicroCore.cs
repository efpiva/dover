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
    public class MicroCore
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

        public void PrepareFramework()
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
