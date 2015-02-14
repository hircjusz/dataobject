using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Design;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Actions;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.SimpleWorkflow.Exceptions;


namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    public class WFManualActivity : WFActivityBase
    {
        #region Designer Properties

        [Editor(typeof (WfActivityActionContainerCollectionEditor), typeof (UITypeEditor))]
        [Category("Zdarzenia")]
        [Description("Obsługa wejścia do kroku")]
        public List<WfActivityActionContainer> ActivateTaskActions { get; set; }

        [Editor(typeof (WfActivityActionContainerCollectionEditor), typeof (UITypeEditor))]
        [Category("Zdarzenia")]
        [Description("Obsługa odpięcia użytkownika od kroku")]
        public List<WfActivityActionContainer> UnassignOwnerActions { get; set; }

        [Editor(typeof (WfActivityActionContainerCollectionEditor), typeof (UITypeEditor))]
        [Category("Zdarzenia")]
        [Description("Obsługa przypięcia użytkownika do kroku")]
        public List<WfActivityActionContainer> AssignOwnerActions { get; set; }

        [Editor(typeof (WfActivityActionContainerCollectionEditor), typeof (UITypeEditor))]
        [Category("Zdarzenia")]
        [Description("Obsługa zakończenia zadania")]
        public List<WfActivityActionContainer> CompleteTaskActions { get; set; }

        #endregion

        protected bool ValidateOutgoing { get; set; }

        public WFManualActivity()
        {
            ValidateOutgoing = true;
            ActivateTaskActions = new List<WfActivityActionContainer>();
            UnassignOwnerActions = new List<WfActivityActionContainer>();
            AssignOwnerActions = new List<WfActivityActionContainer>();
            CompleteTaskActions = new List<WfActivityActionContainer>();
        }

        protected override bool Execute(IWFActivityInstance instance)
        {
            return false;
        }

        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            if (this.ValidateOutgoing && this.ConnectorsOutgoing.Count < 1)
                throw new WFDesignException(
                    string.Format("Krok {0} musi zawierać co najmniej jedno połączenie wychodzące", this.Code), Code);

            ValidateActions(ActivateTaskActions, "ActivateTaskActions");
            ValidateActions(UnassignOwnerActions, "UnassignOwnerActions");
            ValidateActions(AssignOwnerActions, "AssignOwnerActions");
            ValidateActions(CompleteTaskActions, "CompleteTaskActions");

            base.Validate(visited);
        }

        public override XElement WriteTemplateToXmlElement()
        {
            var result = base.WriteTemplateToXmlElement();

            WriteActionsTemplateToXmlElement(result, ActivateTaskActions, "ActivateTaskActions");
            WriteActionsTemplateToXmlElement(result, UnassignOwnerActions, "UnassignOwnerActions");
            WriteActionsTemplateToXmlElement(result, AssignOwnerActions, "AssignOwnerActions");
            WriteActionsTemplateToXmlElement(result, CompleteTaskActions, "CompleteTaskActions");

            return result;
        }

        public override void ReadTemplateFromXmlElement(XElement element)
        {
            base.ReadTemplateFromXmlElement(element);

            ActivateTaskActions = ReadActionsTemplateFromXmlElement(element, "ActivateTaskActions");
            UnassignOwnerActions = ReadActionsTemplateFromXmlElement(element, "UnassignOwnerActions");
            AssignOwnerActions = ReadActionsTemplateFromXmlElement(element, "AssignOwnerActions");
            CompleteTaskActions = ReadActionsTemplateFromXmlElement(element, "CompleteTaskActions");
        }

        public override object Clone()
        {
            return new WFManualActivity()
            {
                Caption = this.Caption,
                Code = this.Code,
                Decription = this.Decription,
                DesignerSettings = this.DesignerSettings,
                EndScript = this.EndScript,
                ExecuteScript = this.ExecuteScript,
                LongRunning = this.LongRunning,
                StartScript = this.StartScript,
                ActivateTaskActions = new List<WfActivityActionContainer>(ActivateTaskActions),
                UnassignOwnerActions = new List<WfActivityActionContainer>(UnassignOwnerActions),
                AssignOwnerActions = new List<WfActivityActionContainer>(UnassignOwnerActions),
                CompleteTaskActions = new List<WfActivityActionContainer>(UnassignOwnerActions)
            };
        }

        #region Actions

        private void WriteActionsTemplateToXmlElement(XElement root, List<WfActivityActionContainer> actions, string name)
        {
            if (actions.Count > 0)
            {
                var result = new XElement(name);
                foreach (var action in actions)
                {
                    result.Add(action.WriteTemplateToXmlElement());
                }
                root.Add(result);
            }
        }

        private List<WfActivityActionContainer> ReadActionsTemplateFromXmlElement(XElement element, string name)
        {
            var result = new List<WfActivityActionContainer>();
            var collectionElement = element.Element(name);
            if (collectionElement != null)
            {
                foreach (var childElement in collectionElement.Elements(WfActivityActionContainer.TemplateName))
                {
                    var action = new WfActivityActionContainer();
                    action.ReadTemplateFromXmlElement(childElement);
                    result.Add(action);
                }
            }
            return result;
        }

        private void ValidateActions(List<WfActivityActionContainer> actions, string name)
        {
            if (actions != null)
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    var action = actions[i];
                    try
                    {
                        action.Validate();
                    }
                    catch (WFDesignException exc)
                    {
                        var message = string.Format("{0}[{1}. {2}]: {3}", name, i, action.Name, exc.Message);
                        throw new WFDesignException(string.Format("Błąd w kroku {0}. {1}", Code, message), Code);
                    }
                }
            }
        }

        #endregion
    }
}
