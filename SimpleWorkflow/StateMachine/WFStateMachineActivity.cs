using System;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("State machine activity", "state", "statesmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFStateMachineActivity : WFManualActivity
    {
        protected override bool Execute(IWFActivityInstance instance)
        {
            return false;
        }

        public override object Clone()
        {
            return new WFStateMachineActivity()
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
