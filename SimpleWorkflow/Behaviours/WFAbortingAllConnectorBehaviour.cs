using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Designer;
using System.Drawing.Drawing2D;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    [BehaviourInfo(WFConnectorBehaviourType.AbortAll, DashStyle.Dot)]
    public class WFAbortingAllConnectorBehaviour : WFConnectorBehaviourBase
    {
        public override void PerformActionOnSourceAfterTransition(IWFActivityInstance activity)
        {
            throw new NotImplementedException();
        }

        public override void Run(IWFActivityInstance activityInstance, string codeSuffix, IDictionary<string, object> args)
        {
            throw new NotImplementedException();
        }
    }
}
