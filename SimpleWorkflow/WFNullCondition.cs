using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleWorkFlowEngine;

namespace SoftwareMind.SimpleWorkflow
{
    public class WFNullCondition : IWFCondition
    {
        public bool Eval(object o)
        {
            return true;    
        }
    }
}
