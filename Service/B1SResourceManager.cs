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
using System.Xml.Linq;
using Dover.Framework.Attribute;
using Castle.Core.Logging;
using System.Reflection;
using Dover.Framework.Monad;
using SAPbouiCOM;
using Dover.Framework.Form;
using Dover.Framework.DAO;

namespace Dover.Framework.Service
{
    public class B1SResourceManager : MarshalByRefObject
    {
        // assembly -> form Key. -> XML
        private Dictionary<string, Dictionary<string, XDocument>> formSRFResource = new Dictionary<string, Dictionary<string, XDocument>>();
        private HashSet<string> assemblyLoaded = new HashSet<string>();
        private ILogger Logger;
        private I18NService i18nService;

        public B1SResourceManager(ILogger Logger, I18NService i18nService)
        {
            this.Logger = Logger;
            this.i18nService = i18nService;
        }


        internal void Reset()
        {
            formSRFResource = new Dictionary<string, Dictionary<string, XDocument>>();
            assemblyLoaded = new HashSet<string>();
        }

        private XDocument LoadResourceForms(Assembly addinAsm, Type type, string resourceKey)
        {
            string asmKey = addinAsm.GetName().FullName;
            if (!assemblyLoaded.Contains(asmKey)) // Load AddInAttribute.
            {
                AddInAttribute addInAttribute = (from definedType in addinAsm.GetTypes()
                                                 from attribute in definedType.GetCustomAttributes(true)
                                                 where attribute is AddInAttribute
                                                 select (AddInAttribute)attribute).If(x => x.Count() > 0).With(x => x.First());
                if (addInAttribute != null && !string.IsNullOrWhiteSpace(addInAttribute.B1SResource))
                {
                    LoadAssemblyResources(addinAsm, addInAttribute);
                }
                assemblyLoaded.Add(asmKey);
            }

            var formAttributes =
                    (from attribute in type.GetCustomAttributes(true)
                        where attribute is DoverFormAttribute
                        select (DoverFormAttribute)attribute).ToList();

            foreach (var formAttribute in formAttributes)
            {
                LoadAssemblyFormResource(addinAsm, formAttribute);
            }

            if (!formSRFResource.ContainsKey(asmKey) || !formSRFResource[asmKey].ContainsKey(resourceKey))
            // we should have found the form at this point.
            {
                Logger.Warn(string.Format(Messages.ResourceNotFound, asmKey, resourceKey));
            }
            else
            {
                return formSRFResource[asmKey][resourceKey];
            }
            return null;
        }

        private void LoadAssemblyFormResource(Assembly asm, DoverFormAttribute addInAttribute)
        {
            if (string.IsNullOrWhiteSpace(addInAttribute.Resource) || asm.IsDynamic)
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
                    else
                    {
                        formDictionary[addInAttribute.Resource] = doc; // override b1s form. srf have higher priority.
                    }
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

        internal XDocument GetSystemFormXDoc(string assemblyName, string resourceKey, string formUID)
        {
            var doc = GetFormDocument(assemblyName, resourceKey);
            if (doc != null)
            {
                return ConfigureSystemForm(doc, formUID);
            }
            return null;
        }

        internal string GetSystemFormXML(string assemblyName, string resourceKey, string formUID)
        {
            var doc = GetFormDocument(assemblyName, resourceKey);
            if (doc != null)
            {
                return ConfigureSystemForm(doc, formUID).With(x => x.ToString());
            }
            return string.Empty;
        }

        private XDocument ConfigureSystemForm(XDocument origDoc, string formUID)
        {
            XDocument doc = new XDocument(origDoc); // do not touch original form. Modify a copy.
            var formattedElement = (from app in doc.Elements("Application")
                                    from forms in app.Elements("forms")
                                    from action in forms.Elements("action")
                                    from form in action.Elements("form")
                                    where action.Attribute("type").Value == "update"
                                            || action.Attribute("type").Value == "add"
                                    select form);

            if (formattedElement.Count() > 0)
            {
                XElement xmlForm = formattedElement.First();
                string title = xmlForm.Attribute("title").Return(x => x.Value, string.Empty);
                xmlForm.RemoveAttributes(); // we do not want to update form geometry. Just form UID.
                XAttribute uid = new XAttribute("uid", formUID);
                XAttribute titleAttr = new XAttribute("title", title);
                xmlForm.Add(uid);
                xmlForm.Add(titleAttr);

                return doc;
            }

            return null;
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

        internal void ConfigureFormXML(Assembly addinAsm, Type type, string resourceKey, string formType)
        {
            XDocument doc = LoadResourceForms(addinAsm, type, resourceKey);
            if (doc != null)
            {
                var formattedElement = (from app in doc.Elements("Application")
                                        from forms in app.Elements("forms")
                                        from action in forms.Elements("action")
                                        from form in action.Elements("form")
                                        where action.Attribute("type").Value == "add"
                                        select form);

                if (formattedElement.Count() > 0) // system form does not have add.
                {
                    formattedElement.First().Attribute("FormType").Value = formType;
                }

                ConfigureFormi18N(doc, addinAsm);
                ConfigureFormDataTableSQL(doc, addinAsm, type);
            }
        }

        private void ConfigureFormDataTableSQL(XDocument doc, Assembly addinAsm, Type type)
        {
            var sqlElements = (from descendant in doc.Descendants()
                                where descendant.Name.LocalName.ToUpper() == "QUERY"
                                   select descendant);

            foreach (var element in sqlElements)
            {
                string resource = element.Value.Trim();
                if (resource.IndexOf(" ") == -1)
                {
                    string sql = type.GetSQL(element.Value.Trim());
                    if (!string.IsNullOrEmpty(sql))
                        element.Value = sql;
                }
            }
        }

        private void ConfigureFormi18N(XDocument doc, Assembly addinAsm)
        {
            var i18nElements = (from descendant in doc.Descendants()
                                where descendant.Name.LocalName.ToUpper() == "FORM"
                                    || descendant.Name.LocalName.ToUpper() == "SPECIFIC"
                                    || descendant.Name.LocalName.ToUpper() == "GRIDCOLUMN"
                                    || descendant.Name.LocalName.ToUpper() == "COLUMN"
                                    || descendant.Name.LocalName.ToUpper() == "VALIDVALUE"
                                select descendant);

            foreach (var element in i18nElements)
            {
                switch (element.Name.LocalName.ToUpper())
                {
                    case "FORM":
                        element.Attribute("title").Do(x => x.Value = i18nService.GetLocalizedString(element.Attribute("title").Value, addinAsm));
                        break;
                    case "SPECIFIC":
                        element.Attribute("caption").Do(x => x.Value = i18nService.GetLocalizedString(element.Attribute("caption").Value, addinAsm));
                        break;
                    case "GRIDCOLUMN":
                        element.Attribute("Title").Do(x => x.Value = i18nService.GetLocalizedString(element.Attribute("Title").Value, addinAsm));
                        break;
                    case "COLUMN":
                        element.Attribute("title").Do(x => x.Value = i18nService.GetLocalizedString(element.Attribute("title").Value, addinAsm));
                        break;
                    case "VALIDVALUE":
                        element.Attribute("description").Do(x => x.Value = i18nService.GetLocalizedString(element.Attribute("description").Value, addinAsm));
                        break;
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
