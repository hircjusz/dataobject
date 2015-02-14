using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa reprezentująca kolumnę edytowa ściezek przypisań
    /// </summary>
    [Serializable]
    public class PropertyPathColumn : DataGridViewColumn
    {
        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="PropertyPathColumn"/>.
        /// </summary>
        public PropertyPathColumn()
            : base(new PropertyPathCell(false))
        {
        }

        /// <summary>
        /// Pozwala pobrać i ustwaić szablon.
        /// </summary>
        /// <value>Szablon.</value>
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(PropertyPathCell)))
                    throw new InvalidCastException("Komórka musi być typu " + typeof(PropertyPathCell).Name);
                base.CellTemplate = value;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić kolekcję definicji zmiennych.
        /// </summary>
        /// <value>Kolekcja definicji zmiennych</value>
        public IDictionary<string, Type> VariablesDef { get; set; }
    }

}
