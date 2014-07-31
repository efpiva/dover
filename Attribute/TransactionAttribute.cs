using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;

namespace Dover.Framework.Attribute
{
    public class TransactionAttribute : InterceptorAttribute
    {
        public TransactionAttribute() : base ("transactionProxy")
        {
        }
    }
}
