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

namespace Dover.Framework.Attribute
{
    /// <summary>
    /// AddIn general information and configuration.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class AddInAttribute : System.Attribute
    {
        /// <summary>
        /// If AddIn need to run code during startUp, place here one method name in the current class that should be called.
        /// 
        /// The need to have no parameter and return void.
        /// </summary>
        public string InitMethod { get; set; }
        /// <summary>
        /// Fully Qualified Name of the embedded resource that has the b1s compiled XML output file.
        /// 
        /// Note that you should emebed the compiled XML, not the project file. If you're in doubt, check if the
        /// XML has a project name called VSICreated. If yes, it's the compiled one.
        /// </summary>
        public string B1SResource { get; set; }
        /// <summary>
        /// Description of the Addin
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Fully Qualified Name of the i18n resource that has the Description internationalized.
        /// </summary>
        public string i18n { get ; set; }
        /// <summary>
        /// Name of the addin
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Fully Qualified Name of the resource that has the ChangeLog information
        /// </summary>
        public string ChangeLogResource { get; set; }

        /// <summary>
        /// Fully Qualified Name of the resource that has the public key for this addin.
        /// </summary>
        public string LicenseFile { get; set; }

        /// <summary>
        /// Namespace for this addin. Used for license control.
        /// </summary>
        public string Namespace { get; set; }
    }
}
