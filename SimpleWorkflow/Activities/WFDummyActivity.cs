using System;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    public class WFDummyActivity : WFAutomaticActivity
    {
        protected override bool Execute(IWFActivityInstance instance)
        {
            if (!String.IsNullOrEmpty(ExecuteScript))
                ScriptHelper.Execute(ExecuteScript, instance);
            return true;
        }

        public override object Clone()
        {
            return new WFDummyActivity()
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