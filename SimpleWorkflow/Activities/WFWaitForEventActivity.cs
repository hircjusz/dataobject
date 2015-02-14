using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("Wait for event", "waitforevent", "waitforeventsmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFWaitForEventActivity : WFActivityBase
    {
        public WFWaitForEventActivity()
        {
        }

        protected override bool Execute(IWFActivityInstance instance)
        {
            return false;
        }

        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            base.Validate(visited);
        }

        public override object Clone()
        {
            return new WFWaitForEventActivity()
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
