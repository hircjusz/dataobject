using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.Scripts;
using System.Drawing.Drawing2D;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    [Serializable]
    [BehaviourInfo(WFConnectorBehaviourType.Standard, DashStyle.Solid)]
    public class WFStandardConnectorBehaviour : WFConnectorBehaviourBase
    {
        public override void Run(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, bool force, IDictionary<string, object> args)
        {
            ValidateState();
            var processInstance = activityInstance.ProcessInstance;
            IDictionary<string, object> allArgs = args;
            if (allArgs != null)
            { 
                foreach (var arg in activityInstance.GetVariables())
                    if (!allArgs.ContainsKey(arg.Key))
                        allArgs.Add(arg.Key, arg.Value);
            }
            else
                allArgs = activityInstance.GetVariables();
            if (force || IsAvailable(processInstance, allArgs))
            {
                IDictionary<string, object> arguments = activityInstance.ProcessInstance.GetVariables();
                if(args != null)
                    foreach (var arg in args)
                        if (!arguments.ContainsKey(arg.Key))
                            arguments.Add(arg.Key, arg.Value);
                //tu między innymi są dodawane argumenty z rodzica
                foreach (var arg in activityInstance.GetVariables())
                    if (!arguments.ContainsKey(arg.Key))
                        arguments.Add(arg.Key, arg.Value);
                foreach (var policy in Connector.DesingerPolicies)
                    policy.Policy.Check(processInstance, arguments);


                // policy script wywoływany tylko jak condition jest spełniony
                if (!String.IsNullOrEmpty(Connector.PolicyScript))
                {
                    IScript script = new Script(Connector.PolicyScript, arguments);
                    script.Execute();
                }

                string destinationSuffix = "";
                if (Connector.DestinationSplitter != null)
                {
                    IDictionary<string, object>[] destinations = Connector.DestinationSplitter.GetDestinations(activityInstance, Connector.Destination.Code);
                    foreach (var destination in destinations)
                    {
                        if(destination.ContainsKey("ParentPath") )
                            destinationSuffix = destination["ParentPath"].ToString();
                        else
                            destinationSuffix = "";
                        StartNewActivity(activityInstance, connector, args, destinationSuffix);
                    }
                }
                else
                {
                    destinationSuffix = GetDestinationSuffix(codeSuffix, activityInstance, Connector.DeleteParentPath);
                    StartNewActivity(activityInstance, connector, args, destinationSuffix);
                }
                
                
            }
        }

        private void StartNewActivity(IWFActivityInstance activityInstance, WFConnector connector, IDictionary<string, object> args, string destinationSuffix)
        {
            IWFActivityInstance newActivityInstance = activityInstance.GetOrCreateActivity(Connector.Destination.Code + destinationSuffix, Connector.Destination.GetType());
            if (newActivityInstance != activityInstance) // loop fix
            {
                if (args != null)
                    newActivityInstance.AddVariables(args);
                newActivityInstance.Activity.ChangeState(newActivityInstance);
                activityInstance.ProcessInstance.PushToWorkList(newActivityInstance);
            }
        }

        /// <summary>
        /// Zwraca sufiks nazwy kroku, ucina tyle ścieżek rodziców ile jest w parametrze deleteParentPath
        /// </summary>
        /// <param name="codeSuffix"></param>
        /// <param name="activityInstance"></param>
        /// <param name="deleteParentPath"></param>
        /// <returns></returns>
        private string GetDestinationSuffix(string codeSuffix, IWFActivityInstance activityInstance, int deleteParentPath)
        {
            if (deleteParentPath < 1)
            {
                return codeSuffix + activityInstance.GetParentPath();
            }
            string pp = activityInstance.GetParentPath();
            for(int i = 0; i<deleteParentPath; i++)
            {
                pp = GetParentPath(pp);
            }
            return codeSuffix + pp;
        }

        private string GetParentPath(string path)
        {
            if(string.IsNullOrEmpty(path))
                return "";
            int idx = path.IndexOf('|', 1);
            if (idx > 0)
                return path.Substring(idx);
            return "";
        }

        public override bool ActivityCheck(IWFProcessInstance processInstance)
        {
            return true;
        }
    }
}
