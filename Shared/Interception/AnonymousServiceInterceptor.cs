using System;
using System.Collections.Generic;
using System.Linq;
using MefContrib.Hosting.Interception;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using SoftwareMind.Shared.Infrastructure;
using SoftwareMind.Shared.Service;

namespace SoftwareMind.Shared.Interception
{
    public class AnonymousServiceInterceptor : IExportedValueInterceptor
    {
        private IUnityContainer _container;
        private ISessionWrapperProvider _provider;

        public AnonymousServiceInterceptor(IUnityContainer container, ISessionWrapperProvider provider)
        {
            this._container = container;
            this._provider = provider;
        }

        public object Intercept(object value)
        {
            var interfaces = value.GetType().GetInterfaces();
            var proxyInterface = interfaces.FirstOrDefault();
            var additionalInterfaces = interfaces.Skip(1).ToArray();

            var interceptor = new InterfaceInterceptor();

            IInterceptionBehavior[] behaviors = new IInterceptionBehavior[] {
                new SessionInjectionBehavior(this._provider)
            };

            var proxy = Microsoft.Practices.Unity.InterceptionExtension.Intercept.ThroughProxyWithAdditionalInterfaces(
                proxyInterface,
                value,
                interceptor,
                behaviors,
                additionalInterfaces
            );

            return proxy;
        }
    }

    internal class SessionInjectionBehavior : IInterceptionBehavior
    {
        private ISessionWrapperProvider _provider;

        public SessionInjectionBehavior(ISessionWrapperProvider provider)
        {
            this._provider = provider;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            using (var wrapper = this._provider.GetWrapper())
            {
                ((IAnonymousService)input.Target).Session = wrapper.Session;
                IMethodReturn result = getNext()(input, getNext);
                ((IAnonymousService)input.Target).Session = null;

                return result;
            }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return new[] { typeof(IAnonymousService) };
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
