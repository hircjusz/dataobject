using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects.NET;

namespace ConsoleDataObjects.StateMachine
{
    public interface IWFStateMachineHolder
    {
        /// <summary>
        /// Zwraca maszynę stanów dla podanego typu i pola o podanej nazwie
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        WFEntityStateMachine GetStateMachine(string fieldName);

        /// <summary>
        /// Zwraca ścieżkę do szablonu maszyny stanu zmodyfikowaną o aktualny stan encji.
        /// </summary>
        /// <param name="oldPath">Stara ścieżka.</param>
        /// <param name="fieldName">Pole, któego dotyczy zapytanie.</param>
        /// <returns>Nowa ścieżka.</returns>
        string GetStateMachinePath(string oldPath, string fieldName);

        /// <summary>
        /// Metoda wykonywana przed pobraniem dostępnych połączeń.
        /// </summary>
        /// <param name="transitionCode">Kod tranzycji..</param>
        /// <param name="stateMachine">Maszyna stanu wykorzystywana przy tranzycji.</param>
        /// <param name="variables">Słownik zawierający zmienne przekazane przy wywołaniu tranzycji..</param>
        Dictionary<string, object> PrepareVariables(Session session, WFEntityStateMachine stateMachine, Dictionary<string, object> variables);

        /// <summary>
        /// Metoda wykonywana po wykonaniu tranzycji.
        /// </summary>
        /// <param name="transitionCode">Kod tranzycji..</param>
        /// <param name="stateMachine">Maszyna stanu wykorzystywana przy tranzycji.</param>
        /// <param name="variables">Słownik zawierający zmienne przekazane przy wywołaniu tranzycji..</param>
        void AfterTransition(Session session, string transitionCode, WFEntityStateMachine stateMachine, Dictionary<string, object> variables);

        /// <summary>
        /// Metoda wykonywana przed wykonaniu tranzycji.
        /// </summary>
        /// <param name="transitionCode">Kod tranzycji..</param>
        /// <param name="stateMachine">Słownik zawierający zmienne przekazane przy wywołaniu tranzycji.</param>
        void BeforeTransition(Session session, string transitionCode, WFEntityStateMachine stateMachine, Dictionary<string, object> variables);

        
    }
}
