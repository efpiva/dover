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
using System.Windows.Forms;
using System.Reflection;
using SAPbobsCOM;
using Castle.Core.Logging;
using Dover.Framework.Monad;
using Dover.Framework.Remoting;

namespace Dover.Framework.Factory
{
    public class SAPServiceFactory
    {
        private static SAPbouiCOM.Application application;
        private static SAPbobsCOM.Company company;
        private static Sponsor<SAPbouiCOM.Application> applicationSponsor;
        private static Sponsor<SAPbobsCOM.Company> companySponsor;
        private static object threadLock = new System.Object();
        private static bool b1Connected = false;

        public static ILogger Logger { get; set; }

        private static void B1Connect(String version)
        {
            company = (SAPbobsCOM.Company)AppDomain.CurrentDomain.GetData("SAPCompany");
            application = (SAPbouiCOM.Application)AppDomain.CurrentDomain.GetData("SAPApplication");

            // inception!
            if (application != null && company != null)
            {
                applicationSponsor = new Sponsor<SAPbouiCOM.Application>(application);
                companySponsor = new Sponsor<SAPbobsCOM.Company>(company);
                return;
            }

            try
            {
                SetApplication();
                company = (SAPbobsCOM.Company)application.Company.GetDICompany();

                b1Connected = company.Connected;
            }
            catch (Exception er)
            {
                Logger.Fatal(String.Format(Messages.ConnectionError, er.Message), er);
            }
        }

        private static void SetApplication()
        {
            SboGuiApi sboGuiApi = (SboGuiApi)new SboGuiApiClass();
            sboGuiApi.Connect(GetConnectionString());
            application = sboGuiApi.GetApplication(-1);
        }

        private static string GetConnectionString()
        {
            string ret;
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                ret = Environment.GetCommandLineArgs()[1];
            }
            else
            {
                ret = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
            }

            AppDomain.CurrentDomain.SetData("AddOnePIPE", "addOne" + ret);

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
                string pipeName = (string)AppDomain.CurrentDomain.GetData("AddOnePIPE");

                if (application == null && company == null)
                    B1Connect(GetVersion());
                inception.SetData("SAPCompany", company);
                inception.SetData("SAPApplication", application);
                inception.SetData("AddOnePIPE", pipeName);
            }
        }
    }
}
