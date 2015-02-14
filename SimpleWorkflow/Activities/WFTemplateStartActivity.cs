using System;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    //[NodeProperties("TemplateStart", "start", "startsmall", typeof(Properties.Resources))]
    public class WFTemplateStartActivity : WFAutomaticActivity
    {
        public WFTemplateStartActivity()
        {
        }

        public WFTemplateStartActivity(WFStartActivity start)
        {
            this.Caption = start.Caption;
            this.Code = start.Code;
            this.Decription = start.Decription;
            this.DesignerSettings = start.DesignerSettings;
            this.EndScript = start.EndScript;
            this.ExecuteScript = start.ExecuteScript;
            this.LongRunning = start.LongRunning;
            this.StartScript = start.StartScript;
        }

        public override object Clone()
        {
            return new WFTemplateStartActivity()
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