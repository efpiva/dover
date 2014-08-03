using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Dover.Framework.Attribute;
using SAPbobsCOM;
using Castle.Core.Logging;

namespace Dover.Framework.Proxy
{
    public class TransactionProxy : IInterceptor
    {
        private Company company { get; set; }

        public ILogger Logger { get; set; }

        [ThreadStatic]
        int transactionNestedLevel = 0;

        public TransactionProxy(Company company)
        {
            this.company = company;
        }

        public void Intercept(IInvocation invocation)
        {
            var attrs = invocation.Method.GetCustomAttributes(typeof(TransactionAttribute), true);
            if (attrs.Count() > 0) // transactionAware
            {
                try
                {
                    if (!company.InTransaction)
                        company.StartTransaction();
                    transactionNestedLevel++;
                    invocation.Proceed();
                    transactionNestedLevel--;
                    if (company.InTransaction && transactionNestedLevel == 0)
                        company.EndTransaction(BoWfTransOpt.wf_Commit);
                }
                catch (Exception e)
                {
                    if (company.InTransaction)
                        company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    Logger.Debug(Messages.RollBack);
                    transactionNestedLevel = 0;
                    throw e;
                }
            }
        }
    }
}
