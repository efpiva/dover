using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dover.Framework.Attribute
{
    public enum ResourceType
    {
        UserField,
        UserTable,
        UDO
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class ResourceBOMAttribute : System.Attribute, IComparable<ResourceBOMAttribute>
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

        int IComparable<ResourceBOMAttribute>.CompareTo(ResourceBOMAttribute other)
        {
            if (this.Type == other.Type)
                return ResourceName.CompareTo(other.ResourceName);

            if (this.Type == ResourceType.UserTable)
                return -1;

            if (this.Type == ResourceType.UserField && other.Type == ResourceType.UDO)
                return -1;

            return 1;
        }
    }
}
