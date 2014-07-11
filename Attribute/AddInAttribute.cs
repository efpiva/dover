using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dover.Framework.Attribute
{
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class AddInAttribute : System.Attribute
    {
        public string InitMethod { get; set; }
        public string B1SResource { get; set; }
        public string Description { get; set; }
        public string i18n { get ; set; }
        public bool requireLicense { get; set; }

        public AddInAttribute()
        {
            requireLicense = true;
        }

    }
}
