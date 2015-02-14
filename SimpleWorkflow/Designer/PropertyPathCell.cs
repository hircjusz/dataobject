using System;
using System.Windows.Forms;

namespace SoftwareMind.SimpleWorkflow.Designer
{

    /// <summary>
    /// Klasa reprezentująca komórke edytora ścieżek mapowań
    /// </summary>
    [Serializable]
    public class PropertyPathCell : DataGridViewTextBoxCell
    {
        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="PropertyPathCell"/>.
        /// </summary>
        public PropertyPathCell(bool onlyWithSetters)
            : base()
        {
            OnlyWithSetters = onlyWithSetters;
        }

        public PropertyPathCell()
            : this(false)
        {
        }

        public bool OnlyWithSetters { get; private set; }

        /// <summary>
        /// Inicjalizuje edytor
        /// </summary>
        /// <param name="rowIndex">Ineks wiersza.</param>
        /// <param name="initialFormattedValue">Początkowa awrtość.</param>
        /// <param name="dataGridViewCellStyle">AStyl komówki.</param>
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, 
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            PropertyPathEditingControl ctl = DataGridView.EditingControl as PropertyPathEditingControl;

            ctl.EditingControlFormattedValue = Value;
            PropertyPathColumn column = OwningColumn as PropertyPathColumn;
            ctl.Variables = column.VariablesDef;
            ctl.CellToEdit = this;
        }

        /// <summary>
        /// Zwraza typ edytora
        /// </summary>
        public override Type EditType
        {
            get
            {
                return typeof(PropertyPathEditingControl);
            }
        }

        /// <summary>
        /// Zwraca typ wartosći komówki
        /// </summary>
        public override Type ValueType
        {
            get
            {
                return typeof(string);
            }
        }

        /// <summary>
        /// Wartość poczatkowa komórki
        /// </summary>
        public override object DefaultNewRowValue
        {
            get
            {
                return "";
            }
        }
    }

}
