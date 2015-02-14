using System.Collections.Generic;

namespace SoftwareMind.SimpleWorkflow.Actions
{
    public interface IWfActivityAction
    {
        void Perform(IWfActivityActionParameters parameter, Dictionary<string, object> context);
        IWfActivityActionParameters CreateDefaultParameter();
    }
}
