using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class MappingItem
    {
        [DefaultValue("From")]
        public string From { get; set; }
        [DefaultValue("To")]
        public string To { get; set; }

        public MappingItem()
        {
            From = "From";
            To = "To";
        }

        public XElement SaveToXmlElement()
        {
            return new XElement("Map", new XAttribute("From", this.From ?? ""), new XAttribute("To", this.To ?? ""));
        }

        public void LoadFromXmlElement(XElement element)
        {
            this.From = element.Attribute("From").Value;
            this.To = element.Attribute("To").Value;
        }
    }
}
