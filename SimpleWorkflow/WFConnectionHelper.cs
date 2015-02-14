using System;
using SoftwareMind.SimpleWorkflow.Behaviours;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class WFConnectionHelper
    {
        public static IWFConnector JoinActivities(WFActivityBase source, WFActivityBase destination, IWFConnectorBehaviour behaviour, string code = null)
        {
            IWFConnector connector = null;

            connector = new WFConnector(source, destination, behaviour);
            connector.Code = code;

            return connector;
        }

        public static IWFConnector JoinActivities(WFActivityBase source, WFActivityBase destination, WFConnectorBehaviourType behaviourType = WFConnectorBehaviourType.Standard, string code = null)
        {
            IWFConnectorBehaviour behaviour = null;

            behaviour = WFConnectorBehaviourBase.Create(behaviourType);

            return JoinActivities(source, destination, behaviour, code);
        }
    }
}
