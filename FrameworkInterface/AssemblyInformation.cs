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

namespace Dover.Framework.Model
{
    public enum AssemblyType
    {
        Core,
        Addin,
        Dependency
    }

    public class AssemblyInformation : MarshalByRefObject, IComparable<AssemblyInformation>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        internal int Major { get; set; }
        internal int Minor { get; set; }
        internal int Build { get; set; }
        internal int Revision { get; set; }

        public string Version
        {
            get
            {
                return Major.ToString() + "." + Minor.ToString() + "." + Build.ToString()
                    + "." + Revision.ToString();
            }
            set
            {
                int temp = 0;
                int index = value.IndexOf('.', 0);
                int lastIndex;
                if (index < 0)
                    return;

                int.TryParse(value.Substring(0, index), out temp);
                Major = temp;
                lastIndex = index + 1;

                index = value.IndexOf('.', lastIndex);
                if (index < 0)
                    return;

                temp = 0;
                int.TryParse(value.Substring(lastIndex, index - lastIndex), out temp);
                Minor = temp;
                lastIndex = index + 1;

                index = value.IndexOf('.', lastIndex);
                if (index < 0)
                    return;

                temp = 0;
                int.TryParse(value.Substring(lastIndex, index - lastIndex), out temp);
                Build = temp;
                lastIndex = index + 1;

                temp = 0;
                int.TryParse(value.Substring(lastIndex), out temp);
                Revision = temp;
            }
        }

        public string MD5 { get; set; }

        public DateTime Date { get; set; }

        public int Size { get; set; }

        public string Code { get; set; }

        public AssemblyType Type { get; set; }

        internal static string ConvertTypeToCode(AssemblyType type)
        {
            switch (type)
            {
                case AssemblyType.Addin:
                    return "A";
                case AssemblyType.Core:
                    return "C";
                case AssemblyType.Dependency:
                    return "D";
                default:
                    return "X";
            }
        }

        internal string TypeCode
        {
            get 
            {
                return AssemblyInformation.ConvertTypeToCode(Type);
            }
            set
            {
                switch (value)
                {
                    case "A":
                        Type = AssemblyType.Addin;
                        break;
                    case "C":
                        Type = AssemblyType.Core;
                        break;
                    default:
                        Type = AssemblyType.Dependency;
                        break;
                }
            }
        }

        public DateTime ExpireDate { get; set; }

        public string FileName { get; set; }

        internal List<AssemblyInformation> Dependencies { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name == null ? string.Empty : Name);
            sb.Append(" ");
            sb.Append(Version == null ? string.Empty : Version);
            return sb.ToString();
        }

        public int CompareTo(AssemblyInformation other)
        {
            if (Major < other.Major)
            {
                return -1;
            }
            else if (Major > other.Major)
            {
                return 1;
            }
            else
            {
                if (Minor < other.Minor)
                {
                    return -1;
                }
                else if (Minor > other.Minor)
                {
                    return 1;
                }
                else
                {
                    if (Build < other.Build)
                    {
                        return -1;
                    }
                    else if (Build > other.Build)
                    {
                        return 1;
                    }
                    else
                    {
                        if (Revision < other.Revision)
                        {
                            return -1;
                        }
                        else if (Revision > other.Revision)
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
