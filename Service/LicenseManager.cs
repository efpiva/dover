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
using Dover.Framework.DAO;
using Castle.Core.Logging;

namespace Dover.Framework.Service
{
    /// <summary>
    /// Stub class
    /// </summary>
    public class LicenseManager
    {
        private AssemblyDAO asmDAO;
        public ILogger Logger { get; set; }

        public LicenseManager(AssemblyDAO asmDAO)
        {
            this.asmDAO = asmDAO;
        }

        internal List<AssemblyInformation> ListAddins()
        {
            return asmDAO.GetAssembliesInformation("A");
        }

        internal bool SaveLicense(string xml)
        {
            return true;
            // fake implementation
        }

        internal DateTime GetAddInExpireDate(string module)
        {
            return DateTime.MaxValue;
        }

        internal bool AddInValid(string p)
        {
            // Logger.Error("Addin is not valid");
            return true;
            // Fake implementation
            // throw new NotImplementedException();
        }
    }
}
