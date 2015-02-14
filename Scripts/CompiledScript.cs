using System;
using System.Reflection;
using System.Security;
using System.Runtime.InteropServices;

namespace SoftwareMind.Scripts
{
    /// <summary>
    /// Klasa reprezentuj¹ca skompilowany skrypt
    /// </summary>
    public class CompiledScript : MarshalByRefObject
    {

        /// <summary>
        /// Obiekt wygenerowanego skryptu.
        /// </summary>
        private GeneratedScriptBase generatedScript;
        /// <summary>
        /// Nazwa klasy skrytpu
        /// </summary>
        internal const string className = "GeneratedScript";



        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="CompiledScript"/>.
        /// </summary>
        /// <param name="sourceCode">Kod Ÿród³owy.</param>
        /// <param name="referencedAssemblies">Zale¿ne Assemlby.</param>
        public CompiledScript(byte[] assemblyData)
        {
            Assembly assembly = Assembly.Load(assemblyData);
            generatedScript = (GeneratedScriptBase)assembly.CreateInstance(className);
        }




        /// <summary>
        /// Wykonuje skrypt.
        /// </summary>
        public void Execute()
        {
            generatedScript.Execute();
        }

        /// <summary>
        /// Zwraca wartoœæ zmiennej.
        /// </summary>
        /// <param name="name">Nazwa zmiennej.</param>
        /// <returns>Wartosc zmiennej.</returns>
        public object GetValue(string name)
        {
            return generatedScript.GetValue(name);
        }

        /// <summary>
        /// Pozwala ustawiæ wartosæ zmiennej
        /// </summary>
        /// <param name="name">Nazwa zmiennej.</param>
        /// <param name="value">Wartosæ zmiennej.</param>
        public void SetValue(string name, object value)
        {
            generatedScript.SetValue(name, value);
        }

        /// <summary>
        /// Nadpisuje metodê zwracaj¹c¹ obiekt zarz¹dzaj¹cy cyklem ¿ycia. Teraz obiekt nie bedzie ju¿ niszczony automatycznie.
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
