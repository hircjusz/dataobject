using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    public interface IWFDestinationSplitter : IWFTemplateElement
    {
        IDictionary<string, object>[] GetDestinations(IWFActivityInstance activityInstance, string code);
    }
}
