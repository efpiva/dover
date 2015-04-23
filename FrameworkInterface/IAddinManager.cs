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

        bool CheckAddinConfiguration(string mainDll, out string datatable, out string addinName, out string addinNamespace);

        void LoadAddins(List<Model.AssemblyInformation> addins);

        string GetAddinChangeLog(string addinCode);

        AddinStatus GetAddinStatus(string addinCode);

        void InstallAddin(string addinCode);

        void ShutdownAddin(string addinCode);

        void StartAddin(string addinCode);

        bool Initialized { get; set; }
    }
}
