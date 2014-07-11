using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dover.Framework.Monad;
using Castle.Core.Logging;
using Dover.Framework.Log;
using SAPbouiCOM;
using System.Threading;
using System.Globalization;

namespace Dover.Framework.Service
{
    public class I18NService
    {
        public ILogger Logger { get; set; }
        private SAPbouiCOM.Application app;

        public I18NService(SAPbouiCOM.Application app)
        {
            this.app = app;
        }

        internal string GetLocalizedString(string key, Assembly addin = null)
        {
            if (key == null)
                return string.Empty;

            var index = key.LastIndexOf(".");
            if (index < 0)
            {
                return key;
            }
            string typeName = key.Substring(0, index);
            index++;
            string propertyName = key.Substring(index).Trim();
            var assembly = (addin == null) ? 
                AppDomain.CurrentDomain.Load((string)AppDomain.CurrentDomain.GetData("assemblyName")) : addin;

            // TODO: cache e resourceCulture.
            var resource = new System.Resources.ResourceManager(typeName, assembly);

            if (resource != null)
            {
                Logger.Debug(DebugString.Format(Messages.GetLocalizedStringFoundResource, key));
            }
            else
            {
                Logger.Debug(DebugString.Format(Messages.GetLocalizedStringNotFoundResource, key));
            }

            try
            {
                string value = resource.GetString(propertyName, System.Threading.Thread.CurrentThread.CurrentUICulture);
                if (string.IsNullOrEmpty(value))
                    return key;
                return value;
            }
            catch (Exception)
            {
                return key;
            }
        }

        internal void ConfigureThreadI18n(Thread thread)
        {
            string cultureName = GetCultureName();
            thread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
        }

        private string GetCultureName()
        {
            string cultureName = "en-US"; // default if not found.
            switch (app.Language)
            {
                case BoLanguages.ln_Chinese:
                    cultureName = "zh";
                    break;
                case BoLanguages.ln_Czech_Cz:
                    cultureName = "cs-CZ";
                    break;
                case BoLanguages.ln_Danish:
                    cultureName = "da";
                    break;
                case BoLanguages.ln_Dutch:
                    cultureName = "nl";
                    break;
                case BoLanguages.ln_English:
                case BoLanguages.ln_English_Cy:
                case BoLanguages.ln_English_Sg:
                    cultureName = "en-US";
                    break;
                case BoLanguages.ln_English_Gb:
                    cultureName = "en-GB";
                    break;
                case BoLanguages.ln_Finnish:
                    cultureName = "fi-FI";
                    break;
                case BoLanguages.ln_French:
                    cultureName = "fr";
                    break;
                case BoLanguages.ln_German:
                    cultureName = "de-DE";
                    break;
                case BoLanguages.ln_Greek:
                    cultureName = "el-GR";
                    break;
                case BoLanguages.ln_Hebrew:
                    cultureName = "he-IL";
                    break;
                case BoLanguages.ln_Hungarian:
                    cultureName = "hu-HU";
                    break;
                case BoLanguages.ln_Italian:
                    cultureName = "it-IT";
                    break;
                case BoLanguages.ln_Japanese_Jp:
                    cultureName = "ja-JP";
                    break;
                case BoLanguages.ln_Korean_Kr:
                    cultureName = "ko-KR";
                    break;
                case BoLanguages.ln_Norwegian:
                    cultureName = "no";
                    break;
                case BoLanguages.ln_Polish:
                    cultureName = "pl-PL";
                    break;
                case BoLanguages.ln_Portuguese:
                    cultureName = "pt-PT";
                    break;
                case BoLanguages.ln_Portuguese_Br:
                    cultureName = "pt-BR";
                    break;
                case BoLanguages.ln_Russian:
                    cultureName = "ru-RU";
                    break;
                case BoLanguages.ln_Serbian:
                    cultureName = "sr";
                    break;
                case BoLanguages.ln_Slovak_Sk:
                    cultureName = "sk-SK";
                    break;
                case BoLanguages.ln_Spanish:
                    cultureName = "es-ES";
                    break;
                case BoLanguages.ln_Spanish_Ar:
                case BoLanguages.ln_Spanish_La:
                case BoLanguages.ln_Spanish_Pa:
                    cultureName = "es-AR";
                    break;
                case BoLanguages.ln_Swedish:
                    cultureName = "sv-SE";
                    break;
                case BoLanguages.ln_TrdtnlChinese_Hk:
                    cultureName = "zh-HK";
                    break;
                case BoLanguages.ln_Turkish_Tr:
                    cultureName = "tr-TR";
                    break;
            }
            return cultureName;
        }

        internal bool IsValidi18NCode(string i18n)
        {
            if (i18n.Length > 2)
                i18n = i18n.Substring(0, 2);
            switch (i18n)
            {
                case "zh":
                case "cs":
                case "da":
                case "nl":
                case "en":
                case "fi":
                case "fr":
                case "de":
                case "el":
                case "he":
                case "hu":
                case "it":
                case "ja":
                case "ko":
                case "no":
                case "pl":
                case "pt":
                case "ru":
                case "sr":
                case "sk":
                case "es":
                case "sv":
                case "tr":
                    return true;
            }
            return false;
        }
    }
}
