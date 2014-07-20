using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Model;

namespace Dover.Framework.DAO
{
    public abstract class AssemblyDAO
    {
        internal abstract byte[] GetAssembly(AssemblyInformation asm);

        internal abstract List<AssemblyInformation> getAssembliesInformation(string type);

        internal abstract AssemblyInformation GetAssemblyInformation(string asmName, string type);

        internal abstract void SaveAssembly(AssemblyInformation currentAsm, byte[] asmBytes);

        internal abstract void RemoveAssembly(string moduleName);

        internal abstract void SaveAssemblyI18N(string moduleCode, string i18n, byte[] i18nAsm);

        internal abstract bool AutoUpdateEnabled(AssemblyInformation asm);

        internal abstract List<string> GetSupportedI18N(AssemblyInformation asm);

        internal abstract byte[] GetI18NAssembly(AssemblyInformation asm, string i18n);
    }
}
