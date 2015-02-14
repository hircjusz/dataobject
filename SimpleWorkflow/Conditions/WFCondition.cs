using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Designer;
using System;

namespace SoftwareMind.SimpleWorkflow.Conditions
{
    [Serializable]
    public class WFCondition : IWFCondition
    {
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        public string Expression { get; set; }

        public bool Eval(IDictionary<string, object> arguments)
        {
            Scripts.Script script = new Scripts.Script(this.Expression);
            script.Execute();
            return (bool) script["result"];
        }

        public System.Xml.Linq.XElement WriteTemplateToXmlElement()
        {
            return new XElement("Condition", new XAttribute("Type", this.GetType()), new XAttribute("Expression", this.Expression ?? ""));
        }

        public void ReadTemplateFromXmlElement(XElement element)
        {
            this.Expression = element.Attribute("Expression").Value;
        }
    }
}
