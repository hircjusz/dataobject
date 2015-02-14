using System;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    //[NodeProperties("TemplateEnd", "end", "endsmall", typeof(Properties.Resources))]
    public class WFTemplateEndActivity : WFAutomaticActivity
    {
        public WFTemplateEndActivity()
        {
        }

        public WFTemplateEndActivity(WFEndActivity end)
        {
            this.Caption = end.Caption;
            this.Code = end.Code;
            this.Decription = end.Decription;
            this.DesignerSettings = end.DesignerSettings;
            this.EndScript = end.EndScript;
            this.ExecuteScript = end.ExecuteScript;
            this.LongRunning = end.LongRunning;
            this.StartScript = end.StartScript;
        }

        public override object Clone()
        {
            return new WFTemplateEndActivity()
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