using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dover.Framework.Interface
{
    internal interface IAddinLoader
    {
        void StartMenu();

        void StartMenu(System.Reflection.Assembly assembly);

        void StartThis();
    }
}
