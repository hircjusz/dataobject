using System;

namespace SoftwareMind.SimpleWorkflow.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false, Inherited=true)]
    [Serializable]
    public class WFCustomSerializationAttribute : Attribute
    {
        public Type WFVariableSerializer { get; set; }

        public WFCustomSerializationAttribute(Type wFVariableSerializer)
        {
            if(typeof(IWFVariableSerializer).IsAssignableFrom(wFVariableSerializer) == false)
                throw new ArgumentException("Typ musi implementować IWFVariableSerializer");

            this.WFVariableSerializer = wFVariableSerializer;
        }
    }
}
