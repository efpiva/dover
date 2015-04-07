using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SAPbobsCOM;
using Dover.Framework;
using System.Threading;

namespace FrameworkTest
{
    static class DoverSetup
    {
        internal static void CleanDover()
        {
            var sboGuiApi = new SAPbouiCOM.SboGuiApi(); 
            sboGuiApi.Connect("0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056");
            SAPbouiCOM.Application app = sboGuiApi.GetApplication();
            var company = (SAPbobsCOM.Company)app.Company.GetDICompany();

            try
            {
                var ut = (UserTablesMD)company.GetBusinessObject(BoObjectTypes.oUserTables);
                removeTable(ut, "DOVER_MODULES", app);
                removeTable(ut, "DOVER_MODULES_BIN", app);
                removeTable(ut, "DOVER_MODULES_DEP", app);
                removeTable(ut, "DOVER_MODULES_USER", app);
                removeTable(ut, "DOVER_LOGS", app);
                removeTable(ut, "DOVER_LICENSE_BIN", app);

                
                string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dover");
                if (Directory.Exists(appFolder))
                {
                    Directory.Delete(appFolder, true);
                }
            }
            finally
            {
                company.Disconnect();
            }
        }

        private static void removeTable(UserTablesMD ut, string name, SAPbouiCOM.Application app)
        {
            int ret;
            if (ut.GetByKey(name))
            {
                ret = ut.Remove();
                if (ret != 0)
                {
                    app.SetStatusBarMessage("Error removing table " + name, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    throw new Exception();
                }
                app.StatusBar.SetSystemMessage("Removed table " + name, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        private static void waitDoverApp()
        {
            Application app = new Application();
        }

        private class BootDoverHelper
        {
            internal Application app;
            internal BootDoverHelper()
            {
                app = null;
            }
            internal void Start()
            {
                app = new Application();
                app.Run();
            }
        }

        internal static Application bootDover()
        {
            BootDoverHelper bdh = new BootDoverHelper();
            Thread t = new Thread(bdh.Start);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            int count = 0;
            while (bdh.app == null || !bdh.app.Initialized)
            {
                Thread.Sleep(5000);
                if (count >= 60)
                    throw new Exception();
                ++count;
            }

            return bdh.app;
        }
    }
}
