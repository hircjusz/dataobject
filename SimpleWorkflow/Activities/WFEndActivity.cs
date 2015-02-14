using System;
using System.Collections.Generic;
using log4net;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("End", "end", "endsmall", typeof(Properties.Resources))]
    public class WFEndActivity : WFActivityBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFEndActivity));

        public override void OnActivated(IWFActivityInstance instance)
        {
            log.Debug("Krok końcowy został osiągnięty");
        }

        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            if (this.ConnectorsIncoming.Count == 0)
                throw new WFDesignException(string.Format("Krok {0} musi zawierać co najmniej jedno połączenie wchodzące", this.Code), Code);

            if (this.ConnectorsOutgoing.Count != 0)
                throw new WFDesignException(string.Format("Krok końcowy {0} nie może zawierać połączeń wchodzących", this.Code), Code);
        }

        public override object Clone()
        {
            return new WFEndActivity()
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
