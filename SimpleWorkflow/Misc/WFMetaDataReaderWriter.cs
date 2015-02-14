using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    [Serializable]
    internal class WFMetaDataReaderWriter
    {
        public EditableList<WFActivityMetaData> ReadTemplateFromXmlElement(System.Xml.Linq.XElement element)
        {
            EditableList<WFActivityMetaData> list = new EditableList<WFActivityMetaData>();

            XElement metaDataCollection = null;

            if (element.Name == "MetaDataCollection")
                metaDataCollection = element;
            else
                metaDataCollection = element.Elements("MetaDataCollection").FirstOrDefault();

            if (metaDataCollection == null)
                return list;

            foreach (var mdElement in metaDataCollection.Elements("MetaData"))
            {
                WFActivityMetaData metaData = new WFActivityMetaData();
                metaData.LoadFromXmlElement(mdElement);
                list.Add(metaData);
            }

            return list;
        }
        
        public System.Xml.Linq.XElement WriteTemplateToXmlElement(EditableList<WFActivityMetaData> metaData)
        {
            return new XElement("MetaDataCollection", from md in metaData select md.SaveToXmlElement());
        }
    }
}
