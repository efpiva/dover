using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Service;
using System.Globalization;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Dover.Framework.DAO
{
    public class LicenseDAOSQLImpl : LicenseDAO
    {
        private CryptoService cryptoService;
        private SAPbouiCOM.Application application;
        private BusinessOneDAO b1DAO;

        public LicenseDAOSQLImpl(CryptoService cryptoService, SAPbouiCOM.Application application,
            BusinessOneDAO b1DAO)
        {
            this.cryptoService = cryptoService;
            this.application = application;
            this.b1DAO = b1DAO;
        }


        public override string ReadLicense()
        {
            List<String> hexFile = b1DAO.ExecuteSqlForList<String>(
                String.Format("Select U_Resource from [@DOVER_LICENSE_BIN] ORDER BY Code"));
            StringBuilder sb = new StringBuilder();
            foreach (var hex in hexFile)
            {
                sb.Append(hex);
            }
            SoapHexBinary shb = SoapHexBinary.Parse(sb.ToString());
            return System.Text.Encoding.UTF8.GetString(shb.Value);
        }

        public override void SaveLicense(string xml)
        {
            string sql;
            int maxtext = 256000;
            int insertedText = 0;

            SoapHexBinary xmlBinToHex = new SoapHexBinary(System.Text.Encoding.UTF8.GetBytes(xml));
            var xmlHex = xmlBinToHex.ToString();

            b1DAO.ExecuteStatement("DELETE FROM [@DOVER_LICENSE_BIN]");
            for (int i = 0; i < xmlHex.Length / maxtext; i++)
            {
                string code = b1DAO.GetNextCode("DOVER_LICENSE_BIN");
                sql = String.Format("INSERT INTO [@DOVER_LICENSE_BIN] (Code, Name, U_Resource) VALUES ('{0}', '{1}', '{2}')",
                    code, code, xmlHex.Substring(i * maxtext, maxtext));
                b1DAO.ExecuteStatement(sql);
                insertedText += maxtext;
            }

            if (insertedText < xmlHex.Length)
            {
                string code = b1DAO.GetNextCode("DOVER_MODULES_BIN");
                sql = String.Format("INSERT INTO [@DOVER_LICENSE_BIN] (Code, Name, U_Resource) VALUES ('{0}', '{1}', '{2}')",
                    code, code, xmlHex.Substring(insertedText));
                b1DAO.ExecuteStatement(sql);
            }
        }

        public override string GetSystemID()
        {
            return application.Company.SystemId;
        }

        public override string GetInstallationID()
        {
            return application.Company.InstallationId;
        }

        public class ServerDate
        {
            public string Code { get; set; }
            public string Data { get; set; }
        };

        public override DateTime GetServerDate()
        {

            ServerDate serverCodeData = b1DAO.ExecuteSqlForObject<ServerDate>(
                "SELECT top 1 Code, U_Data Data FROM [@DOVER_LICENSE]");
            DateTime todayDate = b1DAO.ExecuteSqlForObject<DateTime>
                ("SELECT GETDATE()");
            DateTime retDate;


            if (serverCodeData == null)
            {
                string code = b1DAO.GetNextCode("DOVER_LICENSE");
                string date = todayDate.ToString("yyyyMMdd");
                b1DAO.ExecuteStatement(
                    string.Format(@"INSERT INTO [@DOVER_LICENSE] (Code, Name, U_Data)
                    VALUES('{0}', '{0}', '{1}')", code, cryptoService.Encrypt(date)));
                retDate = todayDate;
            }
            else
            {
                DateTime serverDate;
                DateTime.TryParseExact(cryptoService.Decrypt(serverCodeData.Data), "yyyyMMdd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out serverDate);
                if (todayDate > serverDate)
                {
                    b1DAO.ExecuteStatement(
                        string.Format("UPDATE [@DOVER_LICENSE] SET U_Data = '{0}' WHERE Code = '{1}'",
                        serverCodeData.Code, cryptoService.Encrypt(todayDate.ToString("yyyyMMdd"))));
                    retDate = todayDate;
                }
                else
                {
                    retDate = serverDate;
                }
            }

            return retDate;
        }
    }
}
