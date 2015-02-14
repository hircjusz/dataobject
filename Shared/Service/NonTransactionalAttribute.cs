using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.Shared.Service
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NonTransactionalAttribute : Attribute
    {
    }
}
