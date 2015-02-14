using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFTransitionResult
    {
        IWFProcessInstance ProcessInstace { get;  }
        WFTransitionStatus Result { get; }
    }
}

  
