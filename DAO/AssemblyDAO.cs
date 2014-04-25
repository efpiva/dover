using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Model.Assembly;

namespace AddOne.Framework.DAO
{
    public interface AssemblyDAO
    {
        List<AssemblyInformation> GetAddinsAssemblies();

        byte[] GetAssembly(AssemblyInformation asm);

        String GetB1StudioResource(AssemblyInformation asm);

        AssemblyInformation GetCoreAssembly(string asmFile);

        void SaveAssembly(AssemblyInformation currentAsm, byte[] asmBytes);
    }
}
