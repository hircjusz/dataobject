
using System.Xml.Linq;
namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFStateElement
    {
        void ReadStateFromXmlElement(XElement element);
        XElement WriteStateToXmlElement();
    }
}
