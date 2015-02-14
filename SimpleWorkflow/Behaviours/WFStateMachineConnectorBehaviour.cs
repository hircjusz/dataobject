using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.Scripts;
using System.Drawing.Drawing2D;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    [Serializable]
    [BehaviourInfo(WFConnectorBehaviourType.StateMachine, DashStyle.Solid)]
    public class WFStateMachineConnectorBehaviour : WFStandardConnectorBehaviour
    {
        public override void Validate(WFActivityBase source, WFActivityBase destination)
        {
            ValidateState();
        }
    }
}
