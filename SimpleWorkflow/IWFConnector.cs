using System;
using System.Collections.Generic;
using System.Diagnostics;
using SoftwareMind.SimpleWorkflow.Behaviours;
using SoftwareMind.SimpleWorkflow.Designer;

namespace SoftwareMind.SimpleWorkflow
{

    public interface IWFConnector : IWFTemplateElement, IWFConnectorBase, ICloneable
    {
        IList<IWFCondition> Conditions { get; set; }
        string ConditionScript { get; set; }
        DesingerConnectorInfo DesignerSettings { get; set; }
        List<DesingerConditionItem> DesingerConditions { get; set; }
        List<DesingerPolicyItem> DesingerPolicies { get; set; }
        new WFActivityBase Destination { get; set; }
        IList<IWFPolicyRule> Policies { get; set; }
        string PolicyScript { get; set; }

        WFActivityBase Source { get; set; }

        IWFConnectorBehaviour ConnectorBehaviour { get; set; }
        void Disconnect();
        bool IsAvailable(IWFProcessInstance processInstance, IDictionary<string, object> args);
        void Run(IWFActivityInstance activityInstance, string codeSuffix = "", bool force = false, IDictionary<string, object> args = null);
        IWFActivityInstance FireSignal(IWFActivityInstance activityInstance, string codeSuffix = "", IDictionary<string, object> args = null);

        bool WaitingToBeExecuted(IWFProcessInstance iWFProcessInstance, HashSet<string> checkedConnectors);

        bool MainPath { get; set; }
        int DeleteParentPath { get; set; }
        IWFDestinationSplitter DestinationSplitter { get; set; }
    }
}
