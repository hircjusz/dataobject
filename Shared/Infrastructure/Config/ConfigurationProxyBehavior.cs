using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;
using SoftwareMind.Shared.Config;

namespace SoftwareMind.Shared.Infrastructure.Config
{
    internal class ConfigurationProxyBehavior : IInterceptionBehavior
    {
        private readonly ConfigurationWrapper _wrapper;

        internal ConfigurationProxyBehavior(ConfigurationType configurationType, string prefix)
        {
            NameValueCollection config;
            switch (configurationType)
            {
                case ConfigurationType.Application:
                    config = ConfigurationManager.AppSettings;
                    break;
                case ConfigurationType.ConnectionStrings:
                    throw new NotImplementedException();
                    //config = ConfigurationManager.ConnectionStrings;
                    //break;
                default:
                    throw new ArgumentOutOfRangeException("configurationType");
            }

            this._wrapper = new ConfigurationWrapper(config, prefix);
        }

        private PropertyInfo FindProperty(MethodBase method)
        {
            if (!method.IsSpecialName)
            {
                return null;
            }
            return method.DeclaringType.GetProperty(method.Name.Substring(4), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            //PropertyInfo property = this.FindProperty(input.MethodBase);

            MethodBase method = input.MethodBase;
            string methodName = method.IsSpecialName ? method.Name.Substring(4) : method.Name;
            DefaultValueAttribute defaultValue = method.DeclaringType.GetProperty(methodName).GetCustomAttributes(typeof(DefaultValueAttribute), true).Cast<DefaultValueAttribute>().SingleOrDefault();

            object result = this._wrapper.Get(((MethodInfo)method).ReturnType, methodName, defaultValue);

            return input.CreateMethodReturn(result);
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}