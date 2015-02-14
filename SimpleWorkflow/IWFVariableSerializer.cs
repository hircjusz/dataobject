using System.Collections.Generic;

namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFVariableSerializer
    {
        string SerializeValue(object obj);
        object Deserialize(object obj, IDictionary<string, WFVariable> otherVariables);
    }
}
