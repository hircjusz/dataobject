using System;
using System.Drawing;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    /// <summary>
    /// Atrybut opisujący podstawowe właściwości węzła edytorze. Na jego podstawie tworzona jest paleta z dostępnymi aktywnościami.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Serializable]
    public sealed class NodePropertiesAttribute : Attribute
    {
        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="NodePropertiesAttribute"/>.
        /// </summary>
        /// <param name="defaultName">Domyślna nazwa.</param>
        /// <param name="large">Nazwa dużego obrazu w assembly.</param>
        /// <param name="small">Nazwa małego obrazu w assembly.</param>
        /// <param name="type"></param>
        public NodePropertiesAttribute(string defaultName, string large, string small, Type type)
        {
            DefaultName = defaultName;
            LargeImage = InvokeProperty(large, type);
            SmallImage = InvokeProperty(small, type);
        }

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="NodePropertiesAttribute"/>.
        /// </summary>
        /// <param name="defaultName">Domyślna nazwa.</param>
        /// <param name="large">Duży obraz.</param>
        /// <param name="type">Typ, z którego zostanie pobrany obraz.</param>
        public NodePropertiesAttribute(string defaultName, string large, Type type)
            : this(defaultName, large, large, type)
        {
        }

        /// <summary>
        /// Pozwala pobrać i ustawić domyślną wartosć węzła
        /// </summary>
        /// <value>The default name.</value>
        public string DefaultName
        {
            get;
            private set;
        }

        /// <summary>
        /// Pozwala pobrać i ustawić duży obraz(32x32).
        /// </summary>
        /// <value>Duży obraz.</value>
        public Image LargeImage
        {
            get;
            private set;
        }

        /// <summary>
        /// Pozwala pobrać i ustawić małą ikonkę(16x16).
        /// </summary>
        /// <value>Mała ikonka.</value>
        public Image SmallImage
        {
            get;
            private set;
        }




        /// <summary>
        /// Pobiera obraz z property poprzez wywołanie go.
        /// </summary>
        /// <param name="PropertyName">Nazwa property.</param>
        /// <param name="type">Typ, z którego zostanie pobrany obraz.</param>
        /// <returns>Obraz.</returns>
        private static Image InvokeProperty(string PropertyName, Type type)
        {
            var propInfo = type.GetProperty(PropertyName);
            var result = propInfo.GetGetMethod().Invoke(null, new object[0]);
            return result as Image;
        }
    }
}
