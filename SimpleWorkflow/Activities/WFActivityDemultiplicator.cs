using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow;
using SoftwareMind.SimpleWorkflow.Misc;
using SoftwareMind.SimpleWorkflow.Exceptions;


namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("Demultiplied activity", "multi", "multismall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFActivityDemultiplicator : WFActivityBase
    {
        public override string GetParentPath(WFActivityInstance activityInstance)
        {
            string path = base.GetParentPath(activityInstance);
            int idx = path.IndexOf('|', 1);
            if (idx > 0)
                return path.Substring(idx);
            else
                return "";
        }

        protected override void Validate(HashSet<WFActivityBase> validated)
        {
            base.Validate(validated);
        }

        public override object Clone()
        {
            return new WFActivityDemultiplicator()
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
