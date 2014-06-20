using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using AddOne.Framework.Monad;
using Castle.Core.Logging;
using AddOne.Framework.Log;

namespace AddOne.Framework.Service
{
    public class AddIni18n
    {
        public ILogger Logger { get; set; }

        internal string GetLocalizedString(string key, Assembly addin = null)
        {

            var index = key.Return(x => x, String.Empty).LastIndexOf(".");
            if (index < 0)
            {
                Logger.Error(String.Format(Messages.i18nError, key));
                return key.Return(x => x, String.Empty);
            }
            string typeName = key.Substring(0, index);
            index++;
            string propertyName = key.Substring(index);
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

            return resource.GetString(propertyName);
        }
    }
}
