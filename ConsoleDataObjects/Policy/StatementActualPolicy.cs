using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftwareMind.SimpleWorkflow;

namespace ConsoleDataObjects.Policy
{
    public class StatementActualPolicy : WFPolicyRuleBase
    {
        public override void Check(IWFProcessInstance processInstance, IDictionary<string, object> arguments)
        {
            //throw new Exception("Nie mozna zmienic stanu na aktualny");

        }

    }
}
