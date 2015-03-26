using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dover.Framework.Interface
{
    internal interface IEventDispatcher
    {
        void UnregisterEvents();

        void RegisterEvents();
    }
}
