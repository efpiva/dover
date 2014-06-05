using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Model;

namespace AddOne.Framework.DAO
{
    public interface AssemblyDAO
    {
        List<AssemblyInformation> GetAddinsAssemblies();

        byte[] GetAssembly(AssemblyInformation asm);

        AssemblyInformation GetCoreAssembly(string asmFile);

        void SaveAssembly(AssemblyInformation currentAsm, byte[] asmBytes, byte[] b1SResource);

        AssemblyInformation GetAddInAssembly(string p);

        List<AssemblyInformation> GetCoreAssemblies();

        void RemoveAsm(string moduleName);
    }
}
