using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SoftwareMind.SimpleWorkflow.Activities 
{
    public interface IWFWorkSplitter : IWFTemplateElement
    {
        IDictionary<string, object>[] GetInputData(IWFActivityInstance activityInstance);
    }
}
