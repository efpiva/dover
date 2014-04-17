using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AddOne.Framework.Model.SAP
{
    public class AdminInfo
    {
        public string Object;
        public int Version;
    }

    public class BO
    {
        public AdminInfo AdmInfo;
    }

}
