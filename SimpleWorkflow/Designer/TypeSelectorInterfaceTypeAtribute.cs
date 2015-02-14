using System;

namespace SoftwareMind.SimpleWorkflow.Designer
{

    /// <summary>
    /// Atrybut wskazujący typ interfejsu, który zostanie wykorzystany przy przeszukiwaniu typów przez klasę TypeSelector
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class TypeSelectorInterfaceTypeAtribute : Attribute
    {
        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="TypeSelectorInterfaceTypeAtribute"/>.
        /// </summary>
        /// <param name="type">Typ.</param>
        public TypeSelectorInterfaceTypeAtribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Pozwala pobrać typ
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }
    }
}
