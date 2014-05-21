using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;


namespace AddOne.Framework.IPC
{
    [ServiceContract]
    public interface InceptionServer
    {
        [OperationContract]
        void ShutdownAddins();
    }
}
