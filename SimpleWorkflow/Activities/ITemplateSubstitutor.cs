using System;
using System.ComponentModel.Composition;
using System.Linq;
using SoftwareMind.SimpleWorkflow.Behaviours;
using SoftwareMind.Utils.Exceptions;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    public interface IWFTemplateSubstitutor
    {
        void SubstituteTemplate(WFProcess process, WFTemplateActivity activity);
    }

    [Export(typeof(IWFTemplateSubstitutor))]
    public class WFTemplateSubstitutor : IWFTemplateSubstitutor
    {
        private const string SubprocessNamespace = "Subprocess";
        private const string PrefixTemplate = "{0}$";

        private const string TemplateStartActivityCode = "TPL_START";
        private const string TemplateStartActivityCaption = "template start";
        private const string TemplateStartConnectionCode = "TPL_START_CONN";

        private const string TemplateEndActivityCode = "TPL_END";
        private const string TemplateEndActivityCaption = "template end";
        private const string TemplateEndConnectionCode = "TPL_END_CONN";

        public void SubstituteTemplate(WFProcess process, WFTemplateActivity activity)
        {
            this.ValidateTemplate(activity);

            WFConnector incomingConnector = (WFConnector)activity.ConnectorsIncoming.Single();
            WFConnector outgoingConnector = (WFConnector)activity.ConnectorsOutgoing.Single();

            WFProcess template = (WFProcess)WFProcessFactory.GetProcess(activity.TemplateName, SubprocessNamespace).Clone();
            string prefix = string.Format(PrefixTemplate, activity.Code);

            // Add prefix & Move activities to process
            foreach (var a in template.GetAllActivities().ToArray())
            {
                a.Code = prefix + a.Code;
                foreach (var c in a.ConnectorsOutgoing)
                {
                    c.Code = prefix + c.Code;
                }

                process.AddActivity(a);
            }

            string varDefTemplatePrefix = prefix.Replace("$", "_");
            if (template.VariableDefs != null)
            {
                foreach (var vdef in template.VariableDefs)
                {
                    process.VariableDefs.Add(new WFVariableDef(varDefTemplatePrefix + vdef.Key, vdef.Value.Type));
                }
            }

            // Create multiplicator
            var multiplicator = new WFDummyActivity()
            {
                Caption = activity.Caption + " " + TemplateStartActivityCaption,
                Code = prefix + TemplateStartActivityCode,
                Decription = activity.Decription,
                LongRunning = false,
                ExecuteScript = activity.ExecuteScript
            };

            process.AddActivity(multiplicator);
            incomingConnector.Destination = multiplicator;

            var demultiplicator = new WFDummyActivity()
            {
                Caption = activity.Caption + " " + TemplateEndActivityCaption,
                Code = prefix + TemplateEndActivityCode,
                Decription = activity.Decription,
                LongRunning = false
            };
            process.AddActivity(demultiplicator);
            outgoingConnector.Source = demultiplicator;

            // Connect multiplicator to template root
            var startActivity = template.RootActivity;
            var templateStartActivity = new WFTemplateStartActivity(startActivity);

            foreach (var connector in template.RootActivity.ConnectorsOutgoing.ToArray())
            {
                connector.Source = templateStartActivity;
            }
            process.RemoveActivity(startActivity);
            process.AddActivity(templateStartActivity);
            WFConnectionHelper.JoinActivities(multiplicator, templateStartActivity, WFConnectorBehaviourType.Standard, prefix + TemplateStartConnectionCode);

            foreach (var endActivity in template.GetAllActivities().OfType<WFEndActivity>().ToArray())
            {
                // replace WFEndActivity with WFTemplateEndActivity
                var templateEndActivity = new WFTemplateEndActivity(endActivity);
                foreach (var connector in endActivity.ConnectorsIncoming.ToArray())
                {
                    connector.Destination = templateEndActivity;
                }
                process.RemoveActivity(endActivity);
                process.AddActivity(templateEndActivity);
                WFConnectionHelper.JoinActivities(templateEndActivity, demultiplicator, WFConnectorBehaviourType.Standard, prefix + TemplateEndConnectionCode);
            }
        }

        private void ValidateTemplate(WFTemplateActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException("activity");
            }

            if (activity.ConnectorsIncoming.Count != 1)
            {
                throw new ValidationException("Template must have only one incoming connector");
            }

            if (activity.ConnectorsOutgoing.Count != 1)
            {
                throw new ValidationException("Template must have only one outgoing connector");
            }

            if (!string.IsNullOrEmpty(activity.EndScript))
            {
                throw new ValidationException("Template EndScript must be empty");
            }

            //if (!string.IsNullOrEmpty(activity.ExecuteScript))
            //{
            //    throw new ValidationException("Template ExecuteScript must be empty");
            //}

            if (!string.IsNullOrEmpty(activity.StartScript))
            {
                throw new ValidationException("Template StartScript must be empty");
            }
        }
    }
}