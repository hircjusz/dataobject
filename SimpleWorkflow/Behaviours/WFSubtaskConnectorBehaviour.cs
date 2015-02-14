using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Designer;
using System.Drawing.Drawing2D;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    [Serializable]
    [BehaviourInfo(WFConnectorBehaviourType.Subtask, DashStyle.DashDot)]
    public class WFSubtaskConnectorBehaviour : WFConnectorBehaviourBase
    {
        public override void Run(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, bool force, IDictionary<string, object> args)
        {
        }
    }
}
