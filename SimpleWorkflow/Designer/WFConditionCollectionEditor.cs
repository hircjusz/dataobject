using System;
using System.ComponentModel.Design;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Edytor kolekcji warunków
    /// </summary>
    [Serializable]
    public class WFConditionCollectionEditor : CollectionEditor
    {

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFConditionCollectionEditor"/>.
        /// </summary>
        /// <param name="type">Typ obiektów w kolekcji.</param>
        public WFConditionCollectionEditor(Type type)
            : base(type)
        {
        }




        /// <summary>
        /// Zwraca wyświetlany tekst dla obiektu na liscie.
        /// </summary>
        /// <param name="value">Obiekt dla któego ma być zwrócona nazwa</param>
        /// <returns>
        /// String reprezentujący obiekt.
        /// </returns>
        protected override string GetDisplayText(object value)
        {
            DesingerConditionItem item = (DesingerConditionItem)value;
            return item.ConditionType.Name;
        }
    }
}
