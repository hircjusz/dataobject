using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;
using System.ComponentModel;
using SoftwareMind.SimpleWorkflow.Designer;
using System.Drawing.Design;
using System.Xml.Linq;
using SoftwareMind.Scripts;
using log4net;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    /// <summary>
    /// Klasa reprezentująca element diagramu, od któego rozpocznie się wykonywanie workflow po zajściu pewnego zdarzenia.
    /// </summary>
    [Serializable]
    [NodeProperties("Handle event", "handleevent", "handleeventsmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFHandleEventActivity : WFActivityBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFHandleEventActivity));

        /// <summary>
        /// Skrypt który bedzie sprawdzać czy 'Event' jest dostępny
        /// </summary>
        [Category("Common")]
        [Description("Skrypt który bedzie sprawdzać czy zdarzenie jest dostępne.")]
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        public string ConditionScript { get; set; }

        //[Category("Common")]
        //[Description("Okresla czy zdarzenine powoduje ustawienie potomków w stan Initialized, tak aby mogły się wykonać ponownie.")]
        //[DefaultValue(false)]
        //public bool ResetDescentant { get; set; }

        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            if (this.ConnectorsOutgoing.Count > 1)
                throw new WFDesignException(string.Format("Krok {0} musi zawierać conajmniej jedno połączenie wychodzące", Code), Code);

            base.Validate(visited);
        }

        public override void ReadTemplateFromXmlElement(XElement element)
        {
            base.ReadTemplateFromXmlElement(element);

            var atribute = element.Attribute("ConditionScript");
            ConditionScript = atribute != null ? atribute.Value : string.Empty;

            //atribute = element.Attribute("ResetDescentant");
            //if (atribute != null)
            //    ResetDescentant = atribute.Value.ToLower() == "true";
        }

        public override XElement WriteTemplateToXmlElement()
        {
            var el = base.WriteTemplateToXmlElement();
            el.Add(new XAttribute("ConditionScript", ConditionScript ?? ""));
            //el.Add(new XAttribute("ResetDescentant", ResetDescentant));
            return el;
        }

        protected internal override void DoTransitions(IWFActivityInstance activityInstance)
        {
            if (IsAvailable(activityInstance.ProcessInstance))
                base.DoTransitions(activityInstance);
        }

        public virtual bool IsAvailable(IWFProcessInstance processInance)
        {
            if (!string.IsNullOrEmpty(this.ConditionScript))
            {
                IDictionary<string, object> arguments = processInance.GetVariables();
                arguments["instance"] = processInance;
                Script s = new Script(ConditionScript, arguments);
                return (bool)s.Execute() == true;
            }
            else
                return true;
        }

        internal virtual void Fire(IWFProcessInstance processInance, IDictionary<string, object> arguments, Action<IWFProcessInstance> onEachTransitionCompleted, Action<IWFActivityInstance> beforeCompleted = null)
        {
            IWFActivityInstance activityInstance = null;

            log.InfoFormat("Zastygnalizowano Event: {0} w wyniku którego uruchomiona zostanie aktywność: {1}", Caption, Code);

            if (IsAvailable(processInance))
            {
                if (processInance.ContainsActivity(Code) && processInance.GetActivity(Code).State != WFActivityState.Completed)
                    throw new InvalidOperationException("Akytwność jest w danej chwili wykonywana");
                else
                {
                    activityInstance = processInance.GetActivity(Code);
                    activityInstance.AddVariables(arguments);
                    activityInstance.Activity.Run(activityInstance/*, beforeCompleted*/);

                    if (onEachTransitionCompleted != null)
                        onEachTransitionCompleted(processInance);
                }
            }
            else
                throw new InvalidOperationException("Nie można wykonać Eventu, ponieważ nie jest on dostępny w chwili obecnej");
        }

        public override object Clone()
        {
            return new WFHandleEventActivity()
            {
                Caption = this.Caption,
                Code = this.Code,
                ConditionScript = this.ConditionScript,
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
