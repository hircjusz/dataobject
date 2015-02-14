using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa ułatwiająca edycję polis w edytorze
    /// </summary>
    [Serializable]
    public class DesingerPolicyItem
    {

        /// <summary>
        /// 
        /// </summary>
        private IWFPolicyRule policy;



        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="DesingerPolicyItem"/>.
        /// </summary>
        public DesingerPolicyItem()
        {
            Policy = new WFNullPolicyRule();
        }



        /// <summary>
        /// Pozwala pobrać i ustawić konkretny egzemplarz polisy.
        /// </summary>
        /// <value>Polisa.</value>
        [Browsable(false)]
        public IWFPolicyRule Policy
        {
            get
            {
                if (PolicyType == null)
                    return null;
                else if (policy != null && PolicyType == policy.GetType())
                    return policy;
                else
                    return policy = (IWFPolicyRule)Activator.CreateInstance(PolicyType);
            }
            set
            {
                if (value == null)
                {
                    PolicyType = null;
                    policy = null;
                }
                else
                {
                    policy = value;
                    PolicyType = value.GetType();
                }
            }
        }

        /// <summary>
        /// Geter/seter dla desingera ułatwiajacy zmianę właściwosci polisy. Właściwosć dla edytora.
        /// </summary>
        /// <value>Właściwosci polisy.</value>
        [Browsable(true)]
        [EditorAttribute(typeof(DetailsUIEditor), typeof(UITypeEditor))]
        public IWFPolicyRule PolicyProperties
        {
            get
            {
                return Policy;
            }
            set
            {
                Policy = value;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawic typ obiektu. Właściwosc dla edytora.
        /// </summary>
        /// <value>Typ polisy.</value>
        [TypeConverter(typeof(TypeSelector))]
        [TypeSelectorInterfaceTypeAtribute(typeof(IWFPolicyRule))]
        [DefaultValue(typeof(WFNullPolicyRule))]
        public Type PolicyType { get; set; }
    }
}
