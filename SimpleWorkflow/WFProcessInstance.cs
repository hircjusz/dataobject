using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using log4net;
using SoftwareMind.SimpleWorkflow.Activities;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public sealed class WFProcessInstance : WFVariableContainer, IWFStateElement, SoftwareMind.SimpleWorkflow.IWFProcessInstance
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFProcessInstance));

        public string ProcessName { get; set; }
        public IWFProcessInstanceBox Box { get; set; }
        public bool IsOnLongRunning { get; set; }

        private IDictionary<string, IWFActivityInstance> Activities { get; set; }
        public IList<IWFActivityInstance> WorkList { get; private set; }

        public System.Guid ID { get; set; }


        private WFProcess _process;

        public WFProcess Process
        {
            get
            {
                if (_process == null)
                {
                    _process = WFProcessFactory.GetProcess(ProcessName);
                }

                return _process;
            }
        }

        /// <summary>
        /// UWAGA: HACK :)
        /// Podczas gdy jest transition do joina, w joinie sprawdzamy które przejscie zostało wykonane za pomocą tego pola
        /// jeśli jest to pierwsze wejscie do joina i connctor ze scieżki 'głównej' to wtedy wykonujemy takiego joina,
        /// jest jest to pierwsze wejscie i inny connector to nie.
        /// </summary>
        public IWFConnector TransitionConnector { get; set; }

        public WFProcessInstance()
        {
            this.Activities = new Dictionary<string, IWFActivityInstance>();
            WorkList = new List<IWFActivityInstance>();
            this.ID = Guid.NewGuid();
        }

        public WFProcessInstance(WFProcess wFProcess) : this()
        {
            this._process = wFProcess;
            this.ProcessName = wFProcess.Name;
        }

        public void AddActivity(IWFActivityInstance activityInstance)
        {
            activityInstance.ProcessInstance = this;
            this.Activities.Add(activityInstance.Code, activityInstance);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>True - jeśli zadania zostały wykonane, False jeśli są jeszcze przetwarzane</returns>
        public bool ExecuteWorkList()
        {
            bool longRunning = false;
            bool cont = false;
            if (!IsOnLongRunning)
            do
            {
                if (WorkList.Count > 0 && WorkList[0].Activity.LongRunning)
                    longRunning = true;

                if (longRunning/* && !IsOnLongRunning*/)
                {
                    IsOnLongRunning = true;
                    ExecuteWorkListAsynchronously();
                    cont = false;
                }
                else
                {
                    cont = !ExecuteWorkListSynchronously();
                }
            } while (cont);

            return !longRunning;
        }

        public void ExecuteWorkListAsynchronously()
        {
            IWFProcessInstanceBox newBox = Box.Repack(this) as IWFProcessInstanceBox;
            new Thread(() =>
            {
                try
                {
                    newBox.ForwardProcessInstance(this, delegate()
                    {
                        while (HasActivityToWork())
                        {
                            var actInst = PopFromWorkList();
                            log.DebugFormat("Wykonywanie worklist: {0}", actInst.Code);
                            actInst.Activity.Run(actInst/*, beforeCompleted*/);
                        }
                    });
                }
                catch (Exception ex)
                {
                    log.Error("Error while executing asynchronous action", ex);
                }
            }).Start();
        }

        public bool ExecuteWorkListSynchronously()
        {
            while (HasActivityToWork())
            {
                if (WorkList[0].Activity.LongRunning)
                    return false;
                var actInst = PopFromWorkList();
                log.DebugFormat("Wykonywanie worklist: {0}", actInst.Code);
                actInst.Activity.Run(actInst);
            }

            return true;
        }

        public void PushToWorkList(IWFActivityInstance activity)
        {
            if (activity.State != WFActivityState.Waiting && activity.State != WFActivityState.Initialized)
            {
                if (activity.ProcessInstance.WorkList.Contains(activity))
                    activity.ProcessInstance.WorkList.Remove(activity);
                activity.ProcessInstance.WorkList.Add(activity);
            }
        }

        public IWFActivityInstance PopFromWorkList()
        {
            IWFActivityInstance result = null;
            if (HasActivityToWork())
            {
                result = WorkList.Take(1).Single();
                WorkList.Remove(result);
            }
            return result;
        }

        public bool HasActivityToWork()
        {
            return WorkList.Count > 0;
        }

        public IWFActivityInstance GetActivity(string code)
        {
            if (!this.Activities.ContainsKey(code))
                this.Activities[code] = new WFActivityInstance(this, code);
            return this.Activities[code];
        }

        public bool ContainsActivity(string code)
        {
            return this.Activities.ContainsKey(code);
        }

        public void RemoveActivity(string code)
        {
            IWFActivityInstance activity = null;

            if (this.Activities.TryGetValue(code, out activity))
                activity.ProcessInstance = null;

            this.Activities.Remove(code);
        }

        public override WFVariableDefContainer GetDefContainer()
        {
            return this.Process;
        }

        public IEnumerable<IWFActivityInstance> GetActivities()
        {
            IList<IWFActivityInstance> activities = new List<IWFActivityInstance>();
            foreach (KeyValuePair<string, IWFActivityInstance> pair in this.Activities)
                activities.Add(pair.Value);
            return activities;
        }

        public bool IsActivityCompleted(string code, string path)
        {
            if (this.ContainsActivity(code))
            {
                return this.GetActivity(code).IsCompleted();
            }
            else
            {
                if (!String.IsNullOrEmpty(path))
                {
                    foreach (string key in Activities.Keys)
                    {
                        if (key.IndexOf(path) >= 0)
                        {
                            if (this.GetActivity(key).IsCompleted() == false)
                                return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public void ReadStateFromXmlElement(System.Xml.Linq.XElement element)
        {
            try
            {
                this.ProcessName = element.Attribute("ProcessName").Value;
                this.ID = Guid.Parse(element.Attribute("ID").Value);


                foreach (var activityElement in element.Descendants("Activities").Single().Descendants("Activity"))
                {
                    WFActivityInstance instance = new WFActivityInstance(this);
                    instance.ReadStateFromXmlElement(activityElement);
                    this.AddActivity(instance);
                }

                XElement variables = element.Elements("Variables").SingleOrDefault();
                if (variables != null)
                {
                    foreach (var variableElement in variables.Descendants("Variable"))
                    {
                        WFVariable variable = new WFVariable();

                        string name = variableElement.Attribute("Name").Value;

                        if (this.GetDefContainer().ContainsVariable(name))
                        {
                            variable.Container = this;
                            variable.ReadStateFromXmlElement(variableElement);
                            this.AddVariable(variable);
                        }
                        else
                        {
                            log.InfoFormat("Brak definicji zmiennej " + name + " dla procesu " + this.ProcessName);
                        }
                    }
                }

                XElement worklist = element.Elements("WorkList").SingleOrDefault();
                if (worklist != null)
                    foreach (var todo in worklist.Descendants("Element"))
                    {
                        string code = todo.Value;
                        var activity = (from a in this.Activities where a.Value.Code == code select a.Value).Single();
                        this.WorkList.Add(activity);
                    }
                /*
                var tran = element.Attribute("TransitionConnector");
                if (tran != null && string.IsNullOrEmpty(tran.Value) == false)
                    this.TransitionConnector = this.GetConnector(element.Attribute("TransitionConnector").Value);
                 */
            }
            catch (Exception ex)
            {
                log.Error("Błąd parsowania stanu procesu: " + element.ToString(), ex);
                throw new ApplicationException("Sprawa posiada nieaktualną wersję procesu.", ex);
            }
        }

        public System.Xml.Linq.XElement WriteStateToXmlElement()
        {
            XElement element = new XElement("Process",
                new XAttribute("ProcessName", this.ProcessName ?? "noname"),
                new XAttribute("ID", this.ID),
                //new XAttribute("TransitionConnector", this.TransitionConnector == null ? "" : TransitionConnector.Code),
                new XElement("WorkList", (from todo in this.WorkList select new XElement("Element", todo.Code))),
                new XElement("Activities",
                   this.Activities.Select(x => x.Value.WriteStateToXmlElement())));
            this.WriteVariablesStateToXmlElement(element);
            return element;
        }

        public bool IsClosed()
        {
            var endSteps = from act in this.Activities where act.Value.Activity.GetType() == typeof(WFEndActivity) select act;
            bool endStepReached = (endSteps.Count() > 0);
            if (endStepReached == false) return false;

            bool workListEmpty = this.WorkList.Count == 0;
            if (workListEmpty == false) return false;

            var pendingSteps = from act in this.Activities
                               where
                                    act.Value.Activity.GetType() != typeof(WFEndActivity) &&
                                    act.Value.State != WFActivityState.Completed &&
                                    act.Value.State != WFActivityState.Initialized // sprawdzamy ten stan - po aborcie, krok jest ustawiany na initialized
                               select act;


            bool nonEndStepsExist = pendingSteps.Count() > 0;

            return endStepReached == true && nonEndStepsExist == false && workListEmpty == true;
        }

        public static WFProcessInstance ReadStateFromXmlString(string xmlContent)
        {
            if (pathResolver != null)
                xmlContent = pathResolver(xmlContent);
            WFProcessInstance instance = null;

            using (System.IO.StringReader reader = new System.IO.StringReader(xmlContent))
            {
                XElement element = XElement.Load(reader);
                instance = new WFProcessInstance();
                instance.ReadStateFromXmlElement(element);
            }

            return instance;
        }

        internal static Func<string, string> pathResolver;

        public void FireSignal(IWFConnector connector, IDictionary<string, object> arguments/*, Action<IWFProcessInstance> onEachTransitionCompleted, Action<IWFActivityInstance> beforeCompleted*/)
        {
//            if (connector.IsAvailable(this, arguments))
            {
                MapToProcesVariables(connector, arguments);
                var activityInstance = GetActivity(connector.Source.Code);
                var destinationActivityInstance = connector.FireSignal(activityInstance, string.Empty, arguments);

                //if (destinationActivityInstance != null)
                //{
                //    SetAppropriateState(connector.Destination.Code, destinationActivityInstance);
                //}
            }
//            else
//            {
//                throw new InvalidOperationException("Operacja nie jest w tej chwili dostępna. Proszę odświeżyć stronę zadania.");
//            }
        }

        public void FireEvent(string eventCode, IDictionary<string, object> arguments/*, Action<IWFProcessInstance> onEachTransitionCompleted, Action<IWFActivityInstance> beforeCompleted*/)
        {
            log.InfoFormat("Event dla procesu: {0}, event: {1}", this.ID, eventCode);

            IWFActivityInstance activityInstance = this.GetActivity(eventCode);
         //   SetAppropriateState(eventCode, activityInstance);

            this.WorkList.Add(activityInstance);
        }

        public WFTransitionStatus DoTransition(string activityCode, string connectorCode, Dictionary<string, object> arguments/*, Action<IWFActivityInstance> beforeCompleted*/)
        {
            IWFConnector connector = connectorCode != null ? this.Process.GetActivity(activityCode).GetDirectlyConnectedTransitions().Where(x => x.Code == connectorCode).Single() : null;

//            this.TransitionConnector = connector;

            IWFActivityInstance activityInstance = this.GetActivity(activityCode);
            if (connector != null)
                MapToProcesVariables(connector, arguments);

            WFTransitionStatus result = activityInstance.DoTransition(activityInstance, connectorCode, /*beforeCompleted,*/ arguments);

            return result;
        }

        /// <summary>
        /// Mapuje argumeny przekazane podczas wywołaia przejsćia na zmienne procesu.
        /// </summary>
        /// <param name="connector">Konektor na którym zostaje wykonane przejście..</param>
        /// <param name="arguments">Argumenty wywołania.</param>
        private void MapToProcesVariables(IWFConnector connector, IDictionary<string, object> arguments)
        {
            if (arguments != null)
                foreach (var item in connector.DialogBoxValueMaping)
                    if (arguments.ContainsKey(item.From))
                        this.SetVariableValue(item.To, arguments[item.From]);
        }

        public IWFConnectorBase GetDefaultTransition(string activityCode, IDictionary<string, object> args = null)
        {
            var connector = this.GetActivity(activityCode).GetDefaultTransition(args);

            //if (connector == null)
            //{
            //    throw new Exception(string.Format("Aktywność {0} nie posiada domyślnego aktywnego przejścia", activityCode));
            //}

            return connector;
        }

        /// <summary>
        /// Ustawia status kroków występująych po danej aktywności na Initialized, aby zadanie mogło się powtórnie wykonać.
        /// </summary>
        /// <param name="activityInstance">Instancja aktywności, od której rozpocznie się zmiana stanu.</param>
        private void SetDependantActivityStateToInitialized(IWFActivityInstance activityInstance)
        {
            activityInstance.State = WFActivityState.Initialized;
            foreach (var activity in activityInstance.GetAvailableTransitions().Where(x => x.ConnectorBehaviourType == Behaviours.WFConnectorBehaviourType.Standard).Select(x => this.GetActivity(x.Destination.Code)).Where(x => x.State == WFActivityState.Completed || x.State == WFActivityState.Waiting))
                SetDependantActivityStateToInitialized(activity);
        }

        public IEnumerable<IWFConnector> GetAvailableGlobalEvents()
        {
            var events = this.Process.RootActivity.ConnectorsOutgoing.Where(x => x.ConnectorBehaviourType == Behaviours.WFConnectorBehaviourType.Signal /*&&
                x.IsAvailable(this, null)*/).ToList();

            events.ForEach(e => e.IsAvailable(this, null));

            return events;
        }

        public IWFConnector GetConnector(string code)
        {
            return GetActivities().SelectMany(x => x.Activity.ConnectorsOutgoing).Where(x => x.Code == code).First();
        }

        public IWFProcessInstance ProcessOwner { get; set; }

        #region IWFProcessInstance Members

        IEnumerable<IWFConnectorBase> IWFProcessInstance.GetTransitions(string activityCode, IDictionary<string, object> args)
        {
            return this.GetActivity(activityCode).GetTransitions(args);
        }

        public IWFProcessInstance ChangeProcessDefinition(string processDefinitionCode)
        {
            this._process = WFProcessFactory.GetProcess(processDefinitionCode);
            this.ProcessName = processDefinitionCode;
            return this;
        }

        /// <summary>
        /// Sprawdza czy zadanie jest w stanie Waiting. Metoda dla skryptów.
        /// </summary>
        /// <param name="code">Kod aktywności.</param>
        /// <returns>true jeśli jest w stanie wWaiting; w przeciwnym wypadku false.</returns>
        public bool IsWaiting(string code)
        {
            return GetActivity(code).State == WFActivityState.Waiting;
        }

        /// <summary>
        /// Sprawdza czy zadanie jest w stanie Active. Metoda dla skryptów.
        /// </summary>
        /// <param name="code">Kod aktywności.</param>
        /// <returns>true jeśli jest w stanie Active; w przeciwnym wypadku false.</returns>
        public bool IsActive(string code)
        {
            return GetActivity(code).State == WFActivityState.Active;
        }

        public bool IsAnyActive(string stepCode)
        {
            IEnumerable<IWFActivityInstance> activities = this.GetActivities(stepCode);
            return activities.Any(a => a.State == WFActivityState.Waiting);
        }

        public override void SetVariableValue(string name, object value)
        {
            OnVariableValueChanged(name, value);
            base.SetVariableValue(name, value);
        }

        private void OnVariableValueChanged(string name, object value)
        {
            if (base.ContainsVariable(name) == false) return;

            object oldValue = base.GetVariableValue(name);
            IDictionary<string, object> variables = new Dictionary<string, object>();
            IDictionary<string, object> processVariables = this.GetVariables();

            foreach (var processVar in processVariables)
            {
                variables.Add(processVar.Key, processVar.Value);
            }

            bool changed = false;
            if (oldValue == null && value != null)
                changed = true;
            if (oldValue != null && value == null)
                changed = true;
            if (oldValue != null && value != null)
                changed = !oldValue.Equals(value);

            if ((string.IsNullOrEmpty(this.Process.OnVariableChanged) == false) && changed)
            {
                variables.Add("name", name);
                if (oldValue != null)
                    variables.Add("newValue", Convert.ChangeType(value, oldValue.GetType()));
                else
                    variables.Add("newValue", value);
                variables.Add("oldValue", oldValue);
                variables.Add("instance", this);

                Scripts.Script script = new Scripts.Script(this.Process.OnVariableChanged, variables);
                script.Execute();
            }

            foreach (var scriptVar in variables)
            {
                if(processVariables.ContainsKey(scriptVar.Key))
                    base.SetVariableValue(scriptVar.Key, scriptVar.Value);
            }
        }

        public IEnumerable<IWFActivityInstance> GetActivities(string realActivityCode)
        {
            foreach (var activityEntry in this.Activities)
            {
                if (WFProcess.GetRealCode(activityEntry.Key) == realActivityCode)
                {
                    yield return activityEntry.Value;
                }
            }
        }

        #endregion

        [Serializable]
        public class TransparentBox : IWFProcessInstanceBox
        {
            IWFProcessInstance processInstance;

            public TransparentBox(IWFProcessInstance processInstance)
            {
                this.processInstance = processInstance;
            }

            public IWFProcessInstance UseProcessInstance(WFProcessInstanceOperation operation, bool forceLock = false)
            {
                if(processInstance != null)
                    processInstance.Box = this;

                try
                {
                    if (operation != null)
                        processInstance = operation(processInstance);
                }
                finally
                {
                    processInstance.Box = null;
                }
                return processInstance;
            }

            public IWFProcessInstance ForwardProcessInstance(IWFProcessInstance instance, WFProcessActivityOperation operation)
            {
                if (operation != null)
                    operation();
                return processInstance;
            }

            public void Transactional(IWFProcessInstance instance, bool recovery, WFProcessActivityOperation operation)
            {
                if (operation != null)
                    operation();
            }


            public void Dispose()
            {
                processInstance = null;
            }

            public IWFProcessInstanceBox Repack(IWFProcessInstance instance)
            {
                return new TransparentBox(instance);
            }

        }

        /// <summary>
        /// Zwraca aktywności bez rodzica
        /// </summary>
        /// <param name="code">Kod</param>
        /// <returns>Aktywności</returns>
        public IEnumerable<IWFActivityInstance> GetActivitiesWithoutParent(string code)
        {
            return GetActivities(code).Where(x => !x.HasParent());
        }

        /// <summary>
        /// Zwraca aktywności z konkretnym rodzicem
        /// </summary>
        /// <param name="code">Kod</param>
        /// <param name="paretnPath">ścieżka rodizca</param>
        /// <returns>Aktywności</returns>
        public IEnumerable<IWFActivityInstance> GetActivitiesWithParent(string code, string paretnPath)
        {
            return GetActivities(code).Where(x => x.HasParent() && x.GetParentPath() == paretnPath);
        }

        /// <summary>
        /// Zwraca aktywności z konkretnym przodkiem - pierwszym ze wszystkich przodków - jeżeli ma tylko rodzica to zwraca taki sam jak w przypadku GetActivietiesWithParent
        /// </summary>
        /// <param name="code">Kod</param>
        /// <param name="paretnPath">ścieżka rodizca</param>
        /// <returns>Aktywności</returns>
        public IEnumerable<IWFActivityInstance> GetActivitiesWithAncestor(string code, string paretnPath)
        {
            return GetActivities(code).Where(x => x.HasParent() && x.GetAncestorPath() == paretnPath);
        }
    }
}
