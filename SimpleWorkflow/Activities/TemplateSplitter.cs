using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    public class TemplateSplitter : IWFWorkSplitter
    {
        private string _templateName;

        public TemplateSplitter(string templateName)
        {
            this._templateName = templateName;
        }

        public void ReadTemplateFromXmlElement(XElement element)
        {
            throw new NotImplementedException();
        }

        public XElement WriteTemplateToXmlElement()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object>[] GetInputData(IWFActivityInstance activityInstance)
        {
            IDictionary<string, object> input = new Dictionary<string, object>() { { "List", this._templateName } };

            return new[] { input };
        }
    }
}