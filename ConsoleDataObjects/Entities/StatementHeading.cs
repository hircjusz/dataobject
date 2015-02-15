using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDataObjects.ObjectsDomain;
using ConsoleDataObjects.StateMachine;
using DataObjects.NET.Attributes;
using SoftwareMind.SimpleWorkflow.StateMachine;

namespace ConsoleDataObjects.Entities
{
    [Flat]
    public abstract class StatementHeading : BusinessObject, IWFStateMachineHolder
    {

        [WFStateMachine("StatmentMachine.xml")]
        [Length(1)]
        [Indexed]
        public abstract string Status { get; set; }





        public virtual WFEntityStateMachine GetStateMachine(string fieldName)
        {
            return WFEntityStateMachine.GetStateMachine(this, fieldName);
        }

        public virtual string GetStateMachinePath(string oldPath, string fieldName)
        {
            return oldPath;
        }

        public virtual Dictionary<string, object> PrepareVariables(DataObjects.NET.Session session, WFEntityStateMachine stateMachine, Dictionary<string, object> variables)
        {
            variables["CanMove"] = false;
            return variables;
        }

        public virtual void AfterTransition(DataObjects.NET.Session session, string transitionCode, WFEntityStateMachine stateMachine, Dictionary<string, object> variables)
        {

        }

        public virtual void BeforeTransition(DataObjects.NET.Session session, string transitionCode, WFEntityStateMachine stateMachine, Dictionary<string, object> variables)
        {

        }
    }
}
