using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SAPbobsCOM;
using Dover.Framework;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dover.Framework.DAO;

namespace FrameworkTest
{
    static class DoverSetup
    {
        private static BootDoverHelper bootDoverHelper = null;
        private static bool cleaned = false;

        internal static Application CleanDover(bool dropTable)
        {
            Application doverApp = new Application();
            var company = doverApp.Resolve<SAPbobsCOM.Company>();
            var app = doverApp.Resolve<SAPbouiCOM.Application>();
            Recordset rs = (dropTable ? null : (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset));
            UserTablesMD ut = (!dropTable ? null : (UserTablesMD)company.GetBusinessObject(BoObjectTypes.oUserTables));

            try
            {
                if (cleaned)
                {
                    // For some reason VS holds reference for unloaded appdomains, for debug reason. Clean on the first run.
                    string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dover");
                    if (Directory.Exists(appFolder))
                    {
                        cleanAllFiles(appFolder);
                    }
                    cleaned = true;
                }

                if (!dropTable)
                {
                    rs.DoQuery("DELETE FROM \"@DOVER_MODULES\"");
                    rs.DoQuery("DELETE FROM \"@DOVER_MODULES_BIN\"");
                    rs.DoQuery("DELETE FROM \"@DOVER_MODULES_DEP\"");
                    rs.DoQuery("DELETE FROM \"@DOVER_MODULES_USER\"");
                    rs.DoQuery("DELETE FROM \"@DOVER_LOGS\"");
                    rs.DoQuery("DELETE FROM \"@DOVER_LICENSE\"");
                    rs.DoQuery("DELETE FROM \"@DOVER_LICENSE_BIN\"");
                }
                else
                {
                    removeTable(ut, "DOVER_MODULES", app, company);
                    removeTable(ut, "DOVER_MODULES_BIN", app, company);
                    removeTable(ut, "DOVER_MODULES_DEP", app, company);
                    removeTable(ut, "DOVER_MODULES_USER", app, company);
                    removeTable(ut, "DOVER_LOGS", app, company);
                    removeTable(ut, "DOVER_LICENSE", app, company);
                    removeTable(ut, "DOVER_LICENSE_BIN", app, company);
                }
                
            }
            finally
            {
                if (rs != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                if (ut != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ut);
            }
            return doverApp;
        }

        private static void cleanAllFiles(string appFolder)
        {
            // For some reason, some reference is hold for the Framework directory.
            // All files are released and no AppDomain is loaded, so we delete only the
            // files so we can move on.
            var files = Directory.GetFiles(appFolder, "*", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                File.Delete(file);
            }
        }

        internal static void removeUDO(UserObjectsMD uo, string name,
                            SAPbouiCOM.Application app, SAPbobsCOM.Company company)
        {
            int ret;
            string errMsg;
            if (uo.GetByKey(name))
            {
                ret = uo.Remove();
                if (ret != 0)
                {
                    company.GetLastError(out ret, out errMsg);
                    app.SetStatusBarMessage(string.Format("Error removing object {0}", name),
                        SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    throw new Exception(errMsg);
                }
                app.StatusBar.SetSystemMessage(string.Format("Removed field {0}", name), 
                    SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        internal static void removeField(UserFieldsMD uf, string tableName, int fieldID, 
                            SAPbouiCOM.Application app, SAPbobsCOM.Company company)
        {
            int ret;
            string errMsg;
            if (uf.GetByKey(tableName, fieldID))
            {
                ret = uf.Remove();
                if (ret != 0)
                {
                    company.GetLastError(out ret, out errMsg);
                    app.SetStatusBarMessage(string.Format("Error removing field {0}-{1}", 
                        tableName, fieldID), SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    throw new Exception(errMsg);
                }
                app.StatusBar.SetSystemMessage(string.Format("Removed field {0}-{1}", 
                    tableName, fieldID), 
                    SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        internal static void removeTable(UserTablesMD ut, string name, SAPbouiCOM.Application app,
                                        SAPbobsCOM.Company company)
        {
            int ret;
            string errMsg;
            if (ut.GetByKey(name))
            {
                ret = ut.Remove();
                if (ret != 0)
                {
                    company.GetLastError(out ret, out errMsg);
                    app.SetStatusBarMessage("Error removing table " + name, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    throw new Exception(errMsg);
                }
                app.StatusBar.SetSystemMessage("Removed table " + name,
                    SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        private static void waitDoverApp()
        {
            Application app = new Application();
        }

        private class BootDoverHelper
        {
            internal Thread bootThread;
            internal Application app;
            internal BootDoverHelper(Application app)
            {
                this.app = app;
            }
            internal void Start()
            {
                try
                {
                    app.Run();
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
            internal void Shutdown()
            {
                app.ShutDownApp();
                bootThread.Join();
            }
        }

        internal static Application bootDover(Application doverApp)
        {
            if (bootDoverHelper != null)
                throw new Exception("Dover is running");

            bootDoverHelper = new BootDoverHelper(doverApp);
            Thread t = new Thread(bootDoverHelper.Start);
            bootDoverHelper.bootThread = t;
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            int count = 0;
            while (bootDoverHelper.app == null || !bootDoverHelper.app.Initialized)
            {
                Thread.Sleep(5000);
                if (count >= 60)
                    throw new Exception("5 minutes connection timeout");
                ++count;
            }

            return bootDoverHelper.app;
        }

        internal static void shutdownDover()
        {
            if (bootDoverHelper != null)
            {
                bootDoverHelper.Shutdown();
                bootDoverHelper = null;
            }
            var domains = DomainHelper.LoadedDomains;
            Assert.AreEqual(2, domains.Count());
        }

    }
}
