using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public abstract class WFPolicyRuleBase: IWFPolicyRule
    {
        public virtual void ReadTemplateFromXmlElement(System.Xml.Linq.XElement element)
        {
        }

        public virtual System.Xml.Linq.XElement WriteTemplateToXmlElement()
        {
            return new System.Xml.Linq.XElement("PolicyRule",
                new XAttribute("Type", GetType().GetShortName()));
        }

        public virtual void Check(IWFProcessInstance processInstance, IDictionary<string, object> arguments)
        {
        }
    }
}
