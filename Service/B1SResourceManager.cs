using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AddOne.Framework.Attribute;
using Castle.Core.Logging;
using System.Reflection;
using AddOne.Framework.Monad;

namespace AddOne.Framework.Service
{
    public class B1SResourceManager
    {
        Dictionary<string, XDocument> assemblyB1SResource = new Dictionary<string, XDocument>();

        private ILogger Logger;

        public B1SResourceManager(ILogger Logger)
        {
            this.Logger = Logger;

            var asms = AppDomain.CurrentDomain.GetAssemblies();
            List<Tuple<AddInAttribute, Assembly>> attributes = 
                    (from asm in asms
                                      from definedType in asm.GetTypes()
                                      from attribute in definedType.GetCustomAttributes(true)
                                      where attribute is AddInAttribute
                                      select new Tuple<AddInAttribute, Assembly>((AddInAttribute)attribute, asm)
                                      ).ToList();

            foreach(var tuple in attributes)
            {
                LoadAssemblyResources(tuple.Item2, tuple.Item1);
            }
        }

        private void LoadAssemblyResources(System.Reflection.Assembly asm, AddInAttribute addInAttribute)
        {
            using (var resource = asm.GetManifestResourceStream(addInAttribute.B1SResource))
            {
                if (resource != null)
                {
                    var doc = XDocument.Load(resource);
                    var key = asm.GetName().FullName;
                    if (!assemblyB1SResource.ContainsKey(key))
                    {
                        assemblyB1SResource.Add(key, doc);
                    }
                }
                else
                {
                    Logger.Warn(string.Format(Messages.ResourceNotFound, asm.GetName().FullName, addInAttribute.B1SResource));
                }
            }
        }


        internal string GetFormXML(string assemblyName, string resourceKey)
        {
            var attr = GetFormAttribute(assemblyName, resourceKey);
            if (attr != null)
            {
                return attr.Value;
            }
            return string.Empty;
        }

        internal void ConfigureFormXML(string assemblyName, string resourceKey, string formType)
        {
            XAttribute attribute = GetFormAttribute(assemblyName, resourceKey);
            var doc = XDocument.Load(XMLClass.GenerateStreamFromString(attribute.Value));
            var formattedElement = (from app in doc.Elements("Application")
                                    from forms in app.Elements("forms")
                                    from action in forms.Elements("action")
                                    from form in action.Elements("form")
                                    where action.Attribute("type").Value == "add"
                                    select form);

            if (formattedElement.Count() > 0) // system form and udo does not have add.
            {
                formattedElement.First().Attribute("FormType").Value = formType;
                attribute.Value = doc.ToString();
            }

        }

        private XAttribute GetFormAttribute(string assemblyName, string resourceKey)
        {
            XDocument doc;
            assemblyB1SResource.TryGetValue(assemblyName, out doc);
            if (doc != null)
            {
                var vsicreated = (from elem in doc.Descendants()
                                  where elem.Name == "project" &&
                                      elem.Attribute("name").Value == "VSIcreated"
                                  select elem);
                if (vsicreated == null)
                {
                    Logger.Error(string.Format(Messages.B1SResourceVSICreatedNotFound, assemblyName));
                    throw new ArgumentException(string.Format(Messages.B1SResourceVSICreatedNotFound, assemblyName));
                }
                var content = (from elem in vsicreated.First().Elements()
                               where elem.Name == "file" && elem.Attribute("name").Value == resourceKey
                               select elem);
                if (content == null)
                {
                    Logger.Error(string.Format(Messages.B1SResourceKeyNotFound, assemblyName, resourceKey));
                    throw new ArgumentException(string.Format(Messages.B1SResourceKeyNotFound, assemblyName, resourceKey));
                }
                return content.First().Element("content").Attribute("desc");
            }
            else
            {
                Logger.Error(string.Format(Messages.B1SResourceMissing, assemblyName));
                throw new ArgumentException(string.Format(Messages.B1SResourceMissing, assemblyName));
            }
        }
    }
}
