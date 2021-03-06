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
                throw new WFDesignException(string.Format("Krok {0} musi zawierać jedno połączenie wchodzące", this.Code), Code);

            if (this.ConnectorsOutgoing.Count != 1)
                throw new WFDesignException(string.Format("Krok {0} musi zawierać jedno połączenie wychodzące", this.Code), Code);

            base.Validate(visited);
        }
    }
}