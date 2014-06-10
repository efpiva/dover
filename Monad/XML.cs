using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace AddOne.Framework.Monad
{
    internal class MyXmlTextWriter : XmlTextWriter
    {
        public MyXmlTextWriter(Stream stream)
            : base(stream, Encoding.Unicode)
        {

        }

        public override void WriteEndElement()
        {
            base.WriteFullEndElement();
        }
    }

    public static class XMLClass
    {
        public static string Serialize<T>(this T obj)
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

        public static V Deserialize<V>(this string xml)
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
