using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.Model.Assembly
{
    public class AssemblyInformation
    {
        public string Name { get; set; }

        public string ResourceName { get; set; }

        public string Version { get; set; }

        public string MD5 { get; set; }

        public DateTime Date { get; set; }

        public int Size { get; set; }

        public string Code { get; set; }

        public string Type { get; set; }
    }
}
