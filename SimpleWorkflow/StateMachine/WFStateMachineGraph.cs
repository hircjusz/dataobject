using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.SimpleWorkflow.StateMachine
{

    /// <summary>
    /// Klasa reprezentująca graf maszyny stanów;
    /// </summary>
    [Serializable]
    public class WFStateMachineGraph
    {
        /// <summary>
        /// Pozwala pobrać i ustawić stan początkowy.
        /// </summary>
        /// <value>The start state.</value>
        public string StartState { get; set; }

        /// <summary>
        /// Graf maszyny stanów.
        /// </summary>
        public string Graph { get; set; }

        /// <summary>
        /// Pozwala pobrać i ustawić nazwę
        /// </summary>
        /// <value>Nazwa.</value>
        public string Name { get; set; }

        /// <summary>
        /// Pozwala pobrać i ustawić krótki alias.
        /// </summary>
        /// <value>Krótki alias.</value>
        public string ShortAlias { get; set; }
    }
}
