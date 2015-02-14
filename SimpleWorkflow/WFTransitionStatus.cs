using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class WFTransitionStatus : IComparable, IComparable<WFTransitionStatus>
    {
        public string Status { get; protected set; }

        internal protected WFTransitionStatus()
        {

        }

        public static WFTransitionStatus Successful
        {
            get
            {
                return new WFTransitionStatus() { Status = "Successful" };
            }
        }

        public static WFTransitionStatus ConnectorNotAvailable
        {
            get
            {
                return new WFTransitionStatus() { Status = "ConnectorNotAvailable" };
            }
        }

        public static WFTransitionStatus Error
        {
            get
            {
                return new WFTransitionStatus() { Status = "Error" };
            }
        }

        #region IComparable<WFTransitionStatus> Members

        public int CompareTo(WFTransitionStatus other)
        {
            return StringComparer.InvariantCulture.Compare(this.Status, other.Status);

        }

        #endregion

        public override string ToString()
        {
            return this.Status;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            WFTransitionStatus other = obj as WFTransitionStatus;

            if (other == null)
            {
                return -1;
            }
            else
            {
                return StringComparer.InvariantCulture.Compare(this.Status, other.Status);

            }
        }

        #endregion
    }
}
