using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    /// <summary>
    /// Tej klasy możemy użyć w momencie gdy chcemy oznaczyć gałąź jako kończącą aktywność ale nie cały proces :)
    /// </summary>
    [Serializable]
    [NodeProperties("NULL", "end", "endsmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class NullActivity : WFEndActivity
    {
        protected override void Validate(HashSet<WFActivityBase> validated)
        {
        }

        public override object Clone()
        {
            return new NullActivity()
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
