using System;
using System.ComponentModel.Design;

namespace SoftwareMind.SimpleWorkflow
{
    /// <summary>
    /// Edytor kolekcji zmiennych
    /// </summary>
    [Serializable]
    public class WFVariableCollectionEditor : CollectionEditor
    {

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFVariableCollectionEditor"/>.
        /// </summary>
        /// <param name="type">Typ elementów w kolekcji..</param>
        public WFVariableCollectionEditor(Type type)
            : base(type)
        {
        }




        /// <summary>
        /// Zwraca tekst wyświetlany na liscie obiektów.
        /// </summary>
        /// <param name="value">Obiekt, którego tekst ma zostac zwrócony.</param>
        /// <returns>
        /// Tekst.
        /// </returns>
        protected override string GetDisplayText(object value)
        {
            WFVariableDef item = (WFVariableDef)value;
            return base.GetDisplayText(item.Name);
        }
    }
}
