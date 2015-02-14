using System;

namespace SoftwareMind.SimpleWorkflow.Actions
{
    public interface IWfActivityActionParameters : ICloneable, IWFTemplateElement
    {
        void Validate();
    }
}
