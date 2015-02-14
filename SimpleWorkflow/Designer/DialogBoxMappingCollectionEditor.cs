using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    [Serializable]
    public class DialogBoxMappingCollectionEditor : CollectionEditor
    {

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFVariableCollectionEditor"/>.
        /// </summary>
        /// <param name="type">Typ elementów w kolekcji..</param>
        public DialogBoxMappingCollectionEditor(Type type)
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
            MappingItem item = (MappingItem)value;
            return base.GetDisplayText(item.From);
        }

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        

        protected override Type CreateCollectionItemType()
        {
            return typeof(MappingItem);
        }
    }
}
