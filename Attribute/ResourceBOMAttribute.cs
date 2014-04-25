using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.Attribute
{
    public enum ResourceType
    {
        UserField,
        UserTable,
        UDO,
        FormattedSearch,
        UserQueries,
        QueryCategories
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class ResourceBOMAttribute : System.Attribute
    {
        public String ResourceName;
        public ResourceType Type;

        public ResourceBOMAttribute(String ResourceName, ResourceType Type)
        {
            this.ResourceName = ResourceName;
            this.Type = Type;
        }

        public override string ToString()
        {
            return String.Format("[ResourceName={0} ; Type = {1}]", ResourceName, Type);
        }

    }
}
