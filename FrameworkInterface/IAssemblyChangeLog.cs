using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dover.Framework.Interface
{
    internal interface IAssemblyChangeLog
    {
        string GetAddinChangeLog(string addin);
    }
}
