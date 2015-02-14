using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Diagnostics;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    [DebuggerDisplay("Code = {Code} State={State}")]
    public class WFActivityInstance : WFVariableContainer, IWFActivityInstance
    {
        public string Code { get; set; }
        private WFActivityState state { get; set; }
        public WFActivityState State 
        {
            get 
            { 
                return this.state; 
            }
            set 
            {
                //if(!String.IsNullOrEmpty(this.Code))
                //    Logger.Log.Debug("Zmiana statnu aktywności o kodzie '{0}' z '{1}' na '{2}' ", this.Code, this.State, value);

                this.state = value; 
            }
        }
        public IWFProcessInstance ProcessInstance { get; set; }
        public virtual WFActivityBase Activity
        {
            get
            {
                return ProcessInstance.Process.GetActivity(Code);
            }
        }

        public override WFVariableDefContainer GetDefContainer()
        {
            return this.Activity;
        }

        internal WFActivityInstance(IWFProcessInstance processInstance)
        {
            ProcessInstance = processInstance;
            State = WFActivityState.Initialized;
        }

        public WFActivityInstance(IWFProcessInstance processInstance, string code) : this(processInstance)
        {
            Code = code;
            processInstance.AddActivity(this);
        }

        public WFActivityInstance(IWFProcessInstance processInstance, string code, WFActivityState state) : this(processInstance, code)
        {
            State = state;
        }

        public virtual IWFActivityInstance GetOrCreateActivity(string activityCode, Type activityType)
        {
            if (this.ProcessInstance.ContainsActivity(activityCode))
            {
                return this.ProcessInstance.GetActivity(activityCode);
            }
            else
            {
                IWFActivityInstance activityInstance;
                activityInstance = new WFActivityInstance(this.ProcessInstance, activityCode);
                return activityInstance;
            }
        }

        public IWFConnectorBase GetDefaultTransition(IDictionary<string, object> args)
        {
            return this.Activity.GetDefaultTransition(this.ProcessInstance, args);
        }

        public IEnumerable<IWFConnector> GetAvailableTransitions(IDictionary<string, object> args = null)
        {
            return this.Activity.GetAvailableTransitions(this.ProcessInstance, args);
        }

        public IEnumerable<IWFConnectorBase> GetTransitions(IDictionary<string,object> args = null)
        {
            return this.Activity.GetTransitions(this.ProcessInstance, args);
        }

        public virtual bool IsCompleted()
        {
            return this.State == WFActivityState.Completed;
        }

        public virtual bool HasParent()
        {
            return Activity.HasParent(this);
        }

        public string GetParentPath()
        {
            return Activity.GetParentPath(this);
        }

        public override IDictionary<string, object> GetVariables()
        {
            IDictionary<string, object> result = base.GetVariables();
            if (HasParent())
            {
                IDictionary<string, object> parentVariables = null;
                IWFActivityInstance parent = this.Activity.GetParent(this);
                if (parent != null)
                if (parent.Code != this.Code)
                {
                    parentVariables = parent.GetVariables();

                    foreach (string key in parentVariables.Keys)
                    {
                        if (!result.ContainsKey(key))
                        {
                            result.Add(key, parentVariables[key]);
                        }
                    }
                }
            }
            return result;
        }


        public virtual void ReadStateFromXmlElement(XElement element)
        {
            this.Code = element.Attribute("Code").Value;
            this.State = (WFActivityState)Enum.Parse(typeof(WFActivityState),element.Attribute("State").Value);
            this.ReadVariablesStateFromXmlElement(element);
        }

        public virtual XElement WriteStateToXmlElement()
        {
            XElement element = new XElement("Activity",
                new XAttribute("Code", this.Code),
                new XAttribute("State", this.State));
            this.WriteVariablesStateToXmlElement(element);
            return element;
        }

        public void Abort(IDictionary<string, object> args = null)
        {
            bool force = false;
            if (args != null && args.ContainsKey("force"))
                force = (bool)args["force"];
            if (this.State == WFActivityState.Active || this.State == WFActivityState.Waiting || force)
                this.Activity.OnAborted(this, args);

            // ustawiamy stan na Init, poniewaz podczas konwersj wywołujemy Abort na WSZYSTKICH ZADANIACH
            // i nie chcemy zeby w następnym przebiegu były jakieś zadania oznaczone jako Completed, które
            // zostały ukończone w poprzednim przebiegu ....
            this.State = WFActivityState.Initialized;
        }

        public WFTransitionStatus DoTransition(IWFActivityInstance activityInstance, string connectorCode/*, Action<IWFActivityInstance> beforeCompleted = null*/, Dictionary<string, object> arguments = null)
        {
            return this.Activity.DoTransition(activityInstance, connectorCode, arguments/*, beforeCompleted*/);
        }

        public void ManualChangeState(string stepCode, WFActivityState state)
        {
            switch(state)
            {
                case WFActivityState.Active:
                    State = WFActivityState.Initialized;
                    break;
                case WFActivityState.Completed:
                    State = WFActivityState.Active;
                    break;
                case WFActivityState.Initialized:
                    State = WFActivityState.Waiting;
                    break;
                case WFActivityState.Waiting:
                    State = WFActivityState.Waiting;
                    break;
            }
        }

        /// <summary>
        /// Próbuje pobrać wartośc zmiennej najpierw z kroku, a potem z procesu. Jesli znajdzie gdzieś zmienną wywołuje operacje przekazaną jako drugi parametr.
        /// </summary>
        /// <param name="name">Nazwa zmiennej</param>
        /// <param name="operation"></param>
        public void TryGetValue(string name, TryGetValueOperation operation)
        {
            if (ContainsVariable(name))
                operation(GetVariableValue(name));
            else if (ProcessInstance.ContainsVariable(name))
                operation(ProcessInstance.GetVariableValue(name));
        }


        public string GetAncestorPath()
        {
            return Activity.GetAncestorPath(this);
        }

        private const string PrefixSplitter = "$";

        private string GetPrefixForVarName()
        {
            //przykład: Code = ADDCONTROL$ADDCON$START to nazwa zmiennej powinna być taka: ADDCONTROL_ADDCON_nazwaparametr
            int index = Code.LastIndexOf(PrefixSplitter);
            if(index <=0)
                return "";
            string prefix = Code.Substring(0, index + 1);
            return prefix.Replace(PrefixSplitter, "_");
        }

        public string GetVariableNameFromSubprocess(string varName)
        {
            return GetPrefixForVarName() + varName;
        }

        /// <summary>
        /// Buduje prefix dzięki któremu można się dostać do zmiennych dla podprocesu z podanych kodów
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public string GetPrefixFromParams(params string[] codes)
        {
            if(codes == null)
                return "";
            string prefix = "";
            foreach (var code in codes)
            {
                prefix += code + "_";
            }
            return prefix;
        }
    }
}
