using System;

namespace SoftwareMind.SimpleWorkflow.Exceptions
{
    [Serializable]
    public class WFInvalidStateException : Exception
    {
        public string StepCode { get; set; }

        public WFInvalidStateException(string code, string message) : base(message)
        {
            this.StepCode = code;
        }
    }
}
