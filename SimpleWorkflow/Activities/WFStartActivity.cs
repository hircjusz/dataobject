using System;
using System.Linq;
using SoftwareMind.SimpleWorkflow.Misc;
using SoftwareMind.SimpleWorkflow.Exceptions;
using System.Collections.Generic;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("Start", "start", "startsmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFStartActivity  : WFAutomaticActivity
    {
        protected internal override void DoTransitions(IWFActivityInstance activityInstance)
        {
            foreach (IWFConnector connector in this.ConnectorsOutgoing.Where(x => !(x.ConnectorBehaviourType == Behaviours.WFConnectorBehaviourType.Signal)))
               connector.Run(activityInstance);
        }

        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            if (ConnectorsOutgoing.Where(x => !(x.ConnectorBehaviourType == Behaviours.WFConnectorBehaviourType.Signal )).Count() != 1)
                throw new WFDesignException(string.Format("Krok {0} musi zawierać jedno połączenie wychodzące nie prowadzące do zdarzenia.", this.Code), Code);

            foreach (WFConnector connector in this.ConnectorsOutgoing)
            {
                if (visited.Contains(connector.Destination))
                    continue;
                visited.Add(connector.Destination);
                connector.Validate(visited);
            }
        }

        internal void Run(IWFActivityInstance instance, /*Action<IWFActivityInstance> beforeCompleted = null,*/ IDictionary<string, object> arguments = null)
        {
            base.Run(instance/*, beforeCompleted*/);

            if(arguments != null)
                foreach (var activity in instance.ProcessInstance.WorkList)
                    activity.AddVariables(arguments);
        }

        public override object Clone()
        {
            return new WFStartActivity()
            {
                Caption = this.Caption,
                Code = this.Code,
                Decription = this.Decription,
                DesignerSettings = this.DesignerSettings,
                EndScript = this.EndScript,
                ExecuteScript = this.ExecuteScript,
                LongRunning = this.LongRunning,
                StartScript = this.StartScript
            };
        }
    }
}
