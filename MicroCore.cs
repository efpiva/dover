using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using SAPbobsCOM;
using System.IO;
using AddOne.Framework.Service;
using Castle.Core.Logging;
using AddOne.Framework.Model.Assembly;

namespace AddOne.Framework
{
    internal class MicroCore
    {
        private SAPbobsCOM.Company company;
        private DatabaseConfiguration dbConf;
        private AssemblyLoader assemblyLoader;

        public ILogger Logger { get; set; }

        public MicroCore(DatabaseConfiguration dbConf, SAPbobsCOM.Company company, AssemblyLoader assemblyLoader)
        {
            this.company = company;
            this.dbConf = dbConf;
            this.assemblyLoader = assemblyLoader;
        }

        internal string PrepareFramework()
        {
            try
            {
                Logger.Debug(Messages.PreparingFramework);
                dbConf.PrepareDatabase();

                if (InsideInception())
                    return null;

                string appFolder = CheckAppFolder();
                Logger.Debug(String.Format(Messages.CreatedAppFolder, appFolder));

                assemblyLoader.UpdateAssemblies(AssemblySource.Core, appFolder);
                assemblyLoader.UpdateAssemblies(AssemblySource.AddIn, appFolder);

                var appLog = Path.Combine(appFolder, "log4net.config");
                var defaultLog = Path.Combine(Environment.CurrentDirectory, "log4netAddin.config");
                if (!File.Exists(appLog) && File.Exists(defaultLog))
                {
                    File.Copy(defaultLog, appLog);
                }

                return appFolder;
            }
            catch (Exception e)
            {
                Logger.Fatal(String.Format(Messages.GeneralError, e.Message), e);
                Environment.Exit(10);
                return null;
            }
        }

        private string CheckAppFolder()
        {
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AddOne";
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
            return AppDomain.CurrentDomain.FriendlyName == "AddOne.Inception"
                || AppDomain.CurrentDomain.FriendlyName == "AddOne.AddIn";
        }
    }
}
