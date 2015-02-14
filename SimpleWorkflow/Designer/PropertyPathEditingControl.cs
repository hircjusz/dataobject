using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa reprezentująca edytor ścieżek
    /// </summary>
    [Serializable]
    public class PropertyPathEditingControl : TreeView, IDataGridViewEditingControl
    {

        private static HashSet<Type> ignoreList = new HashSet<Type>() { typeof(string) };
        /// <summary>
        /// Kolekcja definicji zmiennych
        /// </summary>
        private IDictionary<string, Type>  variables;
        /// <summary>
        /// Sformatowana wartość.
        /// </summary>
        private string editingControlFormattedValue;
        /// <summary>
        /// Przycisk wstawienia nowego wiersza.
        /// </summary>
        private MenuItem insertToolStripMenuItem;



        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="PropertyPathEditingControl"/>.
        /// </summary>
        public PropertyPathEditingControl(bool onlyWithSetters)
        {
            OnlyWithSetters = onlyWithSetters;
            ContextMenu = new ContextMenu();
            insertToolStripMenuItem = new MenuItem()
            {
                Text = "Wstaw wszystkich potomków",
            };
            ContextMenu.MenuItems.Add(insertToolStripMenuItem);
            insertToolStripMenuItem.Click += insertToolStripMenuItem_Click;
            ContextMenu.Popup += ContextMenu_Popup;
        }

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="PropertyPathEditingControl"/>.
        /// </summary>
        public PropertyPathEditingControl()
            : this(false)
        {
        }



        /// <summary>
        /// Pozwala pobrać i ustawić edytowaną komórkę.
        /// </summary>
        /// <value>The cell to edit.</value>
        public PropertyPathCell CellToEdit { get; set; }

        /// <summary>
        /// Kontrolka DataViewGrid, która będzie wyśietlałą edytor
        /// </summary>
        /// <value>Kontrolka DataViewGrid</value>
        public DataGridView EditingControlDataGridView
        {
            get;
            set;
        }

        /// <summary>
        /// pozwala pobrać i ustawić sformatowana wartość.
        /// </summary>
        /// <value>Wartość.</value>
        public object EditingControlFormattedValue
        {
            get
            {
                return BuildPath();
            }
            set
            {
                editingControlFormattedValue = value as String;
                SelectPath(editingControlFormattedValue);
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić indeks wiersza.
        /// </summary>
        /// <value>Indeks wiersza</value>
        public int EditingControlRowIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Okresla czy wartość uległa zmianie
        /// </summary>
        /// <value>Zwraca prawdę jeśli wartość się zmienił, w przeciwnym wypadku fałsz.</value>
        public bool EditingControlValueChanged
        {
            get;
            set;
        }

        /// <summary>
        /// Pozwala pobrać kursor
        /// </summary>
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        /// <summary>
        /// Pozwala pobrać wartosć informującą czy wylistowane zostaną włąściwości posiadajace publiczny setter
        /// </summary>
        /// <value>
        /// 	<c>true</c> jeśli tak; w przeciwnym wypadku, <c>false</c>.
        /// </value>
        public bool OnlyWithSetters { get; private set; }

        /// <summary>
        /// Zwraca wartośc czy po edycji komórki muszą zostać ustawione ponownie.
        /// </summary>
        /// <value>Zawsze fałsz.</value>
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Zwaraca wybrany typ.
        /// </summary>
        /// <value>Wybrany typ.</value>
        public Type SelectedType
        {
            get
            {
                return SelectedNode != null ? SelectedNode.Tag as Type : null;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić kolekcje definicji zmiennych
        /// </summary>
        /// <value>The varialbes.</value>
        public IDictionary<string, Type> Variables
        {
            get
            {
                return variables;
            }
            set
            {
                if (variables != value)
                {
                    variables = value;
                    Nodes.Clear();
                    foreach (var item in variables)
                    {
                        TreeNode placeHolderNode = new TreeNode()
                        {
                            Text = "",
                        };
                        TreeNode variableNode = new TreeNode()
                        {
                            Text = item.Key,
                            Tag = item.Value,
                        };
                        variableNode.Nodes.Add(placeHolderNode);
                        Nodes.Add(variableNode);
                        InitNode(variableNode);
                    }
                    
                    SelectPath(editingControlFormattedValue);
                }
            }
        }




        /// <summary>
        /// Ustawia styl
        /// </summary>
        /// <param name="dataGridViewCellStyle">Konrolka hostująca edytor.</param>
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        /// <summary>
        /// Informuje kontrolkę hostującą o typie klawiszy przechwytywanych przez edytor.
        /// </summary>
        /// <param name="key">Klawisz.</param>
        /// <param name="dataGridViewWantsInputKey">Jeśli jest ustawniony na <c>true</c> informuje, e kontrolka DataGridVIew 
        /// jest zainteresowana zdarzeniem..</param>
        /// <returns></returns>
        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        /// <summary>
        /// Zwraca sformatowana wartość.
        /// </summary>
        /// <param name="context">Kontekst wywołania.</param>
        /// <returns>
        /// Sformatowana wartosć.
        /// </returns>
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        /// <summary>
        /// Przygotowywuje kontrolkę do edycji- ignorowane.
        /// </summary>
        /// <param name="selectAll">Prawdaj jeśli konieczne jest zaznaczenie całej zartosci komórki.</param>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        /// <summary>
        /// Zaznacza wskazaną ścieżkę
        /// </summary>
        /// <param name="path">Ścieżka.</param>
        public void SelectPath(string path)
        {
            if (path == null)
            {
                SelectedNode = null;
                return;
            }
            string[] steps = path.Split('.');

            if (steps.Length == 0)
                SelectedNode = null;
            else
            {
                int index = 1;
                SelectedNode = Nodes.OfType<TreeNode>().Where(x => x.Text == steps[0]).FirstOrDefault();
                TreeNode lastNode = SelectedNode;
                while (SelectedNode != null && index < steps.Length)
                {
                    OnBeforeExpand(new TreeViewCancelEventArgs(SelectedNode, false, TreeViewAction.Unknown));
                    lastNode = SelectedNode;
                    SelectedNode = SelectedNode.Nodes.OfType<TreeNode>().Where(x => x.Text == steps[index]).FirstOrDefault();
                    index++;
                }
                if(index != steps.Length)
                    SelectedNode = lastNode;
            }
        }

        /// <summary>
        /// Obsługuję zdarzenie zmiany zaznaczonej wartości.
        /// </summary>
        /// <param name="e">Instancja klasy <see cref="T:System.Windows.Forms.TreeViewEventArgs"/> zawierajaca informacje
        /// o zdarzeniu</param>
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
            CellToEdit.Tag  = SelectedType;
            base.OnAfterSelect(e);
        }

        /// <summary>
        /// Obsługuje zdarzenie rozwinięcia węzła.
        /// </summary>
        /// <param name="e">Instancja klasy <see cref="T:System.Windows.Forms.TreeViewCancelEventArgs"/> zawierajaca informacje
        /// o zdarzeniu</param>
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            foreach(TreeNode n in node.Nodes)
                InitNode(n);

            base.OnBeforeExpand(e);
        }

        /// <summary>
        /// Obsługuje zdarzenie podwójnego kliknięcia węzła.
        /// </summary>
        /// <param name="e">Instancja klasy TreeViewMouseClickEventArgs zawierajaca informacje
        /// o zdarzeniu</param>
        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            EditingControlValueChanged = true;
            EditingControlDataGridView.EndEdit();
        }

        /// <summary>
        /// Buduje ścieżkę z wybranego węzła.
        /// </summary>
        /// <returns></returns>
        private string BuildPath()
        {
            if (SelectedNode == null)
                return "";
            Stack<string> stack = new Stack<string>();
            TreeNode tn = SelectedNode;
            while (tn != null)
            {
                stack.Push(tn.Text);
                tn = tn.Parent;
            }
            return String.Join(".", stack);
        }

        /// <summary>
        /// Obsługuje zdarzenia otwarcia menu kontekstowego. Uaktywania dostępne elementy.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        void ContextMenu_Popup(object sender, EventArgs e)
        {
            insertToolStripMenuItem.Visible = SelectedNode != null && SelectedNode.Nodes.Count > 0;
        }

        /// <summary>
        /// Tworzy węzeł drzewka wraz z wartownikiem, któy zostanie dodany jako potomek.
        /// </summary>
        /// <param name="node">Węzeł do kótrego zostanie dodany potomek.</param>
        /// <param name="name">Nazwa węzła.</param>
        /// <param name="type">Typ wartosci węzła.</param>
        private static void CreateTreeNode(TreeNode node, String name, Type type)
        {
            if(type == typeof(ExtensionDataObject))
                return;

            TreeNode placeHolderNode = new TreeNode()
            {
                Text = "",
            };
            TreeNode propertyNode = new TreeNode()
            {
                Text = name,
                Tag = type,
            };
            propertyNode.Nodes.Add(placeHolderNode);
            node.Nodes.Add(propertyNode);
        }

        /// <summary>
        /// Zwraca ścieżkę z wskazanego węzła
        /// </summary>
        /// <param name="node">Węzeł.</param>
        /// <returns>Ścieżka</returns>
        private string GetPath(TreeNode node)
        {
            Stack<string> stack = new Stack<string>();
            do
            {
                stack.Push(node.Text);
                node = node.Parent;
            }
            while (node != null);
            return string.Join(".", stack);
        }

        /// <summary>
        /// Inicjalizuje węzeł
        /// </summary>
        /// <param name="node">Węzeł do inicjalizacji.</param>
        private void InitNode(TreeNode node)
        {
            if (node.Nodes.Count == 1 && node.Nodes[0].Text == "")
            {
                node.Nodes.Clear();
                Type type = node.Tag as Type;
                if (!ignoreList.Contains(type))
                {
                    var toFiltrProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance); 
                    if ((EditingControlDataGridView.CurrentCell as PropertyPathCell).OnlyWithSetters)
                        toFiltrProperties = toFiltrProperties.Where(x => x.GetSetMethod() != null && x.GetSetMethod().IsPublic).ToArray();
                    var properties = toFiltrProperties.Select(x => new KeyValuePair<string, Type>(x.Name, x.PropertyType));
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(x => new KeyValuePair<string, Type>(x.Name, x.FieldType));
                    var toCreate = properties.Union(fields).OrderBy(x => x.Key);
                    foreach (var item in toCreate)
                        CreateTreeNode(node, item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// Obsługuje zdarzenia kliknięcia przycisku dodawania nowego wiersza.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dvg = EditingControlDataGridView;
            var row = dvg.CurrentCell.OwningRow;
            EditingControlValueChanged = false;
            if(row.Index != -1)
                dvg.Rows.Remove(row);
            dvg.SuspendLayout();
            string path = GetPath(SelectedNode);
            string[] oldValues = dvg.Rows.OfType<DataGridViewRow>().Select(x => x.Cells[0].Value as String).ToArray();
            foreach (var node in SelectedNode.Nodes.OfType<TreeNode>())
            {
                string newPath = path + "." + node.Text;
                if (oldValues.Contains(newPath))
                    continue;

                DataGridViewRow newRow = new DataGridViewRow();
                newRow.Cells.Add(new PropertyPathCell(true)
                {
                    Value = newPath,
                });
                Type type = VariableMapEditorDialog.GetExpectedType(newPath, Variables);
                newRow.Cells.Add(new DataGridViewTextBoxCell() 
                {
                    Value = type.Name,
                    Tag = type,
                });
                newRow.Cells.Add(new PropertyPathCell(false));
                dvg.Rows.Add(newRow);
            }
            dvg.ResumeLayout();

        }
    }
}
