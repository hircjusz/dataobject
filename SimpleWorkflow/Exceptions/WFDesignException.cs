using System;

namespace SoftwareMind.SimpleWorkflow.Exceptions
{
    [Serializable]
    public class WFDesignException : Exception
    {
        public String Code { get; private set; }

        public WFDesignException(string message, string code = null) : base(message)
        {
            Code = code;
        }
    }
}
