using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SoftwareMind.SimpleWorkflow.Misc;
using SoftwareMind.Logger;
using log4net;


namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa reprezentująca edytor mapowania zmiennych do parametrów metod
    /// </summary>
    [Serializable]
    public partial class VariableMapEditorDialog : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(VariableMapEditorDialog));

        /// <summary>
        /// Metoda do mapowania
        /// </summary>
        /// <value>Metoda.</value>
        private MethodBase _method;
        /// <summary>
        /// Mapowanie zmiennych
        /// </summary>
        private VariableMapper _variableMapper;
        /// <summary>
        /// Mapowanie
        /// </summary>
        private string[] map;
        /// <summary>
        /// Ostatnio zaznaczony wiersz.
        /// </summary>
        private DataGridViewRow selectedRow;


        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="VariableMapEditorDialog"/>.
        /// </summary>
        public VariableMapEditorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pozwala pobrać mapowanie
        /// </summary>
        /// <value>Mapowanie.</value>
        public string[] Map
        {
            get
            {
                return map;
            }
            set
            {
                map = value;
            }
        }

        /// <summary>
        /// Metoda do mapowania.
        /// </summary>
        /// <value>Metoda.</value>
        public MethodBase Method
        {
            get
            {
                return _method;
            }
            set
            {
                _method = value;
                if (_method != null)
                    nameColumn.VariablesDef = _method.GetParameters().ToDictionary(x => x.Name, x => x.ParameterType);
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić mapowanie zmiennych.
        /// </summary>
        /// <value>Mapowanie zmiennych.</value>
        public VariableMapper VariableMapper
        {
            get
            {
                return _variableMapper;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Mapowanie zmiennych nie możę być nullem");
                _variableMapper = value;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić kolekjcę definicji zmiennych
        /// </summary>
        /// <value>The variables def collection.</value>
        public IDictionary<string, Type> VariablesDefCollection
        {
            get
            {
                return pathColumn.VariablesDef;
            }
            set
            {
                pathColumn.VariablesDef = value;
            }
        }

        /// <summary>
        /// Wyświetla okno dialogowe
        /// </summary>
        public new DialogResult ShowDialog()
        {
            if (Method == null)
                return System.Windows.Forms.DialogResult.None;

            mapDataGridView.Rows.Clear();
            BuildDataGridItems();

            return base.ShowDialog();
        }

        /// <summary>
        /// Obsługuje zdarzenie dodania nowego wiersza do tabeli..
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapDataGridView.SuspendLayout();
            DataGridViewRow row = new DataGridViewRow();
            row.Cells.Add(new PropertyPathCell(true)
            {
                Value = "",
            });
            row.Cells.Add(new DataGridViewTextBoxCell()
            {
                Value = "",
            });
            row.Cells.Add(new PropertyPathCell(false)
            {
                Value = "",
            });
            mapDataGridView.Rows.Add(row);
            mapDataGridView.ResumeLayout();
            row.Selected = true;
        }

        /// <summary>
        /// Wypełnia tablę danymi.
        /// </summary>
        private void BuildDataGridItems(int? start = null, int? end = null)
        {
            String[][] entries = map.Select(x => new { Entry = x, SplitPosition = x.IndexOf('=') }).
                Select(x => new string[] { x.Entry.Remove(x.SplitPosition), x.Entry.Substring(1 + x.SplitPosition) }).ToArray();

            ParameterInfo[] parameters = Method.GetParameters();

            mapDataGridView.SuspendLayout();
            for (int i = 0; i < entries.Length; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                try
                {
                    Type expectedType = GetExpectedType(entries[i][0], nameColumn.VariablesDef);
                    row.Cells.Add(new PropertyPathCell(true)
                    {
                        Value = entries[i][0],
                    });
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = expectedType.Name,
                        Tag = expectedType,
                    });
                    row.Cells.Add(new PropertyPathCell(false)
                    {
                        Value = entries[i][1],
                    });
                    mapDataGridView.Rows.Add(row);
                }
                catch (Exception ex)
                {
                    log.Warn("Wpis " + entries[i] + " jest nieprawidłowy i zostanie pominięty.", ex);
                }
            }
            mapDataGridView.ResumeLayout();
        }

        /// <summary>
        /// Anuluje wprowadzone zmiany.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.No;
            Close();
        }

        /// <summary>
        /// Obsługuje zdarzenie wyświetlenia menu kontekstowego. Dezaktywuje niedostępne przyciski.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        void contextMenuStrip_Opened(object sender, EventArgs e)
        {
            Point position = mapDataGridView.PointToClient(Control.MousePosition);
            var hitTestResult = mapDataGridView.HitTest(position.X, position.Y);
            selectedRow = GetSelectedRow();
            removeToolStripMenuItem.Visible = selectedRow != null;
        }

        /// <summary>
        /// Zwraca zaznaczony wiersz
        /// </summary>
        /// <returns>Wiersz</returns>
        private DataGridViewRow GetSelectedRow()
        {
            if (mapDataGridView.SelectedRows.Count > 0)
                return mapDataGridView.SelectedRows[0];
            else if (mapDataGridView.SelectedCells.Count > 0)
                return mapDataGridView.Rows[mapDataGridView.SelectedCells[0].RowIndex];
            else
                return null;
        }

        /// <summary>
        /// Inicjalizuje rozmiar edytora ścieżki.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void mapDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = mapDataGridView.Rows[e.RowIndex];
            row.Height = 400;
        }

        /// <summary>
        /// Obsługuje zdarzenie zakończenia edycji komórki. Przywaraca domyślne rozmiary komórek. Wprowadza zmiany typu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void mapDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            mapDataGridView.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            var row = mapDataGridView.Rows[e.RowIndex];
            if (!string.IsNullOrEmpty(row.Cells[0].Value as string))
            {
                Type type = GetExpectedType(row.Cells[0].Value as String, nameColumn.VariablesDef);
                var cell = row.Cells[1];
                cell.Value = type.Name;
                cell.Tag = type;
            }
            if (mapDataGridView.Rows.OfType<DataGridViewRow>().Select(x => x.Cells[0].Value as string).Where(x => x == row.Cells[0].Value as string).Count() > 1)
                MessageBox.Show("Wpis z mapowaniem do tego pola juz znajduje się w tabeli. Usuń powtarzający się wppis.", " Błąd");
        }

        /// <summary>
        /// Zaznacza komórki po kliknięciu myszki.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        void mapDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            mapDataGridView.ClearSelection();
            if (e.RowIndex >= 0)
                mapDataGridView.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// Obsługuje zdarzenie wcisniecia klawisza del i usunięcia wiersza.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void mapDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            var indexes = mapDataGridView.SelectedCells.OfType<DataGridViewCell>().Select(x => x.RowIndex).OrderByDescending(x => x).Distinct().ToArray();
            if (indexes.Length != 0 && e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                foreach (int index in indexes)
                    mapDataGridView.Rows.RemoveAt(index);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie zmiany zaznaczenia. Uaktywani odpowiednie przyciski.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void mapDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            removeToolStripButton.Enabled = mapDataGridView.SelectedCells.Count > 0;
            int[] selectedRowIndex = mapDataGridView.SelectedCells.OfType<DataGridViewCell>().Select(x=>x.RowIndex).Distinct().OrderBy(x => x).ToArray();
            if (selectedRowIndex.Length != 0)
            {
                selectedRow = mapDataGridView.Rows[selectedRowIndex[selectedRowIndex.Length - 1]];
                foreach (var rowIndex in selectedRowIndex.Take(selectedRowIndex.Length - 1))
                    mapDataGridView.Rows[rowIndex].Selected = false;
            }
            else
                selectedRow = null;
        }

        /// <summary>
        /// Obsługuje zdarzenie usunięcia wiersza z tabeli.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = selectedRow.Index;
            mapDataGridView.Rows.Remove(selectedRow);
            if (mapDataGridView.Rows.Count > 0)
            {
                if (mapDataGridView.Rows.Count != index)
                    mapDataGridView.Rows[index].Selected = true;
                else
                    mapDataGridView.Rows[index - 1].Selected = true;
            }
        }

        /// <summary>
        /// Zatwierdza wprowadzone zmiany
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Instancja <see cref="System.EventArgs"/> zawierająca informacje o zdarzeniu.</param>
        private void save_Click(object sender, EventArgs e)
        {
            Map = mapDataGridView.Rows.OfType<DataGridViewRow>().Where(x => !String.IsNullOrEmpty(x.Cells[0].Value as string))
                .Select(x => x.Cells[0].Value + "=" + x.Cells[2].Value).ToArray();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Zwraca oczekiwany typ welej strony wyrażenia
        /// </summary>
        /// <param name="left">Lewa strona.</param>
        /// <param name="dic">Zmienne.</param>
        /// <returns>Typ</returns>
        internal static Type GetExpectedType(string left, IDictionary<string, Type> dic)
        {
            string[] path = left.Split('.');
            return GetTypeFromPath(path, 1, dic[path[0]]);
        }

        /// <summary>
        /// Zwraca typ ścieżki
        /// </summary>
        /// <param name="path">Ścieżka.</param>
        /// <param name="index">Indeks aktualnie rozpatrywanego stringu.</param>
        /// <param name="type">Typ.</param>
        /// <returns>Typ.</returns>
        internal static Type GetTypeFromPath(string[] path, int index, Type type)
        {
            if (index >= path.Length)
                return type;

            string member = path[index];
            PropertyInfo property = type.GetProperty(member);
            Type newType;
            if (property != null)
                newType = property.PropertyType;
            else
            {
                FieldInfo field = type.GetField(member);
                newType = field.FieldType;
            }
            return GetTypeFromPath(path, index + 1, newType);
        }
    }
}