using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using SoftwareMind.SimpleWorkflow.Activities;
using SoftwareMind.SimpleWorkflow.Behaviours;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.SimpleWorkflow.Misc;
using SoftwareMind.Utils.Exceptions;

namespace SoftwareMind.SimpleWorkflow
{

    [Serializable]
    public class WFProcess : WFVariableDefContainer, ICloneable
    {
        [Browsable(true)]
        public string Name { get; set; }

        protected IDictionary<string, WFActivityBase> Activities { get; set; }

        [Browsable(false)]
        public WFStartActivity RootActivity { get; set; }
        [Browsable(false)]
        public DesignerGlobalInfo DesingerGlobalSettings { get; set; }


        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        [Category("Common")]
        [Description("Skrypt wykonywany przed zmianą wartości zmiennej.")]
        public string OnVariableChanged { get; set; }

        public WFProcess()
        {
            this.DesingerGlobalSettings = new DesignerGlobalInfo();
            this.Activities = new Dictionary<string, WFActivityBase>();
        }

        public WFActivityBase GetActivity(string activityCode)
        {
            return this.Activities[GetRealCode(activityCode)];
        }

        public bool ContainsActivity(string activityCode)
        {
            return Activities.ContainsKey(GetRealCode(activityCode));
        }

        internal static string GetRealCode(string activityCode)
        {
            string realCode = activityCode;
            if (realCode.Contains('|')) realCode = realCode.Substring(0, realCode.IndexOf('|'));
            // jeśli jest singleInstance na false to mozna tworzyć więcej niż jedno zadanie o podanym kodzie,
            // w takim wypadku dodajmy # na koniec
            if (realCode.Contains("#")) realCode = realCode.Substring(0, realCode.IndexOf("#"));

            return realCode;
        }

        public void AddActivity(WFActivityBase activity, bool substituteTemplate = true)
        {
            //w przypadku uruchamiania workflow editora nie podmieniamy templatów
            if (substituteTemplate && activity is WFTemplateActivity)
            {
                ((WFTemplateActivity)activity).SubstituteTemplate(this);
                return;
            }

            activity.Process = this;
            this.Activities.Add(activity.Code, activity);
        }

        public void RemoveActivity(WFActivityBase activity)
        {
            activity.Process = null;
            this.Activities.Remove(activity.Code);
        }

        public virtual void ValidateStructure()
        {
            HashSet<WFActivityBase> visitedActivites = new HashSet<WFActivityBase>();
            this.RootActivity.StartValidate(visitedActivites);
        }

        public IEnumerable<WFActivityBase> GetAllActivities()
        {
            return this.Activities.Values;
        }

        public WFProcessInstance Run(WFProcessInstance processInstance, IDictionary<string, object> processArguments/*, Action<IWFProcessInstance> onEachTransitionCompleted = null, Action<IWFActivityInstance> beforeCompleted = null*/)
        {
            if (processInstance == null) throw new ArgumentNullException("processInstance", "Instancja procesu musi być podana i być różna od null");

            processInstance.ProcessName = this.Name;
            processInstance.AddVariables(processArguments);

            IWFActivityInstance activityInstance = new WFActivityInstance(processInstance, this.RootActivity.Code);

            this.RootActivity.Run(activityInstance, /*beforeCompleted, */processArguments);
//            if (onEachTransitionCompleted != null)
 //               onEachTransitionCompleted(processInstance);

            return processInstance;
        }


        public object Clone()
        {
            var process = new WFProcess
            {
                DesingerGlobalSettings = this.DesingerGlobalSettings,
                Name = this.Name,
                OnVariableChanged = this.OnVariableChanged
            };

            foreach (var variable in this.Variables)
            {
                var value = (WFVariableDef)variable.Value.Clone();
                value.Collection = this.Variables;

                process.Variables.Add(variable.Key, value);
            }

            IDictionary<WFActivityBase, WFActivityBase> cloneMap = new Dictionary<WFActivityBase, WFActivityBase>();
            foreach (var activity in this.Activities)
            {
                var value = (WFActivityBase)activity.Value.Clone();
                //klonowanie zmiennych
                if ((value.VariableDefs == null || value.VariableDefs.Count == 0) && (activity.Value.VariableDefs != null && activity.Value.VariableDefs.Count > 0))
                {
                    if(value.VariableDefs == null)
                        value.VariableDefs = new WFVariableDefCollection();
                    foreach(var def in activity.Value.VariableDefs)
                        value.VariableDefs.Add(def);
                }

                process.Activities.Add(activity.Key, value);
                cloneMap.Add(activity.Value, value);
            }

            foreach (var activity in this.Activities)
            {
                foreach (IWFConnector connector in activity.Value.ConnectorsOutgoing.ToArray())
                {
                    var newConnector = (IWFConnector)connector.Clone();
                    newConnector.Destination = cloneMap[connector.Destination];
                    newConnector.Source = cloneMap[connector.Source];
                }
            }

            process.RootActivity = (WFStartActivity) cloneMap[this.RootActivity];

            return process;
        }
    }
}
