using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AddOne.Framework.Attribute;
using Castle.Core.Logging;
using System.Reflection;
using AddOne.Framework.Monad;
using SAPbouiCOM;
using SAPbouiCOM.Framework;

namespace AddOne.Framework.Service
{
    public class B1SResourceManager
    {
        // assembly -> form Key. -> XML
        Dictionary<string, Dictionary<string, XDocument>> formSRFResource = new Dictionary<string, Dictionary<string, XDocument>>();

        private ILogger Logger;

        public B1SResourceManager(ILogger Logger)
        {
            this.Logger = Logger;

            var asms = AppDomain.CurrentDomain.GetAssemblies();
            var addInAttributes = 
                    (from asm in asms
                                      from definedType in asm.GetTypes()
                                      from attribute in definedType.GetCustomAttributes(true)
                                      where attribute is AddInAttribute
                                      select new {Attribute=(AddInAttribute)attribute, Assembly=asm}
                                      ).ToList();

            foreach (var tuple in addInAttributes)
            {
                if (tuple.Attribute.B1SResource == null)
                    continue; // ignore addins that use SRF.
                LoadAssemblyResources(tuple.Assembly, tuple.Attribute);
            }

            var formAttributes =
                    (from asm in asms
                        from definedType in asm.GetTypes()
                        from attribute in definedType.GetCustomAttributes(true)
                        where attribute is FormAttribute
                        select new { Attribute = (FormAttribute)attribute, Assembly = asm }
                                        ).ToList();

            foreach (var tuple in formAttributes)
            {
                LoadAssemblyFormResource(tuple.Assembly, tuple.Attribute);
            }
        }

        private void LoadAssemblyFormResource(Assembly asm, FormAttribute addInAttribute)
        {
            if (string.IsNullOrWhiteSpace(addInAttribute.Resource))
                return;

            using (var resource = asm.GetManifestResourceStream(addInAttribute.Resource))
            {
                if (resource != null)
                {
                    var doc = XDocument.Load(resource);
                    var key = asm.GetName().FullName;
                    Dictionary<string, XDocument> formDictionary;
                    if (!formSRFResource.ContainsKey(key))
                    {
                        formDictionary = new Dictionary<string, XDocument>();
                        formSRFResource.Add(key, formDictionary);
                    }
                    else
                    {
                        formDictionary = formSRFResource[key];
                    }
                    if (!formDictionary.ContainsKey(addInAttribute.Resource))
                    {
                        formDictionary.Add(addInAttribute.Resource, doc);
                    }
                }
                else
                {
                    Logger.Warn(string.Format(Messages.ResourceNotFound, asm.GetName().FullName, addInAttribute.Resource));
                }
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

                    Dictionary<string, XDocument> formDictionary;
                    if (!formSRFResource.ContainsKey(key))
                    {
                        formDictionary = new Dictionary<string, XDocument>();
                        formSRFResource.Add(key, formDictionary);
                    }
                    else
                    {
                        formDictionary = formSRFResource[key];
                    }

                    ParseB1SFormSRF(doc, addInAttribute, key, formDictionary);
                }
                else
                {
                    Logger.Warn(string.Format(Messages.ResourceNotFound, asm.GetName().FullName, addInAttribute.B1SResource));
                }
            }
        }

        private void ParseB1SFormSRF(XDocument doc, AddInAttribute addInAttribute, string assemblyName,
                                Dictionary<string, XDocument> formDictionary)
        {
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
                var contents = (from elem in vsicreated.First().Elements()
                               where elem.Name == "file"
                               select elem).ToList();
                if (contents == null)
                {
                    Logger.Error(string.Format(Messages.B1SResourceKeyNotFound, assemblyName, "*"));
                    throw new ArgumentException(string.Format(Messages.B1SResourceKeyNotFound, assemblyName, "*"));
                }

                foreach (var content in contents)
                {
                    string name = content.Attribute("name").Value;
                    if (!formDictionary.ContainsKey(name))
                    {
                        formDictionary.Add(name, XDocument.Parse(content.Element("content").Attribute("desc").Value));
                    }
                }
            }
            else
            {
                Logger.Error(string.Format(Messages.B1SResourceMissing, assemblyName));
                throw new ArgumentException(string.Format(Messages.B1SResourceMissing, assemblyName));
            }
        }

        internal string GetSystemFormXML(string assemblyName, string resourceKey, string formUID, IForm sysForm)
        {
            var doc = GetFormDocument(assemblyName, resourceKey);
            if (doc != null)
            {
                return ConfigureSystemForm(doc, formUID, sysForm);
            }
            return string.Empty;
        }

        private string ConfigureSystemForm(XDocument doc, string formUID, IForm sysForm)
        {
            var formattedElement = (from app in doc.Elements("Application")
                                    from forms in app.Elements("forms")
                                    from action in forms.Elements("action")
                                    from form in action.Elements("form")
                                    where action.Attribute("type").Value == "update"
                                    select form);

            if (formattedElement.Count() > 0) // system form and udo does not have add.
            {
                XElement xmlForm = formattedElement.First();
                xmlForm.RemoveAttributes(); // we do not want to update form geometry. Just form UID.
                XAttribute uid = new XAttribute("uid", formUID);
                xmlForm.Add(uid);

                return doc.ToString();
            }

            return string.Empty;
        }

        internal string GetFormXML(string assemblyName, string resourceKey)
        {
            var doc = GetFormDocument(assemblyName, resourceKey);
            if (doc != null)
            {
                return doc.ToString();
            }
            return string.Empty;
        }

        internal void ConfigureFormXML(string assemblyName, string resourceKey, string formType)
        {
            XDocument doc = GetFormDocument(assemblyName, resourceKey);
            if (doc != null)
            {
                var formattedElement = (from app in doc.Elements("Application")
                                        from forms in app.Elements("forms")
                                        from action in forms.Elements("action")
                                        from form in action.Elements("form")
                                        where action.Attribute("type").Value == "add"
                                        select form);

                if (formattedElement.Count() > 0) // system form and udo does not have add.
                {
                    formattedElement.First().Attribute("FormType").Value = formType;
                }
            }
        }

        private XDocument GetFormDocument(string assemblyName, string resourceKey)
        {
            XDocument doc;
            Dictionary<string, XDocument> assemblyForms;
            if (formSRFResource.TryGetValue(assemblyName, out assemblyForms))
            {
                if (assemblyForms.TryGetValue(resourceKey, out doc))
                {
                    return doc;
                }
            }
            return null;
        }
    }
}
