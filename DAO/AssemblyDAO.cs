/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
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

        internal abstract List<AssemblyInformation> GetAssembliesInformation(AssemblyType type);

        internal abstract AssemblyInformation GetAssemblyInformation(string asmName, AssemblyType type);

        internal abstract void SaveAssembly(AssemblyInformation currentAsm, byte[] asmBytes);

        internal abstract void RemoveAssembly(string code);

        internal abstract bool AutoUpdateEnabled(AssemblyInformation asm);

        internal abstract void SaveAssemblyDependency(AssemblyInformation newAsm, AssemblyInformation dependency, byte[] dependencyBytes);

        internal abstract List<AssemblyInformation> GetAutoUpdateAssemblies();

        internal abstract List<AssemblyInformation> GetDependencies(AssemblyInformation asm);

        internal abstract int GetDependencyCount(AssemblyInformation dep);

        internal abstract string GetDependencyCode(string md5);

        internal abstract void SaveAssemblyDependency(AssemblyInformation newAsm, string dependencyCode);

        internal abstract void DeleteOrphanDependency();
    }
}
