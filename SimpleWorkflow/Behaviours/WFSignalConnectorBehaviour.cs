using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Designer;
using System.Drawing.Drawing2D;
using log4net;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    [BehaviourInfo(WFConnectorBehaviourType.Signal, DashStyle.Dash)]
    [Serializable]
    public class WFSignalConnectorBehaviour : WFConnectorBehaviourBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFSignalConnectorBehaviour));

        public override void Run(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, bool force, IDictionary<string, object> args)
        {

        }

        public override bool WaitingToBeExecuted(WFConnector wFConnector, IWFProcessInstance iWFProcessInstance, HashSet<string> checkedConnectors)
        {
            return false;
        }


        public override IWFActivityInstance FireSignal(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, IDictionary<string, object> args)
        {
            IWFActivityInstance newActivityInstance = null;

            ValidateState();

            var processInstance = activityInstance.ProcessInstance;
            if (IsAvailable(processInstance, args))
            {
                IDictionary<string, object> arguments = activityInstance.ProcessInstance.GetVariables();
                foreach (var arg in activityInstance.GetVariables())
                    if (!arguments.ContainsKey(arg.Key))
                        arguments.Add(arg.Key, arg.Value);
                foreach (var policy in Connector.DesingerPolicies)
                    policy.Policy.Check(processInstance, arguments);

                newActivityInstance = CreateNewActivity(activityInstance, connector, codeSuffix, args, processInstance);
            }
            else
                throw new InvalidOperationException("Operacja nie jest w tej chwili dostępna. Proszę odświeżyć stronę zadania.");

            return newActivityInstance;
        }

        private IWFActivityInstance CreateNewActivity(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, IDictionary<string, object> args, IWFProcessInstance processInstance)
        {
            log.Info("CreateNewActivity: ");

            IWFActivityInstance newActivityInstance = null;

            string destinationSuffix = codeSuffix + activityInstance.GetParentPath();
            string destinationActivityCode = Connector.Destination.Code + destinationSuffix;

            // 1. Sprawdzić czy zadanie sie nie wykonuje już ?
            var singleInstance = connector.SingleInstance;
            var alreadyExists = processInstance.ContainsActivity(destinationActivityCode);
            var forceSingleInstance = connector.ForceSingleInstance;

            log.DebugFormat("singleInstance: {0}, alreadyExists: {1}", singleInstance, alreadyExists);

            if (alreadyExists)
            {
                IWFActivityInstance activity = processInstance.GetActivities(Connector.Destination.Code).Where(x => x.State != WFActivityState.Completed).FirstOrDefault();
                if(activity == null)
                    activity = processInstance.GetActivities(Connector.Destination.Code).FirstOrDefault();
                if ((singleInstance && (activity.State != WFActivityState.Completed && activity.State != WFActivityState.Initialized)) || forceSingleInstance)
                {
                    // nic nie robimy, moze być tylko jedna instancja a ona już istnieje
                    newActivityInstance = null;
                    if (forceSingleInstance)
                    {
                        activity.State = WFActivityState.Initialized;
                        newActivityInstance = activity;
                    }
                }
                else
                {
                    //! Poprawka na ogranieczenie ilości zadań do 2 (te same kody zadań)
                    int count = 1 + activityInstance.ProcessInstance.GetActivities(destinationActivityCode).Count();
                    // istnieje już taka aktywność ale możemy stworzyć nową
                    destinationActivityCode += "#" + count;
                    newActivityInstance = activityInstance.GetOrCreateActivity(destinationActivityCode, Connector.Destination.GetType());
                    if (args != null)
                        newActivityInstance.AddVariables(args);
                }
            }
            else
            {
                // Tworzymy nową aktywność
                newActivityInstance = activityInstance.GetOrCreateActivity(destinationActivityCode, Connector.Destination.GetType());
                if (args != null)
                    newActivityInstance.AddVariables(args);
            }

            if (newActivityInstance != null)
            {
                newActivityInstance.Activity.ChangeState(newActivityInstance);
                activityInstance.ProcessInstance.PushToWorkList(newActivityInstance);
            }

            return newActivityInstance;
        }
    }
}
