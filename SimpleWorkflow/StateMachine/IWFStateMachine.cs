using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.SimpleWorkflow.StateMachine
{
    public interface IWFStateMachine
    {
        /// <summary>
        /// Zwraca możliwe przejścia maszyny stanów dla podanej kolumny
        /// </summary>
        /// <param name="fieldName">Nazwa kolumny</param>
        /// <returns>Możliwe przejścia maszyny stanów dla podanej kolumny</returns>
        IEnumerable<WFActivityBase> GetAvailableTransitions(string fieldName);

        /// <summary>
        /// Wykonuje przejście maszyny stnaów dla podanej kolumny i podanego konnektora 
        /// </summary>
        /// <param name="fieldName">Nazwa kolumny</param>
        /// <param name="connector">Konnektor</param>
        bool DoTransition(string fieldName, string activityCode);
    }
}
