using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    public enum WFConnectorBehaviourType
    {
        Standard,
        Abort,
        Subtask,
        Signal,
        StateMachine
    }
}
