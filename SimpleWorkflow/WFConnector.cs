using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using SoftwareMind.Scripts;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;
using SoftwareMind.SimpleWorkflow.Behaviours;
using SoftwareMind.SimpleWorkflow.Activities;
using SoftwareMind.Utils.Extensions.Xml;
using System.Diagnostics;


namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    [DebuggerDisplay("{Code} {Caption} [{Source}]->[{Destination}]")]
    public class WFConnector : IWFConnector
    {
        public WFActivityBase destination;

        [Browsable(false)]
        public WFActivityBase Destination
        {
            get
            {
                return destination;
            }
            set
            {
                if (destination != value)
                {
                    if (value != null && source != null)
                        Validate(source, value);
                    if (destination != null && destination.ConnectorsIncoming.Contains(this))
                        destination.ConnectorsIncoming.Remove(this);
                    destination = value;
                    if (value != null)
                        destination.ConnectorsIncoming.Add(this);
                }

            }
        }

        public WFActivityBase source;

        [Browsable(false)]
        public WFActivityBase Source
        {
            get
            {
                return source;
            }
            set
            {
                if (source != value)
                {
                    if (value != null && destination != null)
                        Validate(value, destination);

                    if (source != null && source.ConnectorsOutgoing.Contains(this))
                        source.ConnectorsOutgoing.Remove(this);
                    source = value;
                    if (value != null)
                        source.ConnectorsOutgoing.Add(this);
                }
            }
        }

        [Description("Kod po≥aczenia.")]
        public String Code { get; set; }

        [Description("Nag≥Ûwek.")]
        public String Caption { get; set; }

        protected bool? isConnectorAvailable;

        [Browsable(false)]
        public bool IsConnectorAvailable
        {
            get
            {
                if (this.isConnectorAvailable.HasValue == false)
                    throw new InvalidOperationException("Metoda IsAvailable() musi byÊ wykonana przed uøyciem tego prop.");
                return isConnectorAvailable.Value;
            }
            private set
            {
                this.isConnectorAvailable = value;
            }
        }

        [Description("åcieøka g≥Ûwna")]
        public bool MainPath { get; set; }

        /// <summary>
        /// Nazwa okna, ktÛre ma zostaÊ wyúwietlone uøytkownikowi przed przejsciem przed wykonaniem tranzycji.
        /// Jeúli puste nie zostanie wyúwielone øadne okno.
        /// </summary>
        [Description("Nazwa okna, ktÛre ma zostaÊ wyúwietlone uøytkownikowi przed przejsciem przed wykonaniem tranzycji. Jeúli puste nie zostanie wyúwielone øadne okno.")]
        public string DialogBoxName { get; set; }

        /// <summary>
        /// Pozwala pobraÊ i ustawiÊ mapowanie zmiennych okna dialogowego
        /// </summary>
        /// <value>mapowanie okna dialogowego.</value>
        [Editor(typeof(DialogBoxMappingCollectionEditor), typeof(UITypeEditor))]
        [Description("Mapowanie zmiennych okna dialogowego na zmienne procesu")]
        public EditableList<MappingItem> DialogBoxValueMaping { set; get; }

        /// <summary>
        /// Pozwala pobraÊ i ustawiÊ listÍ paramaetrÛw jaka zostanie pzrekazana do okna dialogowego przed wyúwietleniem.
        /// </summary>
        /// <value>Lista parametrÛw.</value>
        [Description("Lista paramaetrÛw jaka zostanie pzrekazana do okna dialogowego przed wyúwietleniem.")]
        public EditableList<ParameterItem> DialogBoxParameters { set; get; }

        [Description("Skrypt warunku.")]
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        public String ConditionScript { get; set; }

        [Description("Skrypt polisy.")]
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        public String PolicyScript { get; set; }

        [Browsable(false)]
        public IList<IWFPolicyRule> Policies
        {
            get
            {
                return DesingerPolicies.Select(x => x.Policy).ToList();
            }
            set
            {
                DesingerPolicies.Clear();
                if (value != null)
                    foreach (IWFPolicyRule policy in value)
                        DesingerPolicies.Add(new DesingerPolicyItem()
                        {
                            Policy = policy,
                        });
            }
        }

        [Browsable(false)]
        public IList<IWFCondition> Conditions
        {
            get
            {
                return DesingerConditions.Select(x => x.Condition).ToList();
            }
            set
            {
                DesingerConditions.Clear();
                if (value != null)
                    foreach (IWFCondition condition in value)
                        DesingerConditions.Add(new DesingerConditionItem()
                        {
                            Condition = condition,
                        });
            }
        }

        [Editor(typeof(WFMetaDataCollectionEditor), typeof(UITypeEditor))]
        [Description("Metadane po≥πczenia.")]
        public EditableList<WFActivityMetaData> MetaData { get; private set; }

        [Browsable(false)]
        public DesingerConnectorInfo DesignerSettings { get; set; }

        [DisplayName("Conditions")]
        [Description("Warunki.")]
        [Editor(typeof(WFConditionCollectionEditor), typeof(UITypeEditor))]
        public List<DesingerConditionItem> DesingerConditions { get; set; }

        [DisplayName("Policies")]
        [Description("Polisy.")]
        [Editor(typeof(WFPolicyCollectionEditor), typeof(UITypeEditor))]
        public List<DesingerPolicyItem> DesingerPolicies { get; set; }

        [Browsable(false)]
        public IWFConnectorBehaviour ConnectorBehaviour { get; set; }

        /// <summary>
        /// Czy moøna uruchomiÊ tylko jeden raz ?
        /// </summary>
        [DisplayName("SingleInstance")]
        [Description("Czy moøna uruchomiÊ tylko jeden raz ?")]
        public Boolean SingleInstance { get; set; }

        /// <summary>
        /// Czy wymusza uruchomienie tylko jeden raz w sygnalach tzn nawet jak zadanie bylo jyz zakonczone to jest wznawiane
        /// </summary>
        [DisplayName("ForceSingleInstance")]
        [Description("Czy wymusza uruchomienie tylko jeden raz ?")]
        public Boolean ForceSingleInstance { get; set; }

        [DisplayName("SystemConnection")]
        [Description("Czy jest to po≥πczenie systemowe, tj. niewidoczne z GUI")]
        public Boolean IsSystem { get; set; }

        [DisplayName("ConnectorBehaviour")]
        [Description("Zachowanie konektora.")]
        [DefaultValue(WFConnectorBehaviourType.Standard)]
        public WFConnectorBehaviourType ConnectorBehaviourType
        {
            get
            {
                return WFConnectorBehaviourBase.GetBehaviourType(ConnectorBehaviour);
            }
            set
            {
                if (WFConnectorBehaviourBase.GetBehaviourType(ConnectorBehaviour) != value)
                {
                    ValidateBahaviour(Source, Destination, ConnectorBehaviourType);
                    ConnectorBehaviour = WFConnectorBehaviourBase.Create(value);
                }
            }
        }

        [Description("Czy jest dostepny nawet przy niewype≥nionych todosach")]
        [DisplayName("IsActiveWithoutTodos")]
        public Boolean IsActiveWithoutTodos { get; set; }

        private Type _destinator;
        private IWFDestinationSplitter _destinatorInstance;

        [DisplayName("DestinationSplitterType")]
        [Description("Rozdzielenie zadan")]
        [TypeConverter(typeof(TypeSelector))]
        [TypeSelectorInterfaceTypeAtribute(typeof(IWFDestinationSplitter))]
        [DefaultValue(null)]
        public Type DestinationSplitterType
        {
            get { return this._destinator; }
            set
            {
                this._destinator = value;
                this._destinatorInstance = value == null ? null : (IWFDestinationSplitter)Activator.CreateInstance(value);
            }
        }

        [Browsable(false)]
        public IWFDestinationSplitter DestinationSplitter
        {
            get { return this._destinatorInstance; }
            set
            {
                this._destinator = value == null ? null : value.GetType();
                this._destinatorInstance = value;
            }
        }

        [Description("KolejnoúÊ prezentacji przyciskÛw")]
        [DisplayName("Order")]
        public int Order { get; set; }

        [Description("Czy usunπÊ informacjÍ o rodzicu (niejawna demultiplikacja o n poziomÛw)")]
        [DisplayName("DeleteParentPath")]
        public int DeleteParentPath { get; set; }

        [Description("Czy pomimo tego øe jest to event pokazywac go jako przycisk")]
        [DisplayName("ShowAsButton")]
        public Boolean ShowAsButton { get; set; }

        /// <summary>
        /// Sprawdza czy dane po≥πczenie jest prawid≥owe w kontekúcie w≥aúcioúci ConnectorBehaviour.
        /// </summary>
        /// <param name="Source">èrÛd≥o.</param>
        /// <param name="Destination">Cel.</param>
        /// <param name="behaviourType">Zachowanie.</param>
        private void ValidateBahaviour(WFActivityBase Source, WFActivityBase Destination, WFConnectorBehaviourType behaviourType)
        {
           // if (behaviourType == Behaviours.WFConnectorBehaviourType.Signal && !(Destination is WFHandleEventActivity))
           //     throw new WFDesignException("KoÒcowym elementem ≥πcznika typu 'Signal' moøe byÊ tylko zdarzenie", Code);
        }

        /// <summary>
        /// Sprawdza czy dane po≥πczenie jest prawid≥owe w kontekúcie w≥aúcioúci ConnectorBehaviour.
        /// </summary>
        /// <param name="Source">èrÛd≥o.</param>
        /// <param name="Destination">Cel.</param>
        /// <param name="behaviourType">Zachowanie.</param>
        public static bool IsBahaviourValid(WFActivityBase Source, WFActivityBase Destination, WFConnectorBehaviourType behaviourType)
        {
//            if (behaviourType == Behaviours.WFConnectorBehaviourType.Signal && !(Destination is WFHandleEventActivity))
//                return false;

            return true;
        }

        public WFConnector(IWFConnectorBehaviour behaviour)
        {
            this.DesingerConditions = new List<DesingerConditionItem>();
            this.DesingerPolicies = new List<DesingerPolicyItem>();
            this.DesignerSettings = new DesingerConnectorInfo();
            this.MetaData = new EditableList<WFActivityMetaData>();
            this.ConnectorBehaviour = behaviour;
            this.ConnectorBehaviour.SetConnector(this);
            this.DialogBoxValueMaping = new EditableList<MappingItem>();
            this.DialogBoxParameters = new EditableList<ParameterItem>();
            this.SingleInstance = false;
            this.IsActiveWithoutTodos = false;
            this.Order = 10;
            this.DeleteParentPath = 0;
            this.ShowAsButton = false;
            this.DestinationSplitter = null;
            this.ForceSingleInstance = false;
        }

        internal WFConnector(WFActivityBase source, WFActivityBase destination, IWFConnectorBehaviour behaviour)
            : this(behaviour)
        {
            Validate(source, destination);
            this.Source = source;
            this.Destination = destination;
        }

        private void Validate(WFActivityBase source, WFActivityBase destination)
        {
            ValidateBahaviour(source, destination, ConnectorBehaviourType);
            ConnectorBehaviour.Validate(source, destination);
        }

        public void Disconnect()
        {
            if (this.Source.ConnectorsOutgoing.Contains(this))
            {
                this.Source.ConnectorsOutgoing.Remove(this);
            }

            if (this.Destination.ConnectorsIncoming.Contains(this))
            {
                this.Destination.ConnectorsIncoming.Remove(this);
            }
        }

        public void Run(IWFActivityInstance activityInstance, string codeSuffix = "", bool force = false, IDictionary<string, object> args = null)
        {
            activityInstance.ProcessInstance.TransitionConnector = this;
            try
            {
                ConnectorBehaviour.Run(activityInstance, this, codeSuffix, force, args);
            }
            finally
            {
                activityInstance.ProcessInstance.TransitionConnector = null;
            }
        }

        public IWFActivityInstance FireSignal(IWFActivityInstance activityInstance, string codeSuffix = "", IDictionary<string, object> args = null)
        {
            return ConnectorBehaviour.FireSignal(activityInstance, this, codeSuffix, args);
        }

        public bool IsAvailable(IWFProcessInstance processInstance, IDictionary<string, object> args)
        {
            this.IsConnectorAvailable = ConnectorBehaviour.IsAvailable(processInstance, args);
            return this.IsConnectorAvailable;
        }

        internal void Validate(HashSet<WFActivityBase> visited)
        {
            try
            {
                if (this.Destination == null || this.Source == null)
                    throw new WFDesignException("Connector musi zawieraÊ element poczπtkowy i koÒcowy", null);

                var to = this.DialogBoxValueMaping.Select(x => x.To).GroupBy(x => x).Where(x => x.Count() > 1).FirstOrDefault();
                if (to != null)
                    throw new WFDesignException(String.Format("Do zmiennej {0} przypisano dwukrotnie wartoúÊ.", null), null);

                var parameter = this.DialogBoxParameters.Select(x => x.Name).GroupBy(x => x).Where(x => x.Count() > 1).FirstOrDefault();
                if (parameter != null)
                    throw new WFDesignException(String.Format("Parametr {0} wystÍpuje na liscie wielokrotnie.", null), null);

                this.Destination.StartValidate(visited);
            }
            catch
            {
                throw;
            }
        }

        public XElement WriteTemplateToXmlElement()
        {
            WFMetaDataReaderWriter writer = new WFMetaDataReaderWriter();

            return new XElement("Connector",
                                    new XAttribute("From", this.Source.Code ?? ""),
                                    new XAttribute("To", this.Destination.Code ?? ""),
                                    new XAttribute("Code", this.Code ?? ""),
                                    new XAttribute("DialogBoxName", this.DialogBoxName ?? ""),
                                    new XAttribute("SingleInstance", this.SingleInstance.ToString()),
                                    new XAttribute("ForceSingleInstance", this.SingleInstance.ToString()),
                                     new XAttribute("IsSystem", this.IsSystem.ToString()),
                                    new XAttribute("IsActiveWithoutTodos", this.IsActiveWithoutTodos.ToString()),
                                    new XAttribute("Caption", this.Caption ?? ""),
                                    new XAttribute("DesignerSettings", this.DesignerSettings.ToString() ?? ""),
                                    new XAttribute("ConditionScript", this.ConditionScript ?? ""),
                                    new XAttribute("PolicyScript", PolicyScript ?? ""),
                                    new XAttribute("ConnectorBehaviour", ConnectorBehaviour.GetType().GetShortName()),
                                    new XAttribute("DestinationSplitter", DestinationSplitter != null ? DestinationSplitter.GetType().GetShortName() : ""),
                                    new XAttribute("MainPath", this.MainPath),
                                    new XAttribute("Order", this.Order),
                                    new XAttribute("DeleteParentPath", this.DeleteParentPath),
                                    new XAttribute("ShowAsButton", this.ShowAsButton.ToString()),
                                    new XElement("Conditions", this.Conditions.Select(x => x.WriteTemplateToXmlElement())),
                                     new XElement("Policies", this.Policies.Select(x => x.WriteTemplateToXmlElement())),
                                     writer.WriteTemplateToXmlElement(this.MetaData),
                                    new XElement("DialogBoxValueMaping", this.DialogBoxValueMaping.Select(x => x.SaveToXmlElement())),
                                    new XElement("DialogBoxParameters", this.DialogBoxParameters.Select(x => x.SaveToXmlElement())));
        }

        public void ReadTemplateFromXmlElement(XElement element)
        {
            this.Code = element.Attribute("Code").Value;
            this.Caption = element.Attribute("Caption").Value;
            this.SingleInstance = element.GetAttributeBooleanValueOrDefault("SingleInstance");
            this.ForceSingleInstance = element.GetAttributeBooleanValueOrDefault("ForceSingleInstance");
            this.IsSystem = element.GetAttributeBooleanValueOrDefault("IsSystem");
            this.IsActiveWithoutTodos = element.GetAttributeBooleanValueOrDefault("IsActiveWithoutTodos");
            int? order = element.GetAttributeIntValueOrNull("Order");
            this.Order = order == null ? 10 : order.Value;
            int? deleteParentPath = element.GetAttributeIntValueOrNull("DeleteParentPath");
            this.DeleteParentPath = deleteParentPath == null ? 0 : deleteParentPath.Value;
            this.ShowAsButton = element.GetAttributeBooleanValueOrDefault("ShowAsButton");

            var dialogBoxAttribute = element.Attribute("DialogBoxName");
            this.DialogBoxName = dialogBoxAttribute != null ? dialogBoxAttribute.Value : "";

            var desingerSettingAtribute = element.Attribute("DesignerSettings");

            if (desingerSettingAtribute != null)
                this.DesignerSettings = (DesingerConnectorInfo)desingerSettingAtribute.Value;
            else
                this.DesignerSettings = new DesingerConnectorInfo();

            if (!string.IsNullOrWhiteSpace(element.Attribute("ConditionScript").Value))
                this.ConditionScript = element.Attribute("ConditionScript").Value;

            if (!string.IsNullOrWhiteSpace(element.Attribute("PolicyScript").Value))
                this.PolicyScript = element.Attribute("PolicyScript").Value;

            this.MainPath = element.GetAttributeBooleanValueOrDefault("MainPath");

            this.ConnectorBehaviour = (IWFConnectorBehaviour)Activator.CreateInstance(Type.GetType(element.Attribute("ConnectorBehaviour").Value));
            this.ConnectorBehaviour.SetConnector(this);

            this.DestinationSplitter = element.Attribute("DestinationSplitter") != null && !string.IsNullOrEmpty(element.Attribute("DestinationSplitter").Value) ? (IWFDestinationSplitter)Activator.CreateInstance(Type.GetType(element.Attribute("DestinationSplitter").Value)) : null;

            foreach (var condition in element.Element("Conditions").Elements("Condition"))
            {
                Type type = Type.GetType(condition.Attribute("Type").Value);
                IWFCondition con = (IWFCondition)Activator.CreateInstance(type);
                con.ReadTemplateFromXmlElement(condition);
                DesingerConditions.Add(new DesingerConditionItem() { Condition = con });
            }

            foreach (var policy in element.Element("Policies").Elements("PolicyRule"))
            {
                Type type = Type.GetType(policy.Attribute("Type").Value);
                IWFPolicyRule pol = (IWFPolicyRule)Activator.CreateInstance(type);
                pol.ReadTemplateFromXmlElement(policy);
                DesingerPolicies.Add(new DesingerPolicyItem() { Policy = pol });
            }

            this.MetaData = new WFMetaDataReaderWriter().ReadTemplateFromXmlElement(element);

            var dialogBoxValueMappingElement = element.Element("DialogBoxValueMaping");
            if (dialogBoxValueMappingElement != null)
                foreach (var item in dialogBoxValueMappingElement.Elements("Map"))
                {
                    var mappingItem = new MappingItem();
                    mappingItem.LoadFromXmlElement(item);
                    DialogBoxValueMaping.Add(mappingItem);
                }

            var dialogBoxParametersElement = element.Element("DialogBoxParameters");
            if(dialogBoxParametersElement != null)
                foreach (var item in dialogBoxParametersElement.Elements("Parameter"))
                {
                    var parameterItem = new ParameterItem();
                    parameterItem.LoadFromXmlElement(item);
                    DialogBoxParameters.Add(parameterItem);
                }
        }

        #region IWFConnector Members

        public bool WaitingToBeExecuted(IWFProcessInstance iWFProcessInstance, HashSet<string> checkedConnectors)
        {
            if (checkedConnectors.Contains(Code))
                return false;
            else
            {
                checkedConnectors.Add(Code);
                return ConnectorBehaviour.WaitingToBeExecuted(this, iWFProcessInstance, checkedConnectors);
            }
        }

        #endregion

        public object Clone()
        {
            return new WFConnector(WFConnectorBehaviourBase.Create(this.ConnectorBehaviourType))
            {
                Caption = this.Caption,
                Code = this.Code,
                ConditionScript = this.ConditionScript,
                ConnectorBehaviourType = this.ConnectorBehaviourType,
                DeleteParentPath = this.DeleteParentPath,
                DesingerConditions = new List<DesingerConditionItem>(this.DesingerConditions),
                DesingerPolicies = new List<DesingerPolicyItem>(this.DesingerPolicies),
                DesignerSettings = this.DesignerSettings,
                Destination = this.Destination,
                DestinationSplitterType = this.DestinationSplitterType,
                DialogBoxName = this.DialogBoxName,
                DialogBoxParameters = this.DialogBoxParameters, // TODO: new list
                DialogBoxValueMaping = this.DialogBoxValueMaping, // TODO: new list
                IsActiveWithoutTodos = this.IsActiveWithoutTodos,
                IsSystem = this.IsSystem,
                MainPath = this.MainPath,
                MetaData = this.MetaData, // TODO: new list
                Order = this.Order,
                PolicyScript = this.PolicyScript,
                SingleInstance = this.SingleInstance,
                ShowAsButton = this.ShowAsButton,
                Source = this.Source,
                ForceSingleInstance = this.ForceSingleInstance
            };
        }
    }
}