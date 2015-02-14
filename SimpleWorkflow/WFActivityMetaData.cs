using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class WFActivityMetaData
    {
        [DefaultValue("Key")]
        public string Key { get; set; }
        [DefaultValue("")]
        public string Value { get; set; }

        public WFActivityMetaData()
        {
            Key = "Key";
        }

        public XElement SaveToXmlElement()
        {
            return new XElement("MetaData", new XAttribute("Key", this.Key ?? ""), new XAttribute("Value", this.Value ?? ""));
        }

        public void LoadFromXmlElement(XElement element)
        {
            this.Key = element.Attribute("Key").Value;
            this.Value = element.Attribute("Value").Value;
        }
    }
}
