using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AddOne.Framework
{
    public class BootStrap : MarshalByRefObject
    {
        public void Run()
        {
            String location = Assembly.GetEntryAssembly().Location;
        }
    }
}
