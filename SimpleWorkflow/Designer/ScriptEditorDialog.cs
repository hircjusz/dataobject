using System.Windows.Forms;
using System;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Okno dialogowe edytora skryptów
    /// </summary>
    [Serializable]
    public partial class ScriptEditorDialog : Form
    {
        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="ScriptEditorDialog"/>.
        /// </summary>
        public ScriptEditorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pozwala pobrać i ustawić kolekcję zmiennych.
        /// </summary>
        /// <value>Kolekcja zmiennych.</value>
        public WFVariableDefCollection VariableCollection
        {
            get
            {
                return scriptEditor.VariableCollection;
            }
            set
            {
                scriptEditor.VariableCollection = value;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić skrypt.
        /// </summary>
        /// <value>Skrypt.</value>
        public string Script
        {
            get
            {
                return scriptEditor.Script;
            }
            set
            {
                scriptEditor.Script = value;
            }
        }
    }
}
