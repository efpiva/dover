using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using AddOne.Framework.Factory;
using log4net.Core;
using AddOne.Framework.Service;
using SAPbobsCOM;
using AddOne.Framework.DAO;

namespace AddOne.Framework.Log
{
    public class SAPAppender : AppenderSkeleton
    {
        private static MachineInformation machineInformation = new MachineInformation();
        public static BusinessOneDAO B1DAO { get; set; }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            string asm, version;
            var app = SAPServiceFactory.ApplicationFactory();
            GetAsmName(loggingEvent.LocationInformation.ClassName, out asm, out version);
            if (app != null)
                UIAPILog(loggingEvent, app, asm);
            if (B1DAO != null && loggingEvent.Level >= Level.Error)
                DIAPILog(loggingEvent, asm, version);
        }

        private void GetAsmName(string className, out string asmName, out string version)
        {
            Type objectType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                               from type in asm.GetTypes()
                               where type.IsClass && type.FullName == className
                               select type).First();
            Version ver = objectType.Assembly.GetName().Version;
            asmName = objectType.Assembly.GetName().Name;
            version = ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString()
                        + "." + ver.Revision;            
        }

        private void DIAPILog(LoggingEvent loggingEvent, string asm, string version)
        {
            try
            {
                string sqlTemplate = @"INSERT INTO [@GA_AO_LOGS] (Code, Name, U_Date, U_Hour, U_User, U_MachineName, U_MacAddress, U_IP,
                    U_Version, U_Assembly) Values ('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')";

                string code = B1DAO.GetNextCode("GA_AO_LOGS");
                DateTime date = DateTime.Now;
                string b1User = B1DAO.GetCurrentUser();

                string sql = String.Format(sqlTemplate, code, code, FormatDate(date), FormatHour(date), b1User,
                    machineInformation.MachineName, machineInformation.MacAddress, machineInformation.IP, version, asm);
                B1DAO.ExecuteStatement(sql);
            }
            catch
            {
                // Cannot log, can't do anything.
            }
        }

        private string FormatHour(DateTime date)
        {
            return (date.Hour * 100 + date.Minute).ToString();
        }

        private string FormatDate(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        private void UIAPILog(LoggingEvent loggingEvent, SAPbouiCOM.Application app, string asm)
        {
            string msg = String.Format("{0}: {1}", asm, loggingEvent.RenderedMessage);
            if (loggingEvent.Level == Level.Alert)
                app.StatusBar.SetText(msg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
            else if (loggingEvent.Level == Level.Info)
                app.StatusBar.SetText(msg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            else if (loggingEvent.Level == Level.Warn)
                app.StatusBar.SetText(msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            else
                app.StatusBar.SetText(msg, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

            if (loggingEvent.ExceptionObject != null)
            {
                app.MessageBox(String.Format("Error: {0}\nException: {1}\n{2}", msg, loggingEvent.ExceptionObject.Message,
                    loggingEvent.ExceptionObject.StackTrace.ToString()));
            }
        }
    }
}
