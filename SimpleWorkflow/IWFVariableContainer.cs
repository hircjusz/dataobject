using System.Collections.Generic;
namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFVariableContainer
    {
        void AddVariable(WFVariable variable);
        void AddVariableValue(string name, object value);
        bool ContainsVariable(string name);
        WFVariableDefContainer GetDefContainer();
        WFVariable GetVariable(string name);
        object GetVariableValue(string name);
        object GetVariableValueOrDefault(string name);
        void AddVariables(IDictionary<string, object> activityArguments);
        IDictionary<string, object> GetVariables();
        void SetVariableValue(string name, object value);
        void RemoveVariable(string name);
    }
}
