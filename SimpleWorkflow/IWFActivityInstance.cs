using System;
using System.Collections.Generic;

namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFActivityInstance : IWFVariableContainer, IWFStateElement
    {
        WFActivityBase Activity { get; }
        string Code { get; set; }
        IWFActivityInstance GetOrCreateActivity(string activityCode, Type activityType);
        IWFProcessInstance ProcessInstance { get; set; }
        WFActivityState State { get; set; }
        IEnumerable<IWFConnector> GetAvailableTransitions(IDictionary<string, object> args = null);
        IEnumerable<IWFConnectorBase> GetTransitions(IDictionary<string, object> args = null);
        IWFConnectorBase GetDefaultTransition(IDictionary<string, object> args);
        bool IsCompleted();
        bool HasParent();
        string GetParentPath();
        WFTransitionStatus DoTransition(IWFActivityInstance activityInstance, string connectorCode/*, Action<IWFActivityInstance> beforeCompleted = null*/, Dictionary<string, object> arguments = null);
        void Abort(IDictionary<string, object> args = null);
        void ManualChangeState(string stepCode, WFActivityState state);
        
        /// <summary>
        /// Próbuje pobrać wartośc zmiennej najpierw z kroku, a potem z procesu. Jesli znajdzie gdzieś zmienną wywołuje operacje przekazaną jako drugi parametr.
        /// </summary>
        /// <param name="name">Nazwa zmiennej</param>
        /// <param name="operation"></param>
        void TryGetValue(string name, TryGetValueOperation operation);

        string GetAncestorPath();

        string GetPrefixFromParams(params string[] codes);
        string GetVariableNameFromSubprocess(string varName);
    }

    public delegate void TryGetValueOperation(object value);
}
