using System;
using System.Collections.Generic;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    [NodeProperties("Fork", "fork", "forksmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFDecision : WFFork
    {
        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            if (this.ConnectorsOutgoing.Count != 2)
                throw new WFDesignException(string.Format("Krok {0} musi zawieraæ co dok³adnie dwa po³¹czenia wychodz¹ce", this.Code), Code);

            base.Validate(visited);
        }

        public override object Clone()
        {
            return new WFDecision()
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