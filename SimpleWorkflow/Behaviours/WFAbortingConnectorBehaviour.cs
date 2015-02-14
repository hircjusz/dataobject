using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Designer;
using System.Drawing.Drawing2D;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    [Serializable]
    [BehaviourInfo(WFConnectorBehaviourType.Abort, DashStyle.Dot)]
    public class WFAbortingConnectorBehaviour : WFConnectorBehaviourBase
    {
        public override void Run(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, bool force, IDictionary<string, object> args)
        {
            ValidateState();
            IWFProcessInstance pi = activityInstance.ProcessInstance;

            IEnumerable<IWFActivityInstance> activityInstances = null;
            if (!activityInstance.HasParent())
                activityInstances = pi.GetActivities(connector.destination.Code);
            else
                activityInstances = pi.GetActivitiesWithoutParent(connector.destination.Code)
                                        .Union(pi.GetActivitiesWithParent(connector.destination.Code, activityInstance.GetParentPath()))
                                        .Union(pi.GetActivitiesWithAncestor(connector.destination.Code, activityInstance.GetAncestorPath()));

            foreach (var activity in activityInstances.Where(x => x.State != WFActivityState.Completed))
                activity.Abort();
        }
    }
}
