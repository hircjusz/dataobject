using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Edytor wyświetlający PropertyGrida. Jest wykorzystywana do edycji właściwości egzemplarzy polis i warunków.
    /// </summary>
    [Serializable]
    public class DetailsUIEditor : UITypeEditor
    {


        /// <summary>
        /// Wyświetla edytor wartośći.
        /// </summary>
        /// <param name="context">Kontekst wywołania</param>
        /// <param name="provider">Dostarczajacy dostęp do serwisów</param>
        /// <param name="value">Obiekt do edycji.</param>
        /// <returns>
        /// Zwracany jest egzemplarz wejsciowy obiektu, ze zmienionymi właściwosciami.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService wfes = provider.GetService(typeof(IWindowsFormsEditorService)) as
                IWindowsFormsEditorService;

            PropertyGrid propertyGrid = new PropertyGrid();
            propertyGrid.SelectedObject = value;
            propertyGrid.Width = 300;
            propertyGrid.Height = 400;

            wfes.DropDownControl(propertyGrid);
            return value;
        }

        /// <summary>
        /// Zwraca styl w jakim zostanie wyświetlony edytor
        /// </summary>
        /// <param name="context">Contekst</param>
        /// <returns>
        /// Styl
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
