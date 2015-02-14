using System.Collections.Generic;
using System.Linq;
using SoftwareMind.SimpleWorkflow.Activities;
using SoftwareMind.SimpleWorkflow.Exceptions;
using System;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class WFStateMachine 
    {
        // zrobione internal zeby umożliwić testowanie :)
        internal WFProcess Process {get; private set;}

        private WFStateMachine(WFProcess process)
        {
            this.Process = process;
        }

        public static WFStateMachine LoadMachine(string templateXml)
        {
            WFStateMachine machine = new WFStateMachine(WFProcessFactory.GetProcess(templateXml));
            machine.ValidateStructure();
            return machine;
        }

        public static WFStateMachine LoadMachineFromXml(string templateXml, string shortAlias = null)
        {
            WFStateMachine machine = new WFStateMachine(WFProcessFactory.GetProcessFromXml(templateXml));
            machine.ShortAlias = shortAlias;
            machine.ValidateStructure();
            return machine;
        }

        private IWFActivityInstance MakeActivityInstance(string activity, Dictionary<string, object> arguments = null)
        {
            WFProcessInstance instance = new WFProcessInstance();
            instance.AddVariables(arguments);
            IWFActivityInstance activityInstance = new WFActivityInstance(instance, activity, WFActivityState.Executed);
            activityInstance.ProcessInstance.ProcessName = Process.Name;
            return activityInstance;
        }

        public WFActivityBase DoTransition(string activity, string connector, Dictionary<string, object> arguments = null)
        {
            IWFActivityInstance activityInstance = MakeActivityInstance(activity, arguments);
            WFProcessCoordinator.Instance.DoTransition(new SoftwareMind.SimpleWorkflow.WFProcessInstance.TransparentBox(activityInstance.ProcessInstance), activity, connector, arguments);
            var transitions = activityInstance.ProcessInstance.GetTransitions(activity, arguments).Where(x => x.Code == connector && x.IsConnectorAvailable == true);
            return transitions.Select(x => x.Destination).SingleOrDefault();
        }

        public IEnumerable<IWFConnector> GetAvailableTransitions(string activity, Dictionary<string, object> arguments = null)
        {
            IWFActivityInstance activtyInstance = MakeActivityInstance(activity, arguments);
            return activtyInstance.GetAvailableTransitions();
        }

        public WFActivityBase GetStateFromCode(string code)
        {
            return Process.GetActivity(code);
        }

        public void ValidateStructure()
        {
            if (Process.RootActivity.GetDirectlyConnectedActivities().Count() != 1)
                throw new WFDesignException(string.Format("Stan startowy może mieć jedno przejście"), null);

            foreach (WFActivityBase a in Process.GetAllActivities())
            {
                if (a is WFJoin)
                    throw new WFDesignException(string.Format("Maszyna stanów nie może zawierać Join'ów"), null);
                if (a is WFFork)
                    throw new WFDesignException(string.Format("Maszyna stanów nie może zawierać Fork'ów"), null);
            }
        }

        public string GetFirstStateCode()
        {
            return Process.GetAllActivities().OfType<WFStartActivity>().Single().ConnectorsOutgoing[0].Destination.Code;
        }

        public string ToStringGraph(string prefix, IDictionary<string, string> valueMap, bool igonerEmpty = true)
        {
            return string.Join(";",Process.GetAllActivities().Where(x => x is WFStateMachineActivity || x is WFStateMachineEndActivity).
                Select(x => "(" + x.Code + "," + prefix + x.Code + "," + string.Join(",", x.ConnectorsOutgoing.Where(z => !z.IsSystem && (!igonerEmpty || !string.IsNullOrWhiteSpace(z.Caption))).Select(y => y.Code + "," + (valueMap != null ? (valueMap.ContainsKey(prefix + y.Code) ? valueMap[prefix + y.Code] : "") : prefix + y.Code))) + ")"));
        }

        public string GetName()
        {
            return this.Process.Name;
        }

        public string ShortAlias { get; set; }
    }
}
