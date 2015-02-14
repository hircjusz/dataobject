using System;
using System.Collections.Generic;
using System.Linq;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;
using System.Xml.Linq;
using log4net;

namespace SoftwareMind.SimpleWorkflow.Activities
{
    [Serializable]
    [NodeProperties("Join", "join", "joinsmall", typeof(SoftwareMind.SimpleWorkflow.Properties.Resources))]
    public class WFJoin : WFActivityBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFJoin));

        public string MultiplicatorCode { get; set; }

        // nie chcemy zeby join automatycznie ruszył dalej
        // poniewaz w momeńcie aktywacji mogą być jeszcze kroki na które join powinien poczekać,
        // ale które nie są jeszcze zaktywowane
        public override bool AutoExecute(IWFActivityInstance instance)
        {
            return false;
        }

        private bool IsCompleted(IWFConnector connector, IWFActivityInstance instance, IWFActivityInstance activity)
        {
            var variables = new Dictionary<string, object>();
            switch (activity.State)
            {
                case WFActivityState.Active:
                case WFActivityState.Executed:
                case WFActivityState.Waiting:
                    if (connector.IsAvailable(instance.ProcessInstance, variables))
                        return false;
                    break;
            }
            return true;
        }

        protected override bool Execute(IWFActivityInstance instance)
        {
            // krok został zakończony wiec mozna założyć ze todo zostały zrobione

            foreach (var connector in this.ConnectorsIncoming)
            {
                // sytuacja z multiplikatorami powinna być obsłużona ponieważ instance.ProcessInstance.GetActivities
                // zwraca również zmultiplikowane aktywności
                string path = instance.GetParentPath();
                var incomingActivities = instance.ProcessInstance.GetActivities(connector.Source.Code);

                foreach(var activity in incomingActivities)
                {
                    bool cont = true;
                    if (!String.IsNullOrEmpty(path))
                    {
                        if (activity.GetParentPath().IndexOf(path) >= 0)
                            cont = true;
                        else
                            continue;
                    }

                    if (cont)
                    {
                        if (activity.Activity is WFActivityDemultiplicator)
                        {//sprawdzamy wszystkie aktywnosci ze sciezki demultiplikatora
                            string subpath = activity.Code;
                            subpath = subpath.Substring(subpath.IndexOf('|'));
                            subpath = subpath.Substring(subpath.IndexOf('#') + 1);
                            foreach (IWFActivityInstance subActivity in instance.ProcessInstance.GetActivities())
                            {
                                if (subActivity.Code.IndexOf(subpath) >= 0)
                                {
                                    cont = IsCompleted(connector, instance, subActivity);
                                    if (!cont)
                                        break;
                                }
                            }
                        }
                        else
                            cont = IsCompleted(connector, instance, activity);
                    }
                    if (!cont)
                        return false;
                }

            }

            return true;
        }

        private bool AreAllIncomingActivitiesCompleted(IWFActivityInstance instance)
        {
            HashSet<string> checkedConnectors = new HashSet<string>();
            var allCompleted = (from a in this.ConnectorsIncoming
                                where
                                a.WaitingToBeExecuted(instance.ProcessInstance, checkedConnectors) == true
                                select a.Source.Code).ToArray();

            return allCompleted.Count() == 0;
        }

        public override void ChangeState(IWFActivityInstance instance)
        {
            // nie aktywujemy aktywności jeśli jest konnektro dla 'scieżki głównej'
            // który nie został aktywowany
            if (instance.State == WFActivityState.Initialized)
            {
                if (instance.ProcessInstance.TransitionConnector == null)
                {
                    base.ChangeState(instance);
                    return;
                }

                bool isMainPath = instance.ProcessInstance.TransitionConnector.MainPath;
                if (!isMainPath)
                {
                    foreach (WFConnector connector in this.ConnectorsIncoming)
                        if (connector.MainPath)
                        {
                            isMainPath = true;
                            break;
                        }
                }
//                var mainPathConnector = (from conn in this.ConnectorsIncoming where conn.MainPath == true select conn).SingleOrDefault();

                if (isMainPath)
                {
//                    if (instance.ProcessInstance.TransitionConnector.Code == mainPathConnector.Code)
                    if (instance.ProcessInstance.TransitionConnector.MainPath) // dopuszczamy klika wejsc MainPath
                    {
                        base.ChangeState(instance);
                    }
                    else
                    {
                        // nic nie robimy
                        // ponieważ to nie jest kontektor główny
                    }
                }
                else
                {
                    // nie ma konektora dla sciżki głównej, więc zachowujemy się jak poprzednio
                    base.ChangeState(instance);
                }
            }
            else
            {
                base.ChangeState(instance);
            }
        }

        protected override void Validate(HashSet<WFActivityBase> visited)
        {
//            if (this.ConnectorsIncoming.Count <= 1)
//                throw new WFDesignException(string.Format("Krok {0} musi zawierać co najmniej dwa połączenia wchodzące", this.Code), Code);

            base.Validate(visited);
        }

        public override void ReadTemplateFromXmlElement(XElement element)
        {
            base.ReadTemplateFromXmlElement(element);

            var atribute = element.Attribute("MultiplicatorCode");
            MultiplicatorCode = atribute != null ? atribute.Value : string.Empty;
        }

        public override XElement WriteTemplateToXmlElement()
        {
            var el = base.WriteTemplateToXmlElement();
            el.Add(new XAttribute("MultiplicatorCode", MultiplicatorCode ?? ""));
            return el;
        }

        internal override bool WaitingToBeExecuted(IWFProcessInstance iWFProcessInstance, HashSet<string> checkedConnectors)
        {
            string path = null;

            if (!String.IsNullOrEmpty(MultiplicatorCode))
            {
                string parentPath = iWFProcessInstance.GetActivity(this.Code).GetParentPath();
                path = MultiplicatorCode;
                if (!String.IsNullOrEmpty(parentPath))
                    path = MultiplicatorCode + parentPath;


            }

            var completed = iWFProcessInstance.IsActivityCompleted(this.Code, path);

            if (completed) return false;

            var waitingToBeExecuted = (from a in this.ConnectorsIncoming
                                       where
                                       a.WaitingToBeExecuted(iWFProcessInstance, checkedConnectors) == true
                                       select a.Source.Code).ToArray();

            log.DebugFormat("WaitingToBeExecuted==> {0} oczekuje ? {1}", this.Code, waitingToBeExecuted.Count() > 0);
            return waitingToBeExecuted.Count() > 0;
        }

        public override object Clone()
        {
            return new WFJoin()
            {
                Caption = this.Caption,
                Code = this.Code,
                Decription = this.Decription,
                DesignerSettings = this.DesignerSettings,
                EndScript = this.EndScript,
                ExecuteScript = this.ExecuteScript,
                LongRunning = this.LongRunning,
                MultiplicatorCode = this.MultiplicatorCode,
                StartScript = this.StartScript
            };
        }
    }
}
