using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Designer;

namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFConnectorBehaviour
    {
        void SetConnector(IWFConnector connector);

        bool IsAvailable(IWFProcessInstance processInstance, IDictionary<string, object> args);

        void Validate(WFActivityBase source, WFActivityBase destination);

        void Run(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, bool force, IDictionary<string, object> args);

        IWFActivityInstance FireSignal(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, IDictionary<string, object> args);

        bool WaitingToBeExecuted(WFConnector wFConnector, IWFProcessInstance iWFProcessInstance, HashSet<string> checkedConnectors);
    }
}
