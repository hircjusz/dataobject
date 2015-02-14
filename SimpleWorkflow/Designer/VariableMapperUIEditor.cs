using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Edytor mapowania zmiennych do parametrów metody
    /// </summary>
    [Serializable]
    public class VariableMapperUIEditor : UITypeEditor
    {


        /// <summary>
        /// Wyświetla edytor wartośći.
        /// </summary>
        /// <param name="context">Kontekst wywołania</param>
        /// <param name="provider">Dostarczajacy dostęp do serwisów</param>
        /// <param name="value">Obiekt do edycji.</param>
        /// <returns>
        /// Zwracany jest obiekt wejsciowy.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IMethodInfoProvider mprovider = context.Instance as IMethodInfoProvider;
            if (mprovider == null)
                throw new InvalidOperationException("Klasa musi impelenować interferjs IMethodInfoProvider.");
            MethodBase method = mprovider.Method; 

            using (VariableMapEditorDialog ved = new VariableMapEditorDialog())
            {
                VariableMapper vm = value as VariableMapper;
                if (vm == null)
                    throw new InvalidOperationException("Typ może tylko edytować VariableMapper");
                else
                {
                    ved.VariableMapper = vm;
                    WFVariableDefContainer container = context.Instance as WFVariableDefContainer;
                    ved.VariablesDefCollection = container.VariableDefs.GetTypeDictionary();
                    ved.Method = method;
                    ved.Map = vm.VariableMap;
                    if (ved.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        vm.VariableMap = ved.Map;
                }
                return value;
            }
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
