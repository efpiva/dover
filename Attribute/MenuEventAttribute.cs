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
    /// Used to bind a form of method to a specific menu UID.
    /// 
    /// When presented in a class, the class must be of type DoverBaseForm.
    /// 
    /// When presented in a method, the method will be called from Menu event.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false)]
    public class MenuEventAttribute : System.Attribute
    {
        /// <summary>
        /// MenuUID that will trigger the event.
        /// </summary>
        public string UniqueUID { get; set; }

        internal System.Type OriginalType { get; set; }

        internal System.Reflection.MethodInfo OriginalMethod { get; set; }
    }
}
