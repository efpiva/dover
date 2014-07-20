using System.Collections.Generic;
using Dover.Framework.Attribute;
using SAPbouiCOM;

namespace Dover.Framework.DAO
{
    public abstract class BusinessOneUIDAO
    {
        internal abstract void ProcessMenuAttribute(List<MenuAttribute> menus);

        internal abstract IForm LoadFormBatchAction(string xml, string formType);

        internal abstract void LoadBatchAction(string xml);

        internal abstract IForm GetFormByUID(string formUID);
    }
}
