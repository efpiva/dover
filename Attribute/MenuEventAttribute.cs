using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false)]
    public class MenuEventAttribute : System.Attribute
    {
        public string UniqueUID { get; set; }

        internal System.Type OriginalType { get; set; }
    }
}
