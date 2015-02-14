using System;
using System.ComponentModel;
using System.Xml.Linq;
using SoftwareMind.Shared.Mef;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("Template", "subprocess", "subprocesssmall", typeof(Properties.Resources))]
    public class WFTemplateActivity : WFActivityBase
    {
        [DefaultValue(null)]
        public string TemplateName { get; set; }

        public virtual void SubstituteTemplate(WFProcess process)
        {
            //var substitutor = ServiceLocator.Current.GetInstance<IWFTemplateSubstitutor>();
            //substitutor.SubstituteTemplate(process, this);
            //tymczasowowo - to wyżej nie działa w schedulerze
            var substitutor = new WFTemplateSubstitutor();
            substitutor.SubstituteTemplate(process, this);
        }

        public override void ReadTemplateFromXmlElement(XElement element)
        {
            base.ReadTemplateFromXmlElement(element);

            XAttribute templateNameAttr = element.Attribute("TemplateName");
            if (templateNameAttr != null && !string.IsNullOrWhiteSpace(templateNameAttr.Value))
            {
                this.TemplateName = templateNameAttr.Value;
            }
        }

        public override XElement WriteTemplateToXmlElement()
        {
            XElement element = base.WriteTemplateToXmlElement();

            element.Add(new XAttribute("TemplateName", this.TemplateName));

            return element;
        }

        public override object Clone()
        {
            throw new NotSupportedException("Clone of template activity is not supported");
        }
    }
}
