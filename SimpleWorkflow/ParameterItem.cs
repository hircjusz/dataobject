using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class ParameterItem
    {
        [DefaultValue("Name")]
        public string Name { get; set; }
        [DefaultValue("Value")]
        public string Value { get; set; }

        public ParameterItem()
        {
            Name = "Name";
            Value = "Value";
        }

        public XElement SaveToXmlElement()
        {
            return new XElement("Parameter", new XAttribute("Name", this.Name ?? ""), new XAttribute("Value", this.Value ?? ""));
        }

        public void LoadFromXmlElement(XElement element)
        {
            this.Name = element.Attribute("Name").Value;
            this.Value = element.Attribute("Value").Value;
        }
    }
}
