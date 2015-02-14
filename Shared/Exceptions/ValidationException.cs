using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace SoftwareMind.Utils.Exceptions
{
    [Serializable]
    public class ValidationException : TranslatedApplicationException
    {
        [SecuritySafeCritical]
        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        
        public ValidationException() : base(ExceptionCategory.INFO) { }
        
        public ValidationException(string message) : base(ExceptionCategory.INFO, message) { }
        
        public ValidationException(string message, string code) : base(ExceptionCategory.INFO, message)
        {
            this.Data.Add("Code", code);
        }
        
        public ValidationException(string message, Exception innerException) : base(ExceptionCategory.INFO, message, innerException) { }
    }
}
