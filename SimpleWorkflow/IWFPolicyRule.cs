using System.Collections.Generic;
namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFPolicyRule : IWFTemplateElement
    {
        void Check(IWFProcessInstance processInstance, IDictionary<string, object> arguments);
    }
}