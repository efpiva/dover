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
            return Encoding.Unicode.GetString(stream.ToArray());
        }

        public static V Deserialize<V>(this string xml)
            where V : class
        {
            var objSerializer = new XmlSerializer(typeof(V), "");
            return (V)objSerializer.Deserialize(GenerateStreamFromString(xml));
        }

        private static Stream GenerateStreamFromString(string s)
        {
            // wront UTF-16 return from SAP.
            // TODO: testar tratamento utilizando no Serialize<T>.
            if (s.ToUpper().StartsWith("<?xml version=\"1.0\" encoding=\"UTF-16\"?>".ToUpper()))
                s = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + s.Substring(39);

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
