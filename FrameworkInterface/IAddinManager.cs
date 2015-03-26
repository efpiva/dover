using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Service;

namespace Dover.Framework.Service
{
    internal enum AddinStatus
    {
        Running,
        Stopped
    }
}

namespace Dover.Framework.Interface
{
    internal interface IAddinManager
    {
        void LogError(string p);

        void ShutdownAddins();

        void ConfigureAddinsI18N();

        bool CheckAddinConfiguration(string mainDll, out string datatable);

        void LoadAddins(List<Model.AssemblyInformation> addins);

        string GetAddinChangeLog(string module);

        AddinStatus GetAddinStatus(string name);

        void InstallAddin(string moduleName);

        void ShutdownAddin(string moduleName);

        void StartAddin(string moduleName);
    }
}
