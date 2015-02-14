using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDataObjects.ObjectsDomain;
using ConsoleDataObjects.StateMachine;
using DataObjects.NET;

namespace ConsoleDataObjects.Utility
{
    public static class UtilityActions
    {
        public static void DoStateMachineTransition(Session session,BusinessObject entity,string fieldName,string transistion) {

            WFEntityStateMachine stateMachine = null;
            IWFStateMachineHolder stateMachineHolder = (IWFStateMachineHolder)entity;
            stateMachine = stateMachineHolder.GetStateMachine(fieldName);
            var status = stateMachine.DoTransition(session, entity, transistion, new Dictionary<string, object>());
        }

    }
}
