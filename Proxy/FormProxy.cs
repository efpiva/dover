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
                if (invocation.InvocationTarget is DoverFormBase)
                {
                    IForm form = ((DoverFormBase)invocation.InvocationTarget).UIAPIRawForm;
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
