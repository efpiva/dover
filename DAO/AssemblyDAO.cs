using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Model;

namespace AddOne.Framework.DAO
{
    public interface AssemblyDAO
    {
        byte[] GetAssembly(AssemblyInformation asm);

        List<AssemblyInformation> getAssembliesInformation(string type);

        AssemblyInformation GetAssemblyInformation(string asmName, string type);

        void SaveAssembly(AssemblyInformation currentAsm, byte[] asmBytes);

        void RemoveAssembly(string moduleName);

        void SaveAssemblyI18N(string moduleCode, string i18n, byte[] i18nAsm);

        bool AutoUpdateEnabled(AssemblyInformation asm);

        List<string> GetSupportedI18N(AssemblyInformation asm);

        byte[] GetI18NAssembly(AssemblyInformation asm, string i18n);
    }
}
