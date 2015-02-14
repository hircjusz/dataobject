using System;
using System.Linq;
using System.Collections.Generic;
using SoftwareMind.Scripts;
using System.Collections.Specialized;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    /// <summary>
    /// Klasa z metodami pomocniczymi do obsługi skryptów
    /// </summary>
    [Serializable]
    public static class ScriptHelper
    {


        /// <summary>
        /// Wzkonuje skrzypt z zadanymi wartosciami zmiennych
        /// </summary>
        /// <param name="scriptSource">Skrypt.</param>
        /// <param name="enviroment">Zmienne.</param>
        /// <returns></returns>
        public static object Execute(string scriptSource, IDictionary<string, object> enviroment)
        {
            Script script = new Script(scriptSource, enviroment);
            return script.Execute();
        }

        /// <summary>
        /// Wzkonuje skrzypt z zadanymi wartosciami zmiennych
        /// </summary>
        /// <param name="script">Skrypt.</param>
        /// <param name="instance">Instancja aktywności, z której będą pobierane zmienne.</param>
        /// <returns></returns>
        public static object Execute(string script, IWFActivityInstance instance)
        {
            List<string> processVariables = new List<string>();

            IDictionary<string, object> activityVariables = instance.GetVariables();

            foreach (var variable in instance.ProcessInstance.GetVariables())
            {
                if (activityVariables.ContainsKey(variable.Key) == false)
                {
                    activityVariables.Add(variable);
                    processVariables.Add(variable.Key);
                }
            }

            if(activityVariables.Keys.Contains("activityInstance") == false)
                activityVariables.Add("activityInstance", instance);

            object result = new Script(script, activityVariables).Execute();

            foreach (var key in activityVariables.Keys.ToList())
            {
                if (processVariables.Contains(key))
                {
                    instance.ProcessInstance.SetVariableValue(key, activityVariables[key]);
                }
                else
                {
                    instance.SetVariableValue(key, activityVariables[key]);
                }
            }
            return result;
        }

        /// <summary>
        /// Sprawdza poprawnosc skryptu.
        /// </summary>
        /// <param name="scriptSource">Skrypt.</param>
        /// <param name="enviroment">Zmienne.</param>
        /// <param name="cee">Błąd.</param>
        /// <returns>Zwraca prawdę jeśli skrypt skompiluje się poprawniel w przeciwnym wypadku fałsz.</returns>
        public static bool Validate(string scriptSource, WFVariableDefCollection enviroment, out Exception cee)
        {
            cee = null;
            try 
            {
                Script script = new Script(scriptSource);
                script.Validate(enviroment.GetTypeDictionary());
                return true;
            }
            catch (Exception ex)
            {
                cee = ex;
                return false;
            }
        }
    }
}
