using System;
namespace SoftwareMind.SimpleWorkflow
{
    interface IWFProcessCoordinator
    {
        IWFTransitionResult DoTransition(IWFProcessInstanceBox processInstanceBox, string activityCode, string connectorCode, System.Collections.Generic.Dictionary<string, object> arguments);
        IWFProcessInstance FireSignal(IWFProcessInstanceBox processInstanceBox, string eventConnectorCode, System.Collections.Generic.IDictionary<string, object> arguments);
        IWFProcessInstance FireEventByActivityCode(IWFProcessInstanceBox processInstanceBox, string eventCode, System.Collections.Generic.IDictionary<string, object> arguments);
        IWFProcessInstance RunProcess(IWFProcessInstanceBox processInstanceBox, string processName, System.Collections.Generic.IDictionary<string, object> processArguments);
        IWFProcessInstance RunProcess(IWFProcessInstanceBox processInstanceBox, string processName, System.Collections.Generic.IDictionary<string, object> processArguments, WFVariableDefCollection defColl);
        IWFProcessInstance SetVariable(IWFProcessInstanceBox processInstanceBox, string variable, object value);
        IWFProcessInstance Recovery(IWFProcessInstanceBox processInstanceBox);
    }
}
