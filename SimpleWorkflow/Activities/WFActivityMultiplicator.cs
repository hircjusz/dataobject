using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;
using System.Xml.Linq;
using System;
using log4net;


namespace SoftwareMind.SimpleWorkflow.Activities
{
    [NodeProperties("Multiplied activity", "multi", "multismall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    [Serializable]
    public class WFActivityMultiplicator : WFActivityBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFActivityMultiplicator));

        private Type _multiplicator;
        private IWFWorkSplitter _multiplicatorInstance;

        [TypeConverter(typeof(TypeSelector))]
        [TypeSelectorInterfaceTypeAtribute(typeof(IWFWorkSplitter))]
        [DefaultValue(null)]
        public Type Multiplicator
        {
            get { return this._multiplicator; }
            set
            {
                this._multiplicator = value;
                this._multiplicatorInstance = value == null ? null : (IWFWorkSplitter) Activator.CreateInstance(value);
            }
        }

        [Browsable(false)]
        public IWFWorkSplitter MultiplicatorInstance
        {
            get { return this._multiplicatorInstance; }
            set
            {
                this._multiplicator = value == null ? null : value.GetType();
                this._multiplicatorInstance = value;
            }
        }

        protected internal override void DoTransitions(IWFActivityInstance activityInstance)
        {
        }

        protected override bool Execute(IWFActivityInstance activityInstance)
        {
            IDictionary<string, object>[] arguments = MultiplicatorInstance.GetInputData(activityInstance);

            int copyNo = 0;

            log.Debug("Muliplikowanie aktywności");

            foreach (IDictionary<string, object> args in arguments)
            {
                foreach (WFConnector connector in this.ConnectorsOutgoing)
                {
                    string suffix = "";
                    do
                    {
                        copyNo++;
                        suffix = "|" + copyNo + "#" + Code;
                    } while (activityInstance.ProcessInstance.ContainsActivity(connector.Destination.Code + suffix));
                    log.DebugFormat("Muliplikowanie aktywności. Uruchamianie aktywności o kodzie {0} i suffixie {1}", Code, suffix);
                    connector.Run(activityInstance, suffix, false, args);
                    copyNo++;
                }
            }
            return true;
        }

        protected override void Validate(HashSet<WFActivityBase> validated)
        {
            if(this.ConnectorsOutgoing.Count > 1)
                throw new WFDesignException("WFActivityMultiplicator może zawierać tylko jedno połączenie wychodzące", null);
            base.Validate(validated);
        }

        public override void ReadTemplateFromXmlElement(XElement element)
        {
            base.ReadTemplateFromXmlElement(element);
            XAttribute attribute = element.Attribute("Multiplicator");

            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Value))
            {
                this.Multiplicator = Type.GetType(attribute.Value);
                //this.Multiplicator = (IWFWorkSplitter)Activator.CreateInstance(t);
            }
        }

        public override XElement WriteTemplateToXmlElement()
        {
            XElement element = base.WriteTemplateToXmlElement();

            string multiplicatorName = this.Multiplicator != null ? this.Multiplicator.GetShortName() : "";
            element.Add(new XAttribute("Multiplicator", multiplicatorName));

            return element;
        }

        public override object Clone()
        {
            return new WFActivityMultiplicator()
            {
                Caption = this.Caption,
                Code = this.Code,
                Decription = this.Decription,
                DesignerSettings = this.DesignerSettings,
                EndScript = this.EndScript,
                ExecuteScript = this.ExecuteScript,
                LongRunning = this.LongRunning,
                StartScript = this.StartScript
            };
        }
    }
}
