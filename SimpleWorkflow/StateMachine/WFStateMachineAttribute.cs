using System;
using System.IO;
using System.Linq;
using System.Configuration;

namespace SoftwareMind.SimpleWorkflow.StateMachine
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    [Serializable]
    public class WFStateMachineAttribute : Attribute
    {
        public string TemplatePath { get; private set; }

        public WFStateMachineAttribute(string templatePath)
        {
            if (String.IsNullOrEmpty(templatePath))
                throw new ArgumentException("Ścieżka szblonu nie moze być pusta.");

            TemplatePath = "";
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["WorkflowTemplatesDir"]))
                TemplatePath = Path.Combine(ConfigurationManager.AppSettings["WorkflowTemplatesDir"], TemplatePath);
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ResourcesDir"]))
                TemplatePath = Path.Combine(ConfigurationManager.AppSettings["ResourcesDir"], TemplatePath);

            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplatePath)))
            {
                TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplatePath, templatePath);
            }
            else if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), TemplatePath)))
            {
                TemplatePath = Path.Combine(Directory.GetCurrentDirectory(), TemplatePath, templatePath);
            }
            else if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "..", TemplatePath)))
            {
                TemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "..", TemplatePath, templatePath);
            }
            else if (Directory.Exists(Path.Combine(@"..\..\..\..\..\Presentation\Common\MVApplication", TemplatePath))) // check some other known paths
            {
                TemplatePath = Path.Combine(@"..\..\..\..\..\Presentation\Common\MVApplication", TemplatePath, templatePath);
            }
        }
    }
}
