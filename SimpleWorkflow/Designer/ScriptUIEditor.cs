using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Edytor skryptów. Po edycji skryptu przeprowadza jego walidacje.
    /// </summary>
    [Serializable]
    public class ScriptUIEditor : UITypeEditor
    {
        /// <summary>
        /// Wyświetla edytor wartośći.
        /// </summary>
        /// <param name="context">Kontekst wywołania</param>
        /// <param name="provider">Dostarczajacy dostęp do serwisów</param>
        /// <param name="value">Obiekt do edycji.</param>
        /// <returns>
        /// Zwracany jest string reprezentujący string. Przed jego zwróceniem dokonywana jest walidacja. W przypadku błedu wyświetlany
        /// jest stosowny komunikat.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService wfes = provider.GetService(typeof(IWindowsFormsEditorService)) as
                IWindowsFormsEditorService;

            WFVariableDefCollection vc;
            if (typeof(WFVariableDefContainer).IsAssignableFrom(context.Instance.GetType()))
                vc = GetActivityVariables(context.Instance as WFVariableDefContainer);
            else if (context.Instance is WFConnector)
                vc = (context.Instance as WFConnector).Source.Process.VariableDefs;
            else
                vc = new WFVariableDefCollection();

            using (ScriptEditorDialog se = new ScriptEditorDialog())
            {
                se.Script = (string)value;
                se.VariableCollection = vc;
                se.ShowDialog();
                return se.Script;
            }
        }

        private static WFVariableDefCollection GetActivityVariables(WFVariableDefContainer defs)
        {
            WFVariableDefCollection vc;
            vc = new WFVariableDefCollection();
            if (defs is WFActivityBase)
            {
                WFActivityBase activity = defs as WFActivityBase;
                vc.Add(activity.Process.VariableDefs);
            }
            vc.Add(defs.VariableDefs);
            return vc;
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
            return UITypeEditorEditStyle.Modal;
        }
    }
}
