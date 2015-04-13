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
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Dover.Framework.Monad
{
    internal class MyXmlTextWriter : XmlTextWriter
    {
        internal MyXmlTextWriter(Stream stream)
            : base(stream, Encoding.Unicode)
        {

        }

        public override void WriteEndElement()
        {
            base.WriteFullEndElement();
        }
    }

    internal static class XMLClass
    {
        internal static string Serialize<T>(this T obj)
            where T : class
        {
            if (obj == null)
                return null;

            var listSerializer = new XmlSerializer(obj.GetType());
            var xnameSpace = new XmlSerializerNamespaces();
            xnameSpace.Add("", "");
            var stream = new MemoryStream();
            var writer = new MyXmlTextWriter(stream);

            listSerializer.Serialize(writer, obj, xnameSpace);
            stream.Position = 0;
            var streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }

        internal static V Deserialize<V>(this string xml)
            where V : class
        {
            var objSerializer = new XmlSerializer(typeof(V), "");
            return (V)objSerializer.Deserialize(GenerateStreamFromString(xml));
        }

        internal static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.Unicode);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
