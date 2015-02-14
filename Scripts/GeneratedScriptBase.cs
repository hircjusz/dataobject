
using System;
namespace SoftwareMind.Scripts
{
    /// <summary>
    /// Klasa bazowa dla dynamicznie wygenerowanych skryptów
    /// </summary>
    [Serializable]
    public abstract class GeneratedScriptBase
    {


        /// <summary>
        /// Wykonuje skrypt
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Zwraca zmienną o nazwie <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Nazwa zmiennej</param>
        /// <returns>Wartość zmiennej</returns>
        public abstract object GetValue(string name);

        /// <summary>
        /// Ustawia wartość<paramref name="name"/>.
        /// </summary>
        /// <param name="name">Nazwa zmiennej.</param>
        /// <param name="value">Wartość zmiennej.</param>
        public abstract void SetValue(string name, object value);
    }
}
