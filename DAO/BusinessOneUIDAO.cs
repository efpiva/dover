using System.Collections.Generic;
using AddOne.Framework.Attribute;
using SAPbouiCOM;

namespace AddOne.Framework.DAO
{
    public abstract class BusinessOneUIDAO
    {
        public abstract void ProcessMenuAttribute(List<MenuAttribute> menus);

        internal abstract IForm LoadFormBatchAction(string xml);

        internal abstract void LoadBatchAction(string xml);

        internal abstract IForm GetFormByUID(string formUID);
    }
}
