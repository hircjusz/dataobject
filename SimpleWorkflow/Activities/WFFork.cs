using System.Collections.Generic;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;
using System;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    [NodeProperties("Fork (m)", "fork", "forksmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFFork : WFActivityBase
    {
        protected override bool Execute(IWFActivityInstance activityInstance)
        {
            return true;
        }

        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            if (this.ConnectorsIncoming.Count != 1)
                throw new WFDesignException(string.Format("Krok {0} musi zawieraæ jedno po³¹czenie wchodz¹ce", this.Code), Code);

            if (this.ConnectorsOutgoing.Count <= 1)
                throw new WFDesignException(string.Format("Krok {0} musi zawieraæ co najmniej dwa po³¹czenia wychodz¹ce", this.Code), Code);

            base.Validate(visited);
        }

        public override object Clone()
        {
            return new WFFork()
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