using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dover.Framework.Attribute
{
    /// <summary>
    /// 
    /// DoverFormAttribute is used to annotate forms and indicate what is the formType and the Resource for the corresponding Form.
    /// 
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class DoverFormAttribute : System.Attribute
    {
        /// <summary>
        /// DoverFormAttribute is used to annotate forms and indicate what is the formType and the Resource for the corresponding Form.
        /// </summary>
        /// <param name="type">Type string to be used during form creation</param>
        public DoverFormAttribute(string type)
        {
            this.FormType = type;
        }

        /// <summary>
        /// DoverFormAttribute is used to annotate forms and indicate what is the formType and the Resource for the corresponding Form.
        /// </summary>
        /// <param name="type">Type string to be used during form creation</param>
        /// <param name="resource">The name of the resource on the b1s file or a full qualified name of an embedded srf file.</param>
        public DoverFormAttribute(string type, string resource)
        {
            this.FormType = type;
            this.Resource = resource;
        }

        public string FormType { get; set; }

        public string Resource { get; set; }

    }
}
