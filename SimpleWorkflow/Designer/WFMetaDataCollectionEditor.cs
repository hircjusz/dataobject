using System;
using System.ComponentModel.Design;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    [Serializable]
    public class WFMetaDataCollectionEditor : CollectionEditor
    {

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFVariableCollectionEditor"/>.
        /// </summary>
        /// <param name="type">Typ elementów w kolekcji..</param>
        public WFMetaDataCollectionEditor(Type type)
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
            WFActivityMetaData item = (WFActivityMetaData)value;
            return base.GetDisplayText(item.Key);
        }

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        

        protected override Type CreateCollectionItemType()
        {
            return typeof(WFActivityMetaData);
        }
    }
}
