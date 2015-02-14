using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("Script", "script", "scriptsmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFScriptActivity : WFAutomaticActivity
    {
        [Editor(typeof(ScriptUIEditor), typeof(UITypeEditor))]
        [Description("Skrypt do wykonania.")]
        public string Script { get; set; }

        public override void ReadTemplateFromXmlElement(XElement element)
        {
            base.ReadTemplateFromXmlElement(element);

            var atribute = element.Attribute("Script");
            Script = atribute != null ? atribute.Value : string.Empty;
        }

        public override XElement WriteTemplateToXmlElement()
        {
            var el = base.WriteTemplateToXmlElement();
            el.Add(new XAttribute("Script", Script??""));
            return el;
        }

        protected override bool Execute(IWFActivityInstance instance)
        {
            ScriptHelper.Execute(Script, instance);
            return true;
        }

        protected override void Validate(HashSet<WFActivityBase> visited)
        {
            if (this.ConnectorsIncoming.Count == 0)
                throw new WFDesignException(string.Format("Krok {0} musi zawierać co najmniej jedno połączenie wchodzące", this.Code), Code);

            if (this.ConnectorsOutgoing.Count == 0)
                throw new WFDesignException(string.Format("Krok {0} musi zawierać co najmniej jedno połączenie wychodzące", this.Code), Code);
        }

        public override object Clone()
        {
            return new WFScriptActivity()
            {
                Caption = this.Caption,
                Code = this.Code,
                Decription = this.Decription,
                DesignerSettings = this.DesignerSettings,
                EndScript = this.EndScript,
                ExecuteScript = this.ExecuteScript,
                LongRunning = this.LongRunning,
                Script = this.Script,
                StartScript = this.StartScript
            };
        }
    }


}
