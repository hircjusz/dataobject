using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace SoftwareMind.Utils.Exceptions
{
    [Serializable]
    public class AuthorizationException : SecurityException
    {
        [SecuritySafeCritical]
        protected AuthorizationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public AuthorizationException() : base() { }
        public AuthorizationException(string message) : base(message) { }
        public AuthorizationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
