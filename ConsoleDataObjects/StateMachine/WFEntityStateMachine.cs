using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleDataObjects.ObjectsDomain;
using DataObjects.NET;
using log4net;
using SoftwareMind.SimpleWorkflow;
using SoftwareMind.SimpleWorkflow.StateMachine;

namespace ConsoleDataObjects.StateMachine
{
    [Serializable]
    public class WFEntityStateMachine
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFEntityStateMachine));

        #region Static
        private static Dictionary<string, WFEntityStateMachine> machines = new Dictionary<string, WFEntityStateMachine>();
        private static ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

        public static WFEntityStateMachine GetStateMachine<T>(T entity, string fieldName, string sufix = null) where T : BusinessObject, IWFStateMachineHolder
        {
            string key = entity.GetType().ToString() + "::" + fieldName;
            if (sufix != null)
                key += "::" + sufix;
            WFEntityStateMachine result = null;
            readerWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                if (!machines.ContainsKey(key))
                {
                    readerWriterLockSlim.EnterWriteLock();
                    try
                    {
                        if (!machines.ContainsKey(key))
                            machines.Add(key, CreateStateMachine(entity, fieldName, sufix));
                    }
                    finally
                    {
                        readerWriterLockSlim.ExitWriteLock();
                    }
                }
            }
            finally
            {
                if (machines.ContainsKey(key))
                    result = machines[key];
                readerWriterLockSlim.ExitUpgradeableReadLock();
            }

            return result;
        }

        private static WFEntityStateMachine CreateStateMachine<T>(T entity, string fieldName, string variant) where T : BusinessObject, IWFStateMachineHolder
        {
            WFStateMachineAttribute attribute = GetStateMachinesAttribute(typeof(T), fieldName);
            IWFStateMachineHolder stateMachineHolder = (IWFStateMachineHolder)entity;
            string newPath = stateMachineHolder.GetStateMachinePath(attribute.TemplatePath, fieldName);
            newPath = newPath.Replace("{EntityName}", entity.Type.ShortName);
            if (pathResolver != null)
                newPath = pathResolver(newPath);
            log.Debug("Reading: " + newPath);
            string text = File.ReadAllText(File.Exists(newPath) ? newPath : attribute.TemplatePath);
            WFStateMachine machine = WFStateMachine.LoadMachineFromXml(text);
            var result = new WFEntityStateMachine(machine, entity.GetType(), fieldName, variant);
            return result;
        }

        internal static Func<string, string> pathResolver;

        private static WFStateMachineAttribute GetStateMachinesAttribute(Type type, string fieldName)
        {
            PropertyInfo property = type.GetProperty(fieldName);
            if (property == null)
                throw new ArgumentException("Invalid fieldName");

            object[] attributes = property.GetCustomAttributes(typeof(WFStateMachineAttribute), false);
            if (attributes == null || attributes.Length != 1)
                throw new ArgumentException("Invalid fieldName");
            return (WFStateMachineAttribute)attributes[0];
        }

        #endregion

        protected WFStateMachine Machine { get; set; }
        public Type EntityType { get; protected set; }
        public string FieldName { get; protected set; }
        public string Variant { get; protected set; }

        internal WFEntityStateMachine(WFStateMachine machine, Type entityType, string fieldName, string variant)
        {
            this.Machine = machine;
            this.EntityType = entityType;
            this.FieldName = fieldName;
            this.Variant = variant;
        }

        private string GetStateValue(BusinessObject entity)
        {
            object v = entity.GetProperty(this.FieldName);
            if (v is bool)
                return (bool)v ? "T" : "F";
            return (string)v;
        }

        /// <summary>
        /// Zwraca możliwe przejścia maszyny stanów dla podanej encji
        /// </summary>
        /// <param name="fieldName">Encja</param>
        /// <returns>Możliwe przejścia maszyny stanów dla podanej kolumny</returns>
        public IEnumerable<IWFConnector> GetAvailableTransitions(Session session, BusinessObject entity, Dictionary<string, object> variables = null)
        {
            if (!this.EntityType.Equals(entity.GetType()))
                throw new ArgumentException("Illegal entity");
            string state = GetActualState(entity);
            IWFStateMachineHolder stateMachine = (IWFStateMachineHolder)entity;

            if (variables == null)
                variables = new Dictionary<string, object>();
            variables["entity"] = entity;
            //            variables["root"] = stateMachine.Process; //maszyna stanów dotyczy encji które nie musza miec procesu
            variables["session"] = session;

            variables = stateMachine.PrepareVariables(session, this, variables);
            return this.Machine.GetAvailableTransitions(state, variables);
        }

        /// <summary>
        /// Zwraca aktualny stan maszyny.
        /// </summary>
        /// <param name="entity">Encja.</param>
        /// <returns>Aktualny stan maszyny.</returns>
        public string GetActualState(BusinessObject entity)
        {
            return GetStateValue(entity) ?? this.Machine.GetFirstStateCode();
        }

        /// <summary>
        /// Wykonuje przejście maszyny stnaów dla podanej encji i podanego konnektora
        /// </summary>
        /// <param name="fieldName">Encja</param>
        /// <param name="connector">Konnektor</param>
        public bool DoTransition(Session session, BusinessObject entity, string transitionCode, Dictionary<string, object> variables = null, bool setProperty = true)
        {
            if (!this.EntityType.Equals(entity.GetType()))
                throw new ArgumentException("Illegal entity");

            string stateValue = GetActualState(entity);
            // Wykonaj tranzycję do stanu którego kod (Code) znajduje się w zmiennej destination
            IWFStateMachineHolder stateMachine = (IWFStateMachineHolder)entity;

            if (variables == null)
                variables = new Dictionary<string, object>();
            variables["entity"] = entity;
            //    variables["root"] = stateMachine.Process;
            variables["session"] = session;

            variables = stateMachine.PrepareVariables(session, this, variables);
            stateMachine.BeforeTransition(session, transitionCode, this, variables);
            WFActivityBase state = this.Machine.DoTransition(stateValue, transitionCode, variables);

            if (state != null)
            {
                // Ustaw nowy stan w encji
                if (setProperty)
                    entity.SetProperty(this.FieldName, null, state.Code);
                stateMachine.AfterTransition(session, transitionCode, this, variables);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Wykonuje przejście maszyny stnaów dla podanej encji i do podanego stanu.
        /// </summary>
        /// <param name="fieldName">Encja</param>
        /// <param name="connector">Konnektor</param>
        public void DoTransitionToState(Session session, BusinessObject entity, string stateCode, Dictionary<string, object> variables = null, bool setProperty = false)
        {
            string state = GetStateValue(entity);
            var transtion = GetAvailableTransitions(session, entity).Where(x => x.Destination.Code == stateCode).FirstOrDefault();
            if (transtion == null)
                throw new InvalidOperationException(String.Format("Nie można przejść z stanu {0} do stanu {1}", state, stateCode));
            DoTransition(session, entity, transtion.Code, variables, setProperty);
        }


        /// <summary>
        /// Zwraca stan początkowy..
        /// </summary>
        /// <returns>Stan.</returns>
        public string GetFirstState()
        {
            return Machine.GetFirstStateCode();
        }
    }
}
