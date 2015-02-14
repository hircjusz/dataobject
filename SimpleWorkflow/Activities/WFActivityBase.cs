using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using SoftwareMind.Utils.Extensions.Xml;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.SimpleWorkflow.Misc;
using System.Diagnostics;
using SoftwareMind.SimpleWorkflow.Behaviours;
using SoftwareMind.SimpleWorkflow.Exceptions;
using log4net;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    [DebuggerDisplay("{Code} {Caption}")]
    public abstract class WFActivityBase : WFVariableDefContainer, IWFTemplateElement, ICloneable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFActivityBase));

        [Browsable(false)]
        public IList<IWFConnector> ConnectorsOutgoing { get; private set; }

        [Browsable(false)]
        public IList<IWFConnector> ConnectorsIncoming { get; private set; }
        /// <summary>
        /// Dodatkowe dane odnoœnie kroku np.: do jakiego etapu nale¿y krok
        /// </summary>
        [Editor(typeof(WFMetaDataCollectionEditor), typeof(UITypeEditor))]
        [Category("Common")]
        [Description("Metadane aktywnoœci.")]
        public EditableList<WFActivityMetaData> MetaData { get; private set; }
        /// <summary>
        /// Kod kroku powininien byæ unikalny.
        /// Odpytywaæ czy dana aktywnoœæ ma podany kod, powinniœmy siê
        /// przez metode HasCode(code). W przypadku kiedy aktywnoœæ jest 'multiple' aktywnoœæ mo¿e mieæ inny kod,
        /// ale aktywnoœæ która jest zwielokrotniona taki, którego poszukujemy.
        /// </summary>
        [Category("Common")]
        [Description("Kod aktywnoœæi. Musi byæ unikalny.")]
        public string Code { get; set; }
        /// <summary>
        /// Nazwa kroku dla u¿ytkownika. Np.: Wprowadzanie notatki RSU
        /// </summary>
        [Category("Common")]
        [Description("Nazwa aktywnoœci wyœwietlana pod ikonk¹ aktywnoœci.")]
        public string Caption { get; set; }
        /// <summary>
        /// Opis s³owno-muzyczny dla u¿ytkownika
        /// </summary>
        [Category("Common")]
        [Description("Opis aktywnoœci.")]
        public string Decription { get; set; }

        [Browsable(false)]
        public WFProcess Process { get; set; }

        [Browsable(false)]
        public DesignerActivityInfo DesignerSettings { get; set; }

        /// <summary>
        /// Skrypt do wykonania przed dan¹ aktywnoœci¹.
        /// </summary>
        /// <value>Skrypt.</value>
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        [Category("Common")]
        [Description("Skrypt do wykonania przed dan¹ aktywnoœci¹.")]
        public string StartScript { get; set; }

        /// <summary>
        /// Skrypt do wykonania przed dan¹ aktywnoœci¹.
        /// </summary>
        /// <value>Skrypt.</value>
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        [Category("Common")]
        [Description("Skrypt do wykonania .")]
        public string ExecuteScript { get; set; }

        /// <summary>
        /// Skrypt do wykonania po danej aktywnoœci.
        /// </summary>
        /// <value>Skrypt.</value>
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        [Category("Common")]
        [Description("Skrypt do wykonania po danej aktywnoœci.")]
        public string EndScript { get; set; }

        [Browsable(true)]
        [Category("Common")]
        [DefaultValue(false)]
        public bool LongRunning { get; set; }

        ///<remarks>
        ///Ten konstruktor powinien byæ internal, ale rhino mocks nie moze wtedy zrobiæ proxy
        ///nie wiem czemu...
        /// </remarks>
        protected WFActivityBase()
        {
            this.ConnectorsOutgoing = new List<IWFConnector>();
            this.ConnectorsIncoming = new List<IWFConnector>();
            this.MetaData = new EditableList<WFActivityMetaData>();
            this.DesignerSettings = new DesignerActivityInfo();
        }

        internal protected virtual void DoTransitions(IWFActivityInstance activityInstance)
        {
            foreach (IWFConnector connector in this.ConnectorsOutgoing.OrderByDescending(x => x.ConnectorBehaviourType))
                connector.Run(activityInstance);
        }


        /// <returns>Zwraca 0 jeœli nie mo¿na wykonaæ przejœcia z powodu nie dostêpnego po³¹czenia</returns>
        public WFTransitionStatus DoTransition(IWFActivityInstance activityInstance, string connectorCode, Dictionary<string, object> arguments = null, Action<IWFActivityInstance> beforeCompleted = null)
        {
            activityInstance.AddVariables(arguments);
            if (arguments != null)
            {
                var vars = activityInstance.GetVariables();
                foreach (var arg in vars)
                    if (!arguments.ContainsKey(arg.Key))
                        arguments.Add(arg.Key, arg.Value);
                foreach (var arg in arguments)
                    if (!vars.ContainsKey(arg.Key))
                        activityInstance.SetVariableValue(arg.Key, arg.Value);
            }

            if (activityInstance.State == WFActivityState.Initialized)
                throw new InvalidOperationException("Krok jest w stanie initialized");

            bool force = false;
            if (activityInstance.State == WFActivityState.Executed || activityInstance.State == WFActivityState.Waiting || IsArgumentTrue(arguments, "ignoreState"))
            {
                bool isConnectorAvailable = true;

                WFTransitionStatus status = CanComplete(activityInstance);

                if (status.CompareTo(WFTransitionStatus.Successful) != 0 && !IsArgumentTrue(arguments, "force"))
                {
                    return status;
                }

                if (String.IsNullOrEmpty(connectorCode) == false)
                {
                     IWFConnector connector = (from c in this.ConnectorsOutgoing where c.Code == connectorCode select c).Single();
                     isConnectorAvailable = connector.IsAvailable(activityInstance.ProcessInstance, arguments);
                }

                if (isConnectorAvailable)
                {
                    force = true;// pomijamy sprawdzanie warunku IsAvailable gdy¿ zosta³juz sprawdzony
                    activityInstance.AddVariables(arguments);
                    if (activityInstance.State == WFActivityState.Waiting)
                        activityInstance.State = WFActivityState.Executed;
                    ChangeState(activityInstance/*, beforeCompleted*/);
                }
                else
                    return WFTransitionStatus.ConnectorNotAvailable;
            }


            if (activityInstance.State == WFActivityState.Completed || IsArgumentTrue(arguments, "ignoreState"))
            {
                if (String.IsNullOrEmpty(connectorCode)) //for example: on subprocess close
                {
                    DoTransitions(activityInstance);
                }
                else
                {
                    IWFConnector connector = (from c in this.ConnectorsOutgoing where c.Code == connectorCode select c).Single();
                    connector.Run(activityInstance, force: force, args: arguments);
                }

                return WFTransitionStatus.Successful;
            }
            else
                throw new InvalidOperationException("Nie mo¿na zakoñczyæ kroku ¿ród³owego");
        }

        private static bool IsArgumentTrue(Dictionary<string, object> arguments, string name)
        {
            return arguments != null && arguments.ContainsKey(name) && (bool)arguments[name] == true;
        }

        /// <summary>
        /// Jeœli kroku nie mo¿na zakoñczyæ zwracamy rezultat opisuj¹cy w jakiœ sposób
        /// zaistnia³¹ sytuacje
        /// </summary>
        /// <returns></returns>
        protected virtual WFTransitionStatus CanComplete(IWFActivityInstance activityInstance)
        {
            return WFTransitionStatus.Successful;
        }

        public bool HasParent(IWFActivityInstance activityInstance)
        {
            return activityInstance.Code.IndexOf('|') > 0;
        }

        public virtual string GetParentPath(WFActivityInstance activityInstance)
        {
            if (HasParent(activityInstance))
                return activityInstance.Code.Substring(activityInstance.Code.IndexOf('|'));
            else
                return "";
        }

        /// <summary>
        /// Zwraca oznaczenie najwczeœniejszego rodzica
        /// </summary>
        /// <param name="activityInstance"></param>
        /// <returns></returns>
        public virtual string GetAncestorPath(WFActivityInstance activityInstance)
        {
            if (HasParent(activityInstance))
                return activityInstance.Code.Substring(activityInstance.Code.LastIndexOf('|'));
            return "";
        }

        public int GetCurrentPathIndex(IWFActivityInstance activityInstance)
        {
            if (HasParent(activityInstance))
            {
                int idx = activityInstance.Code.IndexOf('|');
                string path = activityInstance.Code.Substring(idx);
                idx = path.IndexOf('#');
                path = path.Substring(0, idx);
                return Int32.Parse(path);
            }
            else
                return -1;
        }



        public IDictionary<string, object> GetParentVariables(IWFActivityInstance instance)
        {
            if (HasParent(instance))
            {
                IWFActivityInstance parent = GetParent(instance);
                return parent.GetVariables();
            }
            else
            {
                return new Dictionary<string, object>();
            }
        }

        public IWFActivityInstance GetParent(IWFActivityInstance instance)
        {
            if (HasParent(instance))
            {
                string path = instance.GetParentPath();
                if (!String.IsNullOrEmpty(path))
                {
                    int idx = path.IndexOf('#');
                    string subpath = path.Substring(idx + 1);
                    IWFActivityInstance parent = instance.ProcessInstance.GetActivity(subpath);
                    string firstPathActivityCode = parent.Activity.ConnectorsOutgoing[0].Destination.Code;
                    string firstPathActivityInstanceCode = firstPathActivityCode + path;
                    return instance.ProcessInstance.GetActivity(firstPathActivityInstanceCode);
                }
            }
            return null;
        }

        public IEnumerable<WFActivityBase> GetDirectlyConnectedActivities(bool realActivitiesOnly = true)
        {
            foreach (WFConnector connector in this.ConnectorsOutgoing)
            {
                yield return connector.Destination;
            }
        }

        public IEnumerable<IWFConnector> GetDirectlyConnectedTransitions()
        {
            foreach (WFConnector connector in this.ConnectorsOutgoing)
            {
                yield return connector;
            }
        }

        public virtual bool AutoExecute(IWFActivityInstance instance) // oznacza czy krok ma w transakcji connectora od razu po aktywacji wykonac sie. np. wyslanie e-mail nie powinno aby w przypadku rollback na connectorze nie zostal wyslany niepotrzebny mail
        {
            return !LongRunning || instance.ProcessInstance.IsOnLongRunning;
        }

        public IEnumerable<IWFConnector> GetDirectlyConnectedIncomingTransitions()
        {
            return this.ConnectorsIncoming;
        }

        /// <summary>
        /// Opisuje stany w jakich mo¿e znajdowaæ sie pojedynczy krok oraz w jakie nastepne stany krok moze przejsc.
        /// Warunki przejscia definiowane s¹ w metodzie CanChangeStateTo
        /// </summary>
        public virtual void ChangeState(IWFActivityInstance instance/*, Action<IWFActivityInstance> beforeCompleted*/)
        {
            switch (instance.State)
            {
                case WFActivityState.Initialized:
                    instance.State = WFActivityState.Active;
                    OnActivated(instance);
                    if (AutoExecute(instance))
                        ChangeState(instance);
                    break;
                case WFActivityState.Active:
                    if (Execute(instance))
                        instance.State = WFActivityState.Executed;
                    else
                    {
                        instance.State = WFActivityState.Waiting;
                        OnWait(instance);
                    }
                    break;
                case WFActivityState.Executed:
                    instance.State = WFActivityState.Completed;
                    OnCompleted(instance/*, beforeCompleted*/);
                    break;
                case WFActivityState.Waiting:
                    instance.State = WFActivityState.Active;
                    ChangeState(instance/*, beforeCompleted*/);
                    break;
                case WFActivityState.Completed:
                    instance.State = WFActivityState.Initialized;
                    ChangeState(instance/*, beforeCompleted*/);
                    break;
            }
        }

        /// <summary>
        /// Sprawdza czy struktura procesu jest prawid³owa i jeœli nie rzuca wyj¹tek
        /// </summary>
        /// <param name="validated">Zbiór zwalidowanych wczeœniej wêz³ów. Wykorzystywany do eleminacji przepe³nienia stosu w przypadku
        /// istnienia pêtli w diagramie.</param>
        protected virtual void Validate(HashSet<WFActivityBase> validated)
        {
            foreach (WFConnector connector in this.ConnectorsOutgoing)
            {
                if (validated.Contains(connector.Destination))
                    continue;
                validated.Add(connector.Destination);
                connector.Validate(validated);
            }

            IWFConnector con = this.ConnectorsOutgoing.GroupBy(x => x.Code).Where(x => x.Count() > 1).Select(x => x.First()).FirstOrDefault();
            if(con != null)
                throw new WFDesignException(String.Format("Z kroku {0} wychodz¹ conajmniej dwa zdarzenia o kodzie {1}.", Code, con.Code), Code);
        }

        /// <summary>
        /// Rozpoczyna walidacjê wêz³oa
        /// </summary>
        /// <param name="visitedActivites">Zbiór zwalidowanych wczeœniej wêz³ów. Wykorzystywany do eleminacji przepe³nienia
        /// stosu w przypadku istnienia pêtli w diagramie.</param>
        internal void StartValidate(HashSet<WFActivityBase> visitedActivites)
        {
            Validate(visitedActivites);
        }

        public IEnumerable<string> GetPossibleConnectors()
        {
            return from a in this.ConnectorsOutgoing select a.Caption;
        }

        /// <summary>
        /// Zapisuje wzór obiektu do xml-a
        /// </summary>
        public virtual XElement WriteTemplateToXmlElement()
        {
            WFMetaDataReaderWriter writer = new WFMetaDataReaderWriter();

            return new XElement("Activity",
                                    new XAttribute("Code", this.Code ?? ""),
                                    new XAttribute("Caption", this.Caption ?? ""),
                                    new XAttribute("Description", this.Decription ?? ""),
                                    new XAttribute("Type", this.GetType().GetShortName()),
                                    new XAttribute("DesignerSettings", this.DesignerSettings.ToString()),
                                    new XAttribute("StartScript", this.StartScript ?? ""),
                                    new XAttribute("EndScript", this.EndScript?? ""),
                                    new XAttribute("ExecuteScript", this.ExecuteScript ?? ""),
                                    new XAttribute("LongRunning", this.LongRunning),
                                    writer.WriteTemplateToXmlElement(this.MetaData),
                                    from v in this.VariableDefs select v.Value.WriteTemplateToXmlElement());


        }

        /// <summary>
        /// Wczytuje ustawienia obiektu z xmla
        /// </summary>
        public virtual void ReadTemplateFromXmlElement(XElement element)
        {
            string code = element.Attribute("Code").Value;
            string name = element.Attribute("Caption").Value;
            string desc = element.Attribute("Description").Value;
            var desingerSetingsAtribute = element.Attribute("DesignerSettings");
            if (desingerSetingsAtribute != null)
                this.DesignerSettings = (DesignerActivityInfo)desingerSetingsAtribute.Value;
            else
                this.DesignerSettings = new DesignerActivityInfo();
            this.Code = code;
            this.Caption = name;
            this.Decription = desc;

            this.StartScript = element.GetAttributeStringValueOrNull("StartScript");
            this.ExecuteScript = element.GetAttributeStringValueOrNull("ExecuteScript");
            this.EndScript = element.GetAttributeStringValueOrNull("EndScript");
            this.LongRunning = element.GetAttributeBooleanValueOrDefault("LongRunning", false);

            IEnumerable<XElement> variablesElements = element.Elements("VariableDef");

            foreach (var definition in variablesElements)
            {
                ReadVariableDef(definition);
            }

            WFMetaDataReaderWriter reader = new WFMetaDataReaderWriter();
            this.MetaData = reader.ReadTemplateFromXmlElement(element);
        }

        private void ReadVariableDef(XElement definitionElement)
        {
            WFVariableDef definition = new WFVariableDef();
            definition.ReadTemplateFromXmlElement(definitionElement);
            VariableDefs.Add(definition);
        }

        public void Run(IWFActivityInstance instance/*, Action<IWFActivityInstance> beforeCompleted = null*/)
        {
            if (null == instance.ProcessInstance)
            {
                throw new ArgumentNullException("ProcessInstance");
            }

            if (null == instance.ProcessInstance.Box)
            {
                throw new ArgumentNullException("ProcessInstance.Box");
            }

            instance.ProcessInstance.Box.Transactional(instance.ProcessInstance, true, delegate()
            {
                bool stop = false;
                do
                {
                    WFActivityState startState = instance.State;
                    ChangeState(instance);
                    stop = instance.State == WFActivityState.Completed || instance.State == WFActivityState.Waiting || instance.State == startState;
                } while (!stop);
                if (instance.State == WFActivityState.Completed)
                    DoTransitions(instance);
            });
        }

        public virtual void OnActivated(IWFActivityInstance instance)
        {
            log.DebugFormat("Workflow activity {0} activated", instance.Code);

            if (!String.IsNullOrEmpty(StartScript))
                ScriptHelper.Execute(StartScript, instance);
        }

        public virtual void OnCompleted(IWFActivityInstance instance/*, Action<IWFActivityInstance> beforeCompleted*/)
        {
//            if(beforeCompleted != null)
//                beforeCompleted(instance);
            log.DebugFormat("Workflow activity {0} completed", instance.Code);
            if (!String.IsNullOrEmpty(EndScript))
                ScriptHelper.Execute(EndScript, instance);
        }

        public virtual void OnWait(IWFActivityInstance instance)
        {
            log.DebugFormat("Workflow activity {0} waiting", instance.Code);
        }

        public virtual void OnAborted(IWFActivityInstance instance, IDictionary<string, object> args = null)
        {
            log.DebugFormat("Zadanie: {0} zosta³o abortowane", this.Code);
        }

        protected virtual bool Execute(IWFActivityInstance instance)
        {
            if (!String.IsNullOrEmpty(ExecuteScript))
                ScriptHelper.Execute(ExecuteScript, instance);
            return true;
        }

        internal IEnumerable<IWFConnector> GetAvailableTransitions(IWFProcessInstance processInstance, IDictionary<string, object> args = null)
        {
            var availableConnectors = from a in this.ConnectorsOutgoing
                    where a.IsAvailable(processInstance, args) == true
                    select a;

            return availableConnectors;
        }

        internal virtual bool WaitingToBeExecuted(IWFProcessInstance iWFProcessInstance, HashSet<string> checkedConnectors)
        {
            if (iWFProcessInstance.ContainsActivity(this.Code) == false)
            {
                var connecotrs = (from a in this.ConnectorsIncoming
                                  where a.WaitingToBeExecuted(iWFProcessInstance, checkedConnectors) == true
                select a.Source.Code).ToArray();

                return connecotrs.Length > 0;
            }

            var act = iWFProcessInstance.GetActivity(this.Code);
            if (act.State == WFActivityState.Completed || act.State == WFActivityState.Initialized)
                return false;

            return true;
        }

        internal IEnumerable<IWFConnectorBase> GetTransitions(IWFProcessInstance iWFProcessInstance, IDictionary<string, object> args)
        {
            var connectors = this.ConnectorsOutgoing.ToList();
            // poprzez wywo³anie metody IsAvailable ustawiamy flage isConnectorAvailable :)
            connectors.ForEach(c => c.IsAvailable(iWFProcessInstance, args));
            return connectors;
        }

        internal IWFConnectorBase GetDefaultTransition(IWFProcessInstance iWFProcessInstance, IDictionary<string, object> args)
        {
            var connectorsCollection = from connector in this.ConnectorsOutgoing
                                       where connector.ConnectorBehaviour.GetType() == typeof(WFStandardConnectorBehaviour)
                                       && connector.IsAvailable(iWFProcessInstance, args)
                                       && connector.IsSystem == false
                                       select connector;

            return connectorsCollection.FirstOrDefault();
        }

        public abstract object Clone();
    }
}