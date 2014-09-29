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
    public class AssemblyInformation : MarshalByRefObject, IComparable<AssemblyInformation>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        private int major;
        private int minor;
        private int build;
        private int revision;

        private string _version;

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                int index = _version.IndexOf('.', 0);
                int lastIndex;
                if (index < 0)
                    return;

                int.TryParse(_version.Substring(0, index), out major);
                lastIndex = index+1;

                index = _version.IndexOf('.', lastIndex);
                if (index < 0)
                    return;
                int.TryParse(_version.Substring(lastIndex, index-lastIndex), out minor);
                lastIndex = index+1;

                index = _version.IndexOf('.', lastIndex);
                if (index < 0)
                    return;
                int.TryParse(_version.Substring(lastIndex, index-lastIndex), out build);
                lastIndex = index+1;

                int.TryParse(_version.Substring(lastIndex), out revision);
            }
        }

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

        public int CompareTo(AssemblyInformation other)
        {
            if (major < other.major)
            {
                return -1;
            }
            else if (major > other.major)
            {
                return 1;
            }
            else
            {
                if (minor < other.minor)
                {
                    return -1;
                }
                else if (minor > other.minor)
                {
                    return 1;
                }
                else
                {
                    if (build < other.build)
                    {
                        return -1;
                    }
                    else if (build > other.build)
                    {
                        return 1;
                    }
                    else
                    {
                        if (revision < other.revision)
                        {
                            return -1;
                        }
                        else if (revision > other.revision)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
        }
    }
}
