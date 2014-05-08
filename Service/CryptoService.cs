using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Castle.Core.Logging;
using AddOne.Framework.Monad;

namespace AddOne.Framework.Service
{
    public class CryptoService
    {
        public ILogger Logger { get; set; }
        private string ENCRYPTION_KEY;
        private byte[] key;
        private byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };

        public CryptoService(SAPbouiCOM.Application app)
        {
            string _systemNumer = app.Company.SystemId.Trim();
            string _instalationNumber = app.Company.InstallationId.Trim();

            ENCRYPTION_KEY = _systemNumer + _instalationNumber;
        }

        internal string Decrypt(string stringToDecrypt)
        {
            byte[] inputByteArray = new byte[stringToDecrypt.Length];
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(ENCRYPTION_KEY.Left(8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt.Replace(" ", "+"));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV),
                    CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                Logger.Error(Messages.UnhandledCrypto, e);
                throw e;
            }
        }

        internal string Encrypt(string stringToEncrypt)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(ENCRYPTION_KEY.Left(8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                Logger.Error(Messages.UnhandledCrypto, e);
                throw e;
            }
        }
    }
}
