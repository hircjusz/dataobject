using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class WFProcessCoordinator : SoftwareMind.SimpleWorkflow.IWFProcessCoordinator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFProcessCoordinator));

        public static WFProcessCoordinator Instance
        {
            get
            {
                return new WFProcessCoordinator();
            }
        }

        /// <remarks>
        /// Ta metoda musi zapisywać do runningProcesses
        /// poniewać umożliwiamy zapis stanu podczas trwania procesu co moze umożliwić odpalenie drugi raz tej samej instancji procesu
        /// </remarks>
        public IWFProcessInstance RunProcess(IWFProcessInstanceBox box, string processName, IDictionary<string, object> processArguments, WFVariableDefCollection defsToAdd)
        {
            log.InfoFormat("Uruchomienie nowego procesu: {0}", processName);

            WFProcess process = WFProcessFactory.GetProcess(processName);
            if (defsToAdd != null)
            {
                foreach (var vdef in defsToAdd)
                {
                    if(!process.VariableDefs.ContainsKey(vdef.Key))
                        process.VariableDefs.Add(vdef);
                }
            }
            return box.UseProcessInstance(delegate(IWFProcessInstance instance)
            {
                instance = new WFProcessInstance(process);
                instance.Box = box;
                box.Transactional(instance, false, () => { }); //hack!! we want WfProcess saved into Process.ProcessInstance from the begining so we make fake operation here
                process.Run(instance as WFProcessInstance, processArguments);
                log.InfoFormat("Uruchomienie nowego procesu: {0} od id: {1} zakończono pomyślnie", processName, instance.ID);
                instance.ExecuteWorkList();
                return instance;
            });

        }

        /// <remarks>
        /// Ta metoda musi zapisywać do runningProcesses
        /// poniewać umożliwiamy zapis stanu podczas trwania procesu co moze umożliwić odpalenie drugi raz tej samej instancji procesu
        /// </remarks>
        public IWFProcessInstance RunProcess(IWFProcessInstanceBox box, string processName, IDictionary<string, object> processArguments)
        {
            return RunProcess(box, processName, processArguments, null);
        }

        /// <remarks>
        /// Ta metoda musi zapisywać do runningProcesses
        /// poniewać umożliwiamy zapis stanu podczas trwania procesu co moze umożliwić odpalenie drugi raz tej samej instancji procesu
        /// </remarks>
        internal IWFProcessInstance RunProcess(WFProcess process, IDictionary<string, object> processArguments, string processName, IWFProcessInstanceBox box)
        {
            return box.UseProcessInstance(delegate(IWFProcessInstance instance)
            {
                instance = process.Run(instance as WFProcessInstance, processArguments);
                if (processName != null)
                    log.InfoFormat("Uruchomienie nowego procesu: {0} od id: {1} zakończono pomyślnie", processName, instance.ID);
                instance.ExecuteWorkList();
                return instance;
            });
        }

        /// <summary>
        /// Wykonuje akcje przejścia z jednego kroku do drugiego
        /// </summary>
        public IWFTransitionResult DoTransition(IWFProcessInstanceBox box, string activityCode, string connectorCode, Dictionary<string, object> arguments)
        {
            WFTransitionStatus result = WFTransitionStatus.Successful;
            IWFProcessInstance modifiedInstance = box.UseProcessInstance(delegate(IWFProcessInstance instance)
            {
                return DoTransitionInternal(instance, box, activityCode, connectorCode, ref result, arguments);
            });

            return new WFTransitionResult() { ProcessInstace = modifiedInstance, Result = result };
        }

        private IWFProcessInstance DoTransitionInternal(IWFProcessInstance instance, IWFProcessInstanceBox box, string activityCode, string connectorCode, ref WFTransitionStatus result, Dictionary<string, object> arguments)
        {
            WFTransitionStatus r = WFTransitionStatus.Successful;
            log.InfoFormat("Wykonanie przejscia dla procesu: {0}, aktywności: {1}, connectora: {2}", instance.ID, activityCode, connectorCode);
            box.Transactional(instance, false, delegate()
            {
                r = instance.DoTransition(activityCode, connectorCode, arguments);
            });
            result = r;
            log.InfoFormat("Przejscie dla procesu: {0}, aktywności: {1}, connectora: {2} zostało wykonane. Wykonywanie pozostałcyh aktywności", instance.ID, activityCode, connectorCode);
            instance.ExecuteWorkList();
            return instance;
        }

        public IWFProcessInstance FireSignal(IWFProcessInstanceBox box, string eventConnectorCode, IDictionary<string, object> arguments)
        {
            return box.UseProcessInstance(delegate(IWFProcessInstance instance)
            {
                return FireSignalInternal(instance, box, eventConnectorCode, arguments);
            });
        }

        private IWFProcessInstance FireSignalInternal(IWFProcessInstance instance, IWFProcessInstanceBox box, string eventConnectorCode, IDictionary<string, object> arguments)
        {
            log.InfoFormat("Event dla procesu: {0}, event: {1}", instance.ID, eventConnectorCode);
            IWFConnector connector = instance.Process.GetAllActivities().SelectMany(x => x.ConnectorsOutgoing).Where(x => x.Code == eventConnectorCode).Single();
            box.Transactional(instance, false, delegate()
            {
                instance.FireSignal(connector, arguments);
            });
            instance.ExecuteWorkList();
            return instance;
        }

        /// <summary>
        /// Wykonuje akcje przejścia z jednego kroku do drugiego lub wywolanie sygnalu w zaleznosci od tego jaki jest typ polaczenia
        /// </summary>
        public IWFTransitionResult DoTransitionOrFireSignal(IWFProcessInstanceBox box, string activityCode, string connectorCode, string connectionType, Dictionary<string, object> arguments)
        {
            WFTransitionStatus result = WFTransitionStatus.Successful;
            IWFProcessInstance modifiedInstance = box.UseProcessInstance(delegate(IWFProcessInstance instance)
            {
                if (connectionType == "Event")
                {
                    //aktualny krok konczymy
                    if(!string.IsNullOrEmpty(activityCode))
                        instance.GetActivity(activityCode).State = WFActivityState.Completed;
                    instance = FireSignalInternal(instance, box, connectorCode, arguments);
                }
                else
                {
                    instance = DoTransitionInternal(instance, box, activityCode, connectorCode, ref result, arguments);
                }
                
                return instance;
            });

            return new WFTransitionResult() { ProcessInstace = modifiedInstance, Result = result };
        }

        public IWFProcessInstance FireEventByActivityCode(IWFProcessInstanceBox box, string eventCode, IDictionary<string, object> arguments)
        {
            return box.UseProcessInstance(delegate(IWFProcessInstance instance)
            {
                log.InfoFormat("Event dla procesu: {0}, event: {1}", instance.ID, eventCode);
                instance.FireEvent(eventCode, arguments);
                instance.ExecuteWorkList();
                return instance;
            });
        }

        public IWFProcessInstance Recovery(IWFProcessInstanceBox box)
        {
            return box.UseProcessInstance(delegate(IWFProcessInstance instance)
            {
                log.InfoFormat("Recovery dla procesu: {0}", instance.ID);
                instance.ExecuteWorkList();
                return instance;
            }, true);
        }

        public IWFProcessInstance SetVariable(IWFProcessInstanceBox box, string variable, object value)
        {
            return box.UseProcessInstance(delegate(IWFProcessInstance instance)
            {
                box.Transactional(instance, false, delegate()
                {
                    instance.SetVariableValue(variable, value);
                });
                return instance;
            });
        }

        private IWFProcessInstance SynchronizeWorkflowExecution(IWFProcessInstance instance, Action onRegister, Action onUnregister, Func<bool> operation)
        {
            bool allTasksCompleted = false;

            Register(instance, onRegister);

            try
            {
                allTasksCompleted = operation();
            }
            finally
            {
                if (allTasksCompleted)
                    UnRegister(instance, onUnregister);
            }

            return instance;
        }

        // Jeśli operacja jest długotrwała, pozostawiamy odrejestrowanie sie operacji (ExecuteWorkList())
        internal void UnRegister(IWFProcessInstance instance, Action onUnRegister)
        {
            if (onUnRegister != null)
                onUnRegister();
        }

        private void Register(IWFProcessInstance instance, Action onRegister)
        {
            if (onRegister != null)
                onRegister();
        }
    }
}
