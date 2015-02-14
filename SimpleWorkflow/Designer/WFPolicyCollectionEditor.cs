using System;
using System.ComponentModel.Design;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Edytor kolekcji polis
    /// </summary>
    [Serializable]
    public class WFPolicyCollectionEditor : CollectionEditor
    {

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFPolicyCollectionEditor"/>.
        /// </summary>
        /// <param name="type">Typ elemenów w kolekcji.</param>
        public WFPolicyCollectionEditor(Type type)
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
            DesingerPolicyItem item = (DesingerPolicyItem)value;
            return base.GetDisplayText(item.PolicyType.Name);
        }
    }
}
