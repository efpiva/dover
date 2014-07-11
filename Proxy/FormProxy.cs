using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Castle.Core.Logging;
using Dover.Framework.Service;
using Dover.Framework.Form;
using SAPbouiCOM;
using System.Reflection;

namespace Dover.Framework.Proxy
{
    /// <summary>
    /// Implement Proxy for DoverFormBase. This proxy is reponsible for handling i18n and exception handler in events.
    /// </summary>
    public class FormProxy : IInterceptor
    {
        public ILogger Logger { get; set; }
        private I18NService i18nService;

        [ThreadStatic]
        private static bool i18nThread = false; // thread was configured as i18n.

        public FormProxy(I18NService i18nService)
        {
            this.i18nService = i18nService;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!i18nThread)
            {
                i18nService.ConfigureThreadI18n(System.Threading.Thread.CurrentThread);
                i18nThread = true;
            }

            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                if (invocation.InvocationTarget is DoverOneFormBase)
                {
                    IForm form = ((DoverOneFormBase)invocation.InvocationTarget).UIAPIRawForm;
                    if (form != null)
                        form.Freeze(false); // force unfreeze in case of error.
                }

                Assembly addinAssembly = invocation.Method.DeclaringType.Assembly;
                Version objVersion = addinAssembly.GetName().Version;
                String addInName = addinAssembly.GetName().Name;
                String addInVersion = objVersion.Major.ToString() + "." + objVersion.Minor.ToString() + "." + objVersion.Build.ToString()
                            + "." + objVersion.Revision;

                Logger.Error(String.Format(Messages.AddInError, addInName, addInVersion), e);
            }
        }
    }
}
