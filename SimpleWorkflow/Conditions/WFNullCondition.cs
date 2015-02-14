using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Designer;
using System;

namespace SoftwareMind.SimpleWorkflow.Conditions
{
    [Serializable]
    public class WFNullCondition : IWFCondition
    {
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        public string Expression { get; set; }

        public virtual bool Eval(IDictionary<string, object> arguments)
        {
            return true;
        }

        public System.Xml.Linq.XElement WriteTemplateToXmlElement()
        {
            return new XElement("Condition", new XAttribute("Type", this.GetType()), new XAttribute("Expression", this.Expression ?? ""));
        }

        public void ReadTemplateFromXmlElement(XElement element)
        {
            //to jest null object, nic nie trzeba robic
        }
    }
}
