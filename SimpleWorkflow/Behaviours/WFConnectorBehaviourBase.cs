using System;
using System.Collections.Generic;
using System.Linq;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.Scripts;
using SoftwareMind.SimpleWorkflow.Misc;
using SoftwareMind.Utils.Extensions;
using log4net;

namespace SoftwareMind.SimpleWorkflow.Behaviours
{
    [Serializable]
    public abstract class WFConnectorBehaviourBase : IWFConnectorBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFConnectorBehaviourBase));

        private static Dictionary<WFConnectorBehaviourType, Type> cache = CreateCacheDictionary();

        protected IWFConnector Connector { get; set; }

        public static IWFConnectorBehaviour Create(WFConnectorBehaviourType behaviourType)
        {
            Type type;
            if (!cache.TryGetValue(behaviourType, out type))
                throw new ArgumentException(String.Format("Nie istnieje typ przypisany do {0}", behaviourType));
            return (IWFConnectorBehaviour)Activator.CreateInstance(type);
        }

        public static WFConnectorBehaviourType GetBehaviourType(IWFConnectorBehaviour connectorBehaviour)
        {
            return cache.Where(x => x.Value == connectorBehaviour.GetType()).Select(x => x.Key).Single();
        }

        public virtual bool ActivityCheck(IWFProcessInstance processInstance)
        {
            if (Connector.SingleInstance)
            {
                var activites = processInstance.GetActivities(Connector.Destination.Code).ToArray();
                if (activites.Length > 0)
                {
                    IWFActivityInstance activity = activites.Where(x => x.State != WFActivityState.Completed && x.State != WFActivityState.Initialized).FirstOrDefault();
                    if (activity != null)
                    {
//                        Logger.Log.Debug("Konektor pomiędzy {0} {1} jest niedostępny", Connector.Source.Code, Connector.Destination.Code);
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual bool IsAvailable(IWFProcessInstance processInstance, IDictionary<string, object> args)
        {
            ValidateState();

            if (!ActivityCheck(processInstance))
                return false;

            bool result = true;
            IDictionary<string, object> arguments = null;
            if (!String.IsNullOrEmpty(Connector.ConditionScript))
            {
                arguments = processInstance.GetVariables();
                arguments.AddNotExistentEntries(args);
                arguments.Add("instance", processInstance);
                if (!arguments.ContainsKey("connectorCode"))
                    arguments.Add("connectorCode", Connector.Code);
                if (!arguments.ContainsKey("connectorSourceCode"))
                    arguments.Add("connectorSourceCode", Connector.Source.Code);
                if (!arguments.ContainsKey("connectorDestinationCode"))
                    arguments.Add("connectorDestinationCode", Connector.Destination.Code);
                IScript script = new Script(Connector.ConditionScript, arguments);
                object res = script.Execute();
                if (res is bool)
                {
                    result = (bool)res;
                }
                else
                    throw new ApplicationException("Invalid script expression result type");
            }

            if (result && Connector.Conditions.Count > 0)
            {
                if (arguments == null)
                {
                    arguments = processInstance.GetVariables();
                    arguments.AddNotExistentEntries(args);
                }
                else
                {
                    arguments = args;
                }

                result = Connector.Conditions.All(con => con.Eval(arguments));
            }

            // podczas testów Source i Destination moze być null
//            if(Connector.Source != null && Connector.Destination != null)
//            Logger.Log.Debug("Konektor pomiędzy {0} {1} jest dostepny? {2}", Connector.Source.Code, Connector.Destination.Code, result.ToString());

            return result;
        }

        public abstract void Run(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, bool force, IDictionary<string, object> args);

        public virtual void SetConnector(IWFConnector connector)
        {
            Connector = connector;
        }

        public virtual void Validate(WFActivityBase source, WFActivityBase destination)
        {
            ValidateState();
            /*
            var existsInSource = from WFConnector c in source.ConnectorsOutgoing
                                 where c.Source == source && c.Destination == destination && c.ConnectorBehaviour.GetType() == this.GetType()
                                 select c;

            if (existsInSource.Count() > 0)
                throw new InvalidOperationException("Połączenie pomiędzy wskazanymi elementami zostało już zdefiniowane");

            var existsInDestination = from WFConnector c in source.ConnectorsOutgoing
                                      where c.Source == source && c.Destination == destination && c.ConnectorBehaviour.GetType() == this.GetType()
                                      select c;

            if (existsInDestination.Count() > 0)
                throw new InvalidOperationException("Połączenie pomiędzy wskazanymi elementami zostało już zdefiniowane");
             */
        }

        protected virtual void ValidateState()
        {
            if (Connector == null)
                throw new InvalidOperationException("Proszę skonfigurować zachowanie za pomocą SetConnector()");
        }

        private static Dictionary<WFConnectorBehaviourType, Type> CreateCacheDictionary()
        {
            try
            {
                return TypeHelper.GetAllTypes()
                    .Where(x => x.GetCustomAttributes(typeof(BehaviourInfoAttribute), false).Length != 0)
                    .ToDictionary(x => (x.GetCustomAttributes(typeof(BehaviourInfoAttribute), false).First() as BehaviourInfoAttribute).BehaviourType, x => x);
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                log.Debug("Load Exceptions", ex);
                foreach (var e in ex.LoaderExceptions)
                {
                    log.Error("load exception:", e);
                }
                throw;
            }
        }

        public virtual bool WaitingToBeExecuted(WFConnector wFConnector, IWFProcessInstance iWFProcessInstance, HashSet<string> checkedConnectors)
        {
            return wFConnector.Source.WaitingToBeExecuted(iWFProcessInstance, checkedConnectors);
        }


        public virtual IWFActivityInstance FireSignal(IWFActivityInstance activityInstance, WFConnector connector, string codeSuffix, IDictionary<string, object> args)
        {
            throw new InvalidOperationException("Nie można wywołąć tej operacji na tego typu konnektorze");
        }

    }
}
