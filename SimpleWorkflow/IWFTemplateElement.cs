using System.Xml.Linq;

namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFTemplateElement
    {
        void ReadTemplateFromXmlElement(XElement element);
        XElement WriteTemplateToXmlElement();
    }
}
