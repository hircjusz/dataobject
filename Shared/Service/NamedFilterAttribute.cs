using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.Shared.Service
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class NamedFilterAttribute : Attribute
    {
        public Type Type { get; set; }

        public NamedFilterAttribute(Type type)
        {
            this.Type = type;
        }
    }
}
