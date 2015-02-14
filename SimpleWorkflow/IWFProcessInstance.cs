using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Activities;

namespace SoftwareMind.SimpleWorkflow
{
    public interface IWFProcessInstance : IWFVariableContainer
    {
        void AddActivity(IWFActivityInstance activityInstance);
        bool ContainsActivity(string code);
        void PushToWorkList(IWFActivityInstance activity);
        IWFActivityInstance PopFromWorkList();
        bool HasActivityToWork();
        bool ExecuteWorkList();
        IWFActivityInstance GetActivity(string code);
        IWFConnectorBase GetDefaultTransition(string activityCode, IDictionary<string, object> args);
        IWFConnector TransitionConnector { get; set;  }
        IEnumerable<IWFConnectorBase> GetTransitions(string activityCode, IDictionary<string, object> args);
        Guid ID { get; set; }
        IWFProcessInstanceBox Box { get; set; }
        bool IsOnLongRunning { get; set; }
        bool IsClosed();
        WFProcess Process { get; }
        bool IsActivityCompleted(string code, string path);
        string ProcessName { get; set; }
        void ReadStateFromXmlElement(System.Xml.Linq.XElement element);
        void RemoveActivity(string code);
        System.Collections.Generic.IList<IWFActivityInstance> WorkList { get; }
        System.Xml.Linq.XElement WriteStateToXmlElement();
        void FireSignal(IWFConnector connector, IDictionary<string, object> arguments);
        void FireEvent(string eventCode, IDictionary<string, object> arguments);
        /// <summary>
        /// Zwraca aktywność + jeśli istnieją to aktywności zmultiplikowane
        /// </summary>
        IEnumerable<IWFActivityInstance> GetActivities(string realActivityCode);
        IEnumerable<IWFActivityInstance> GetActivities();     
        IEnumerable<IWFConnector> GetAvailableGlobalEvents();
        IWFProcessInstance ChangeProcessDefinition(string processDefinitionCode);
        IWFConnector GetConnector(string code);
        bool IsWaiting(string code);
        bool IsAnyActive(string code);
        bool IsActive(string code);
        IWFProcessInstance ProcessOwner { get; set; }
        WFTransitionStatus DoTransition(string activityCode, string connectorCode, Dictionary<string, object> arguments/*, Action<IWFActivityInstance> beforeCompleted*/);
        /// <summary>
        /// Zwraca aktywności bez rodzica
        /// </summary>
        /// <param name="code">Kod</param>
        /// <returns>Aktywności</returns>
        IEnumerable<IWFActivityInstance> GetActivitiesWithoutParent(string code);
        /// <summary>
        /// Zwraca aktywności z konkretnym rodzicem
        /// </summary>
        /// <param name="code">Kod</param>
        /// <param name="paretnPath">ścieżka rodizca</param>
        /// <returns>Aktywności</returns>
        IEnumerable<IWFActivityInstance> GetActivitiesWithParent(string code, string paretnPath);
        /// <summary>
        /// Zwraca aktywności z konkretnym przodkiem - pierwszym ze wszystkich przodków - jeżeli ma tylko rodzica to zwraca taki sam jak w przypadku GetActivietiesWithParent
        /// </summary>
        /// <param name="code">Kod</param>
        /// <param name="paretnPath">ścieżka rodizca</param>
        /// <returns>Aktywności</returns>
        IEnumerable<IWFActivityInstance> GetActivitiesWithAncestor(string code, string paretnPath);
    }


    public delegate IWFProcessInstance WFProcessInstanceOperation(IWFProcessInstance instance);
    public delegate void WFProcessActivityOperation();

    public interface IWFProcessInstanceBox : IDisposable
    {
        IWFProcessInstance UseProcessInstance(WFProcessInstanceOperation operation, bool forceLock = false);
        IWFProcessInstance ForwardProcessInstance(IWFProcessInstance instance, WFProcessActivityOperation operation);
        void Transactional(IWFProcessInstance instance, bool recovery, WFProcessActivityOperation operation);
        IWFProcessInstanceBox Repack(IWFProcessInstance instance);
    }
}
