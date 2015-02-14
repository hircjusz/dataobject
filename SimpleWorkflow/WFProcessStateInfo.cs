using System.Collections.Generic;

namespace SoftwareMind.SimpleWorkflow
{
    public sealed class WFProcessStateInfo
    {
        /// <summary>
        /// xml reprezentujacy stan procesu np. kroki obecnie wykonywane
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// xml reprezentujący wzór procesu workflow
        /// </summary>
        public string Template { get; set; }
        /// <summary>
        /// Lista kroków wykonywanych w obecnej chwili. 
        /// Ta kolekcja jest tylko dla celów informacyjnych zeby użytkownik wiedzial jakie kroki sa wykonywane
        /// </summary>
        public IEnumerable<WFActivityBase> Steps { get; set; }
    }
}
