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
using AddOne.Framework.Form;

namespace AddOne.Framework.Service
{
    public class B1SResourceManager : MarshalByRefObject
    {
        // assembly -> form Key. -> XML
        Dictionary<string, Dictionary<string, XDocument>> formSRFResource = new Dictionary<string, Dictionary<string, XDocument>>();
        HashSet<string> assemblyLoaded = new HashSet<string>();

        private string formUpdateTemplate = @"<?xml version=""1.0"" encoding=""UTF-16""?>
<Application>
  <forms>
    <action type=""update"">
      <form uid="""">
        <datasources/>
        <Menus />
        <items>
          <action type=""update"">
          </action>
        </items>
        <ChooseFromListCollection/>
      </form>
    </action>
  </forms>
</Application>";

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
                                                 select (AddInAttribute)attribute).First();
                if (!string.IsNullOrWhiteSpace(addInAttribute.B1SResource))
                {
                    LoadAssemblyResources(addinAsm, addInAttribute);
                }
                assemblyLoaded.Add(asmKey);
            }

            var formAttributes =
                    (from attribute in type.GetCustomAttributes(true)
                        where attribute is FormAttribute
                        select (FormAttribute)attribute).ToList();

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

        private void LoadAssemblyFormResource(Assembly asm, FormAttribute addInAttribute)
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
                    ParseB1SUDOFormSRF(doc, addInAttribute, key, formDictionary);
                }
                else
                {
                    Logger.Warn(string.Format(Messages.ResourceNotFound, asm.GetName().FullName, addInAttribute.B1SResource));
                }
            }
        }

        private void ParseB1SUDOFormSRF(XDocument doc, AddInAttribute addInAttribute, string assemblyName,
                Dictionary<string, XDocument> formDictionary)
        {
            if (doc != null)
            {
                var addonfiles = (from elem in doc.Descendants()
                                  where elem.Name == "project" &&
                                      elem.Attribute("type").Value == "Add-on"
                                  select elem);
                if (addonfiles == null)
                {
                    Logger.Error(string.Format(Messages.B1SResourceVSICreatedNotFound, assemblyName));
                    throw new ArgumentException(string.Format(Messages.B1SResourceVSICreatedNotFound, assemblyName));
                }
                var contents = (from elem in addonfiles.First().Elements()
                                where elem.Name == "file"
                                select elem);
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

                if (typeof(AddOneUDOFormBase).IsAssignableFrom(type))
                {
                    ConfigureUDOi18N(doc, addinAsm, resourceKey);
                }
                else
                {
                    ConfigureUserAndSystemi18N(doc, addinAsm);
                }

            }
        }

        private void ConfigureUDOi18N(XDocument doc, Assembly addinAsm, string resourceKey)
        {
            List<XElement> translatedElements = new List<XElement>();
            Dictionary<string, XElement> grids = new Dictionary<string,XElement>();

            string i18nTitle = string.Empty;

            var i18nElements = (from descendant in doc.Descendants()
                                where descendant.Name.LocalName.ToUpper() == "FORM"
                                    || descendant.Name.LocalName.ToUpper() == "SPECIFIC"
                                    || descendant.Name.LocalName.ToUpper() == "GRIDCOLUMN"
                                select descendant);

            foreach (var element in i18nElements)
            {
                switch (element.Name.LocalName.ToUpper())
                {
                    case "FORM":
                        i18nTitle = ProcessUDOForm(element, addinAsm);
                        break;
                    case "SPECIFIC":
                        XElement specificElement = ProcessUDOItem(element, addinAsm);
                        if (specificElement != null)
                            translatedElements.Add(specificElement);
                        break;
                    case "GRIDCOLUMN":
                        ProcessGridElement(grids, element, addinAsm);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(i18nTitle) || translatedElements.Count > 0 || grids.Count > 0)
            {
                formSRFResource[addinAsm.GetName().FullName][resourceKey] =
                    CreateUDOXDocument(i18nTitle, translatedElements, grids);
            }
            else
            {
                formSRFResource[addinAsm.GetName().FullName].Remove(resourceKey);
            }
        }

        private XDocument CreateUDOXDocument(string i18nTitle, List<XElement> translatedElements, Dictionary<string, XElement> grids)
        {
            XDocument doc = XDocument.Parse(formUpdateTemplate);
            if (!string.IsNullOrEmpty(i18nTitle))
            {
                doc.Element("forms").Element("action").Element("form").SetAttributeValue("Title", i18nTitle);
            }
            XElement itemUpdate = doc.Element("Application").Element("forms").Element("action").Element("form").Element("items").Element("action");
            foreach (var element in translatedElements)
            {
                itemUpdate.Add(element);
            }
            foreach (var key in grids.Keys)
            {
                itemUpdate.Add(grids[key]);
            }
            return doc;
        }

        private void ProcessGridElement(Dictionary<string, XElement> grids, XElement element, Assembly addinAsm)
        {
            XElement grid = element.With(x => x.Parent).With(x => x.Parent).With(x => x.Parent);
            if (grid != null && grid.Attribute("uid") != null)
            {
                string title = element.Attribute("Title").Value;
                string i18nTitle = i18nService.GetLocalizedString(title, addinAsm);
                if (title != i18nTitle)
                {
                    XElement i18nGrid;
                    string uid = grid.Attribute("uid").Value;
                    if (!grids.TryGetValue(uid, out i18nGrid))
                    {
                        i18nGrid = CreateGridXElement(uid);
                        grids.Add(uid, i18nGrid);
                    }

                    XElement gridColumns = i18nGrid.Element("specific").Element("GridColumns");
                    XElement gridColumn = new XElement("GridColumn");
                    gridColumn.SetAttributeValue("UniqueID", element.Attribute("UniqueID").Value);
                    gridColumn.SetAttributeValue("Title", i18nTitle);
                    gridColumn.Add(gridColumn);
                }
            }
        }

        private XElement CreateGridXElement(string uid)
        {
            XElement grid = new XElement("item");
            grid.SetAttributeValue("uid", uid);
            XElement specific = new XElement("specific");
            XElement gridColumns = new XElement("GridColumns");
            specific.Add(gridColumns);
            grid.Add(specific);
            return grid;
        }

        private XElement ProcessUDOItem(XElement captionElement,
            Assembly addinAsm)
        {
            string name = (from attribute in captionElement.Attributes()
                           where attribute.Name.LocalName.ToUpper() == "CAPTION"
                           select attribute.Value).If(x => x.Count() > 0).Return(x => x.First(), string.Empty);

            string i18nName = i18nService.GetLocalizedString(name, addinAsm);

            if (name != i18nName && captionElement.Parent != null)
            {
                string uid = (from attribute in captionElement.Parent.Attributes()
                               where attribute.Name.LocalName.ToUpper() == "UNIQUEID"
                               select attribute.Value).If(x => x.Count() > 0).Return(x => x.First(), string.Empty);

                if (!string.IsNullOrEmpty(uid)) 
                {
                    XElement returnValue = new XElement("item");
                    returnValue.SetAttributeValue("uid", uid);
                    XElement caption = new XElement("specific");
                    caption.SetAttributeValue("caption", i18nName);
                    returnValue.Add(caption);
                    return returnValue;
                }
            }

            return null;
        }

        private string ProcessUDOForm(XElement element, Assembly addinAsm)
        {
            string title = (from attribute in element.Attributes()
                            where attribute.Name.LocalName.ToUpper() == "TITLE"
                            select attribute.Value).First();
            string i18nTitle = i18nService.GetLocalizedString(title, addinAsm);
            return (title == i18nTitle) ? string.Empty : i18nTitle;
        }

        private void ConfigureUserAndSystemi18N(XDocument doc, Assembly addinAsm)
        {
            var i18nElements = (from descendant in doc.Descendants()
                                where descendant.Name.LocalName.ToUpper() == "FORM"
                                    || descendant.Name.LocalName.ToUpper() == "SPECIFIC"
                                    || descendant.Name.LocalName.ToUpper() == "GRIDCOLUMN"
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
