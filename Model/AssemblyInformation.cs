using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Monad;

namespace AddOne.Framework.Model
{
    public class AssemblyInformation
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
