using System.Collections.Generic;

namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFCondition : IWFTemplateElement
    {
        string Expression { get; set; }
        bool Eval(IDictionary<string, object> arguments);
    }
}
