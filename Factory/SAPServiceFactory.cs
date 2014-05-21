using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using System.Windows.Forms;
using System.Reflection;
using SAPbobsCOM;
using Castle.Core.Logging;
using AddOne.Framework.Monad;

namespace AddOne.Framework.Factory
{
    public class SAPServiceFactory
    {
        private static SAPbouiCOM.Application application;
        private static SAPbobsCOM.Company company;
        private static SAPbouiCOM.Framework.Application frameworkApplication;
        private static object threadLock = new System.Object();
        private static bool b1Connected = false;
        private static bool connString = true;
        internal static string connStringValue = string.Empty;

        public static ILogger Logger { get; set; }

        private static void B1Connect(String version)
        {
            company = (SAPbobsCOM.Company)AppDomain.CurrentDomain.GetData("SAPCompany");

            frameworkApplication = FrameworkApplicationFactory();
            application = SAPbouiCOM.Framework.Application.SBO_Application;

            // inception!
            if (application != null && company != null)
            {
                return;
            }

            try
            {
                company = (SAPbobsCOM.Company)application.Company.GetDICompany();

                b1Connected = company.Connected;
                // Logger depende do b1Connected.
                Logger.Info(String.Format("Iniciado o AddOne ({0}) com sucesso [UI-API / DI-API]", version));
            }
            catch (Exception er)
            {
                Logger.Fatal(String.Format("Erro de conexão do AddOn: {0}", er.Message), er);
            }
        }

        private static string GetConnectionString()
        {
            string ret;
            if (Environment.GetCommandLineArgs().Length > 1 && connString
                && AppDomain.CurrentDomain.FriendlyName != "AddOne.AddIn"
                && AppDomain.CurrentDomain.FriendlyName != "AddOne.Inception")
            {
                ret = Environment.GetCommandLineArgs()[1];
                connString = false;
                connStringValue = ret;
            }
            else
            {
                ret = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
                if (connStringValue == string.Empty)
                {
                    connStringValue = ret; // debug only.
                }
            }

            return ret;
        }


        public static SAPbouiCOM.Application ApplicationFactory()
        {
            lock (threadLock)
            {
                if (!b1Connected)
                {
                    B1Connect(GetVersion());
                }
                    
                return application;
            }
        }

        public static SAPbobsCOM.Company CompanyFactory()
        {
            lock (threadLock)
            {
                if (!b1Connected)
                {
                    B1Connect(GetVersion());
                }
                return company;
            }
        }

        public static SAPbouiCOM.Framework.Application FrameworkApplicationFactory()
        {
            lock (threadLock)
            {
                if (frameworkApplication == null)
                    frameworkApplication = new SAPbouiCOM.Framework.Application(GetConnectionString());
                return frameworkApplication;
            }
        }

        private static string GetVersion()
        {
            Version version = typeof(SAPServiceFactory).Assembly.GetName().Version;
            return version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString()
                + "." + version.Revision;
        }


        public static void PrepareForInception(AppDomain inception)
        {
            lock (threadLock)
            {
                string pipeName = "addOne" + connStringValue;
                AppDomain.CurrentDomain.SetData("AddOnePIPE", pipeName);

                if (application == null && company == null)
                    B1Connect(GetVersion());
                inception.SetData("SAPCompany", company);
                inception.SetData("AddOnePIPE", pipeName);
            }
        }
    }
}
