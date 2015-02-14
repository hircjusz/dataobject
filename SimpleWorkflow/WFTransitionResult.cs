using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class WFTransitionResult : IWFTransitionResult
    {
        public IWFProcessInstance ProcessInstace { get; internal set; }
        public WFTransitionStatus Result { get; internal set; }
    }
}
