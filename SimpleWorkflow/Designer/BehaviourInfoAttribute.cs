using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.SimpleWorkflow.Behaviours;
using System.Drawing.Drawing2D;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed public class BehaviourInfoAttribute : Attribute
    {

        public BehaviourInfoAttribute(WFConnectorBehaviourType behaviourType, DashStyle dashStyle)
        {
            BehaviourType = behaviourType;
            DashStyle = dashStyle;
        }



        public WFConnectorBehaviourType BehaviourType { get; private set; }

        public DashStyle DashStyle { get; private set; }
    }
}
