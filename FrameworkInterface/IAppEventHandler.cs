using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace Dover.Framework.Interface
{
    internal interface IAppEventHandler
    {
        void ShutDown();

        void sapApp_AppEvent(BoAppEventTypes EventType);


        void Reboot();
    }
}
