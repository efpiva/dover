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
using Dover.Framework.Monad;

namespace Dover.Framework.Model
{
    public class AssemblyInformation : MarshalByRefObject
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }

        public string MD5 { get; set; }

        public DateTime Date { get; set; }

        public int Size { get; set; }

        public string Code { get; set; }

        public string Type { get; set; }

        public DateTime ExpireDate { get; set; }

        public string FileName { get; set; }

        public override string ToString()
        {
            return Name.Return(x => x, string.Empty) + " " + Version.Return(x => x, string.Empty);
        }
    }
}
