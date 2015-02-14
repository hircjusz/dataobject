using System;
using System.Collections.Generic;
using SoftwareMind.SimpleWorkflow.Exceptions;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    public abstract class WFAutomaticActivity : WFActivityBase
    {
        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            if (this.ConnectorsIncoming.Count != 1)
                throw new WFDesignException(string.Format("Krok {0} musi zawieraæ jedno po³¹czenie wchodz¹ce", this.Code), Code);

            if (this.ConnectorsOutgoing.Count != 1)
                throw new WFDesignException(string.Format("Krok {0} musi zawieraæ jedno po³¹czenie wychodz¹ce", this.Code), Code);

            base.Validate(visited);
        }
    }
}