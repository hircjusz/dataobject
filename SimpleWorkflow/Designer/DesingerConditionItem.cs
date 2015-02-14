using System;
using System.ComponentModel;
using System.Drawing.Design;
using SoftwareMind.SimpleWorkflow.Conditions;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa ułatwiająca edycję warunków w edytorze
    /// </summary>
    [Serializable]
    public class DesingerConditionItem
    {

        /// <summary>
        /// Przechowywuje instancję klasy wybranego typu, na niej ustawiane są właściwosci konfiguracyjne obiektu
        /// </summary>
        private IWFCondition condition;



        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="DesingerConditionItem"/>.
        /// </summary>
        public DesingerConditionItem()
        {
            Condition = new WFNullCondition();
        }



        /// <summary>
        /// Pozwala pobrać i ustawić konkretny egzemplarz warunku.
        /// </summary>
        /// <value>Warunek.</value>
        [Browsable(false)]
        public IWFCondition Condition
        {
            get
            {
                if (ConditionType == null)
                    return null;
                else if (condition != null && ConditionType == condition.GetType())
                    return condition;
                else
                    return condition = (IWFCondition)Activator.CreateInstance(ConditionType);
            }
            set
            {
                if (value == null)
                {
                    ConditionType = null;
                    condition = null;
                }
                else
                {
                    condition = value;
                    ConditionType = value.GetType();
                }
            }
        }

        /// <summary>
        /// Geter/seter dla desingera ułatwiajacy zmianę właściwosci warunku. Właściwość edytora.
        /// </summary>
        /// <value>Właściwosci warunku.</value>
        [Browsable(true)]
        [EditorAttribute(typeof(DetailsUIEditor), typeof(UITypeEditor))]
        public IWFCondition ConditionProperties
        {
            get
            {
                return Condition;
            }
            set
            {
                Condition = value;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawic typ obiektu. Właściwosc dla edytora.
        /// </summary>
        /// <value>Typ warunku.</value>
        [TypeConverter(typeof(TypeSelector))]
        [TypeSelectorInterfaceTypeAtribute(typeof(IWFCondition))]
        [DefaultValue(typeof(WFNullCondition))]
        public Type ConditionType { get; set; }
    }
}
