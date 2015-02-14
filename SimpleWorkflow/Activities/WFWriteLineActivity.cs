using System;
using System.ComponentModel;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Misc;
using System.Collections.Generic;
using log4net;


namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("Write line", "writeline", "writelinesmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFWriteLineActivity : WFAutomaticActivity
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFWriteLineActivity));

        [Description("Komunikat do wypisania.")]
        public String Message { get; set; }

        protected override bool Execute(IWFActivityInstance instance)
        {
            string subMessage = "";
            IDictionary<string, object> vars = this.GetParentVariables(instance);
            if (vars != null)
                if (vars.ContainsKey("message"))
                    subMessage = (string)vars["message"];
            Console.WriteLine(Message + subMessage);
            log.Debug(Message + subMessage);
            return true;
        }

        public override void ReadTemplateFromXmlElement(XElement element)
        {
            base.ReadTemplateFromXmlElement(element);

            var atribute = element.Attribute("Message");
            Message = atribute != null ? atribute.Value : string.Empty;
        }

        public override XElement WriteTemplateToXmlElement()
        {
            var el = base.WriteTemplateToXmlElement();
            el.Add(new XAttribute("Message", Message ?? ""));
            return el;
        }

        public override object Clone()
        {
            return new WFWriteLineActivity()
            {
                Caption = this.Caption,
                Code = this.Code,
                Decription = this.Decription,
                DesignerSettings = this.DesignerSettings,
                EndScript = this.EndScript,
                ExecuteScript = this.ExecuteScript,
                LongRunning = this.LongRunning,
                Message = this.Message,
                StartScript = this.StartScript
            };
        }
    }
}
