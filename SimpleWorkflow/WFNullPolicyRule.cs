using System.Collections.Generic;
using System;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    class WFNullPolicyRule : WFPolicyRuleBase
    {
        public override void Check(IWFProcessInstance processInstance, IDictionary<string, object> arguments)
        {
        }
    }
}
