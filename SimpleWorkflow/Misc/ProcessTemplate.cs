using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Activities;
using SoftwareMind.Utils.Extensions.Xml;
using log4net;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    [Serializable]
    internal class ProcessTemplate
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessTemplate));

        private static FileSystemWatcher fsw;

        private static void InitFileSystemWatcher()
        {
            string path = GetTemplateDirPath();
            if (Directory.Exists(path))
            {
                fsw = new FileSystemWatcher(path, "*.xml");
                fsw.Changed += TemplateChanged;
                fsw.Deleted += TemplateChanged;
                fsw.Created += TemplateChanged;
                fsw.Renamed += TemplateChanged;
                fsw.EnableRaisingEvents = true;
            }
        }

        private static void TemplateChanged(object sender, FileSystemEventArgs e)
        {
            WFProcessFactory.ClearCache();
        }

        private static string GetTemplateDirPath()
        {
            string templateDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ResourcesDir"]);
            templateDirPath = Path.Combine(templateDirPath, ConfigurationManager.AppSettings["WorkflowTemplatesDir"]);
            return templateDirPath;
        }

        public static void Save(string filename, WFProcess process)
        {
            File.WriteAllText(filename, SaveToXml(process));
        }

        public static string SaveToXml(WFProcess process)
        {
            WFStartActivity startAvtivity = process.RootActivity;
            var activitiesXml = from WFActivityBase activity in process.GetAllActivities()
                                select activity.WriteTemplateToXmlElement();

            var connectors = process.GetAllActivities().SelectMany(ele => ele.ConnectorsOutgoing.ToArray()).Distinct();
            var connectorsXml = from WFConnector con in connectors select con.WriteTemplateToXmlElement();

            XElement root = new XElement("Template",
                new XAttribute("Name", process.Name ?? "brak nazwy"),
                new XAttribute("DesingerSettings", process.DesingerGlobalSettings.ToString()),
                new XAttribute("OnVariableChanged", process.OnVariableChanged ?? ""),
                new XElement("Activities", activitiesXml),
                new XElement("Connectors", connectorsXml),
                process.VariableDefs.Select(x => x.Value.WriteTemplateToXmlElement()));

            return root.ToString();
        }

        public static string GetTemplatePathByName(string name)
        {
            string path = GetTemplateDirPath();
            log.DebugFormat("GetTemplatePathByName: {0} File: {1}.xml", path, name);
            string templatePath = Path.Combine(path, name + ".xml");
            return templatePath;
        }

        public static WFProcess Load(string filename, bool substituteTemplate)
        {
            string templateString = File.ReadAllText(filename);
            return LoadFromXml(templateString, substituteTemplate);
        }

        public static WFProcess LoadFromXml(string templateString, bool substituteTemplate = true)
        {
            if (fsw == null)
                InitFileSystemWatcher();

            WFProcess process = new WFProcess();

            log.Debug("Wczytywanie szablonu..");
            List<WFActivityBase> activitiesList = new List<WFActivityBase>();

            XDocument template = XDocument.Parse(templateString);
            RestoreActivities(activitiesList, template);
            RestoreConnections(activitiesList, template);
            RestoreVariableDefinition(process, template);

            process.Name = template.Element("Template").Attribute("Name").Value;
            process.OnVariableChanged = template.Element("Template").GetAttributeStringValueOrNull("OnVariableChanged");

            process.RootActivity = (WFStartActivity)(from activity in activitiesList where activity.GetType() == typeof(WFStartActivity) select activity).Single();

            foreach (WFActivityBase activity in activitiesList)
                process.AddActivity(activity, substituteTemplate);

            var desingerSettingsAtribute = template.Descendants("Template").First().Attribute("DesingerSettings");
            if (desingerSettingsAtribute != null)
                process.DesingerGlobalSettings = (DesignerGlobalInfo)desingerSettingsAtribute.Value;
            else
                process.DesingerGlobalSettings = new DesignerGlobalInfo();

            log.Debug("Wczytywanie szablonu, zakończone");
            return process;
        }


        private static void RestoreVariableDefinition(WFProcess process, XDocument template)
        {
            var nodes = template.Descendants("VariableDef").Where(x=> x.Parent == template.Nodes().First());
            foreach (XElement e in nodes)
            {
                WFVariableDef def = new WFVariableDef();
                def.ReadTemplateFromXmlElement(e);
                process.VariableDefs.Add(def);
            }
        }

        private static void RestoreActivities(List<WFActivityBase> activitiesList, XDocument template)
        {
            int noActivities = 0;

            var activitiesXml = from act in template.Descendants("Activities").Elements()
                                select act;

            log.Debug("Wczytywanie aktywności");
            foreach (XElement e in activitiesXml)
            {
                string typeName = e.Attribute("Type").Value;

                Type type = Type.GetType(typeName);
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                WFActivityBase a = (WFActivityBase)constructor.Invoke(null);
                a.ReadTemplateFromXmlElement(e);
                activitiesList.Add(a);
                noActivities++;
            }

            log.DebugFormat("Wczytano {0} aktywności", noActivities);
        }

        private static void RestoreConnections(List<WFActivityBase> activitiesList, XDocument template)
        {
            int noConnectors = 0;

            var connectorsXml = from connNode in template.Descendants("Connectors").Elements()
                                select connNode;

            log.Debug("Wczytywanie połączeń");
            foreach (XElement connNode in connectorsXml)
            {
                string fromCode = connNode.Attribute("From").Value;
                string toCode = connNode.Attribute("To").Value;



                var fromActivity = (from activity in activitiesList
                                    where activity.Code == fromCode
                                    select activity).SingleOrDefault();

                var toActivity = (from activity in activitiesList
                                  where activity.Code == toCode
                                  select activity).SingleOrDefault();

                IWFConnector connector = WFConnectionHelper.JoinActivities(fromActivity, toActivity);
                connector.ReadTemplateFromXmlElement(connNode);

                RestoreConditions(connector, connNode);
                noConnectors++;
            }

            log.DebugFormat("Wczytano {0} połączeń", noConnectors);
        }

        private static void RestoreConditions(IWFConnector connector, XElement connectorNode)
        {
            List<IWFCondition> list = new List<IWFCondition>();
            XElement conditionsNode = connectorNode.Element("Conditions");

            if (conditionsNode != null)
            {
                foreach (XElement condition in conditionsNode.Elements("Condition"))
                {
                    Type type = Type.GetType(condition.Attribute("Type").Value);
                    IWFCondition c = (IWFCondition)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    c.ReadTemplateFromXmlElement(condition);
                    list.Add(c);
                }
            }
            connector.Conditions = list;
        }
    }
}
