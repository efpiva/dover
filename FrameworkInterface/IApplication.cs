using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dover.Framework.Interface 
{
    internal interface IApplication
    {
        T Resolve<T>();

        void RunAddin();

        void RunInception();

        void ShutDownApp();
    }
}
