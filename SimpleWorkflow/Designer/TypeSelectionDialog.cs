using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa reprezentująca okno dialogowe pozwalajace wybrać Typ oraz jego konstruktor do wywołania
    /// </summary>
    public partial class TypeSelectionDialog : Form
    {

        /// <summary>
        /// Typ bazowy
        /// </summary>
        private Type _baseType;
        /// <summary>
        /// Mapowanie zmiennych
        /// </summary>
        private VariableMapper _vm;



        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="TypeSelectionDialog"/>.
        /// </summary>
        public TypeSelectionDialog()
        {
            InitializeComponent();
            _vm = new VariableMapper();
        }



        /// <summary>
        /// Pozwala pobrać i ustawić typ obiektu bazowego
        /// </summary>
        /// <value>Typ obiektu bazowego.</value>
        public Type BaseType
        {
            get
            {
                return _baseType;
            }
            set
            {
                _baseType = value;
                typeComboBox.Items.Clear();
                if (value != null)
                {
                    var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).
                        Where(x => value.IsAssignableFrom(x)).ToArray();
                    types = types.Where(x => x.IsPublic).ToArray();
                    foreach (var t in types)
                        typeComboBox.Items.Add(t.FullName);
                }
                if (typeComboBox.Items.Count == 0)
                    typeComboBox.Items.Add("");
                typeComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić mapowanie zmiennych
        /// </summary>
        /// <value>The map.</value>
        public string[] Map
        {
            get
            {
                return _vm.VariableMap;
            }
        }

        /// <summary>
        /// Zwraca wybrany konstruktor
        /// </summary>
        /// <value>Wybrany konstruktor.</value>
        public MethodBase SelectedConstructor
        {
            get
            {
                return MethodBaseHelper.GetMethodBase(SelectedType, constructorComboBox.SelectedItem as String);
            }
        }

        /// <summary>
        /// Zwraca wybrany typ.
        /// </summary>
        /// <value>Wybrany typ.</value>
        public Type SelectedType
        {
            get
            {
                return TypeHelper.GetType(typeComboBox.SelectedItem as string);
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić kolekcję zmiennych.
        /// </summary>
        /// <value>The variable def collection.</value>
        public WFVariableDefCollection VariableDefCollection { get; set; }




        /// <summary>
        /// Obsługuje akcje wciśniecia przycisku anuluj.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Obsługuje akcję zmiany zaznaczonego elementu w liscie wyboru konstruktora.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void constructorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(constructorComboBox.SelectedItem as String))
                okButton.Enabled = false;
            else
                okButton.Enabled = true;
        }

        /// <summary>
        /// Handles the Click event of the mapButton control.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void mapButton_Click(object sender, EventArgs e)
        {
            using (VariableMapEditorDialog ved = new VariableMapEditorDialog())
            {
                
                ved.VariableMapper = _vm;
                ved.VariablesDefCollection = VariableDefCollection;
                ved.Method = MethodBaseHelper.GetMethodBase(SelectedType, constructorComboBox.SelectedItem as String);
                _vm.Method = ved.Method;
                ved.Map = _vm.VariableMap;
                var result = ved.ShowDialog();
                if (result == DialogResult.OK)
                    _vm.VariableMap = ved.Map;
            }
        }

        /// <summary>
        /// Obsługuje akcje wcisniecia przycisku ok.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(typeComboBox.SelectedItem as String))
            {
                MessageBox.Show("Wybierz typ z listy by kontynuować.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Obsługuje akcję zmiany wybranego typu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(typeComboBox.SelectedItem as String))
            {
                constructorComboBox.Enabled = false;
                okButton.Enabled = false;
            }
            else
            {
                constructorComboBox.Enabled = true;
                var constructors = SelectedType.GetConstructors().Where(x => x.IsPublic);
                constructorComboBox.Items.Clear();
                foreach (var item in constructors)
                    constructorComboBox.Items.Add(MethodBaseHelper.GenerateUniqueMethodName(item));
                if (constructorComboBox.Items.Count == 0)
                    constructorComboBox.Items.Add("");
                constructorComboBox.SelectedIndex = 0;
            }
        }
    }
}
