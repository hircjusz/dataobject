using System;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("State machine end activity", "stateend", "stateendsmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFStateMachineEndActivity : WFStateMachineActivity
    {
        public WFStateMachineEndActivity()
        {
            ValidateOutgoing = false;
        }

        public override object Clone()
        {
            return new WFStateMachineEndActivity()
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
