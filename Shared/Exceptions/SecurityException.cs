using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.Serialization;

namespace SoftwareMind.Utils.Exceptions
{
    [Serializable]
    public class SecurityException : TranslatedApplicationException
    {
        [SecuritySafeCritical]
        protected SecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public SecurityException() : base(ExceptionCategory.ERROR) { }
        public SecurityException(string message) : base(ExceptionCategory.ERROR, message) { }
        public SecurityException(string message, Exception innerException) : base(ExceptionCategory.ERROR, message, innerException) { }
    }
}
