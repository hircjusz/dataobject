using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DataObjects.NET;
using log4net;
using MefContrib.Hosting.Interception;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using SoftwareMind.Shared.Infrastructure;
using SoftwareMind.Shared.Service;

namespace SoftwareMind.Shared.Interception
{
    public class ServiceInterceptor : IExportedValueInterceptor
    {
        private IUnityContainer _container;

        public ServiceInterceptor(IUnityContainer container)
        {
            this._container = container;
        }

        public object Intercept(object value)
        {
            var interfaces = value.GetType().GetInterfaces();
            var proxyInterface = interfaces.FirstOrDefault();
            var additionalInterfaces = interfaces.Skip(1).ToArray();

            var interceptor = new InterfaceInterceptor();

            IInterceptionBehavior[] behaviors = new IInterceptionBehavior[] {
                new TransactionInjectionBehavior()
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

    internal class TransactionInjectionBehavior : IInterceptionBehavior
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TransactionInjectionBehavior));

        [DebuggerStepThrough]
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            log.Debug("Invoking method");

            MethodBase method = input.MethodBase;
            MethodBase methodImpl = null;
            IService target = (IService)input.Target;
            IContext context = target.Context;

            if (target != null && method.DeclaringType != null && method.DeclaringType.IsInterface && method is MethodInfo)
            {
                var interfaceDef = method.DeclaringType.IsGenericType ? method.DeclaringType.GetGenericTypeDefinition() : method.DeclaringType;
                var methodDef = (MethodInfo)(method.IsGenericMethod ? ((MethodInfo) method).GetGenericMethodDefinition() : method);
                var typeDef = target.GetType().IsGenericType ? target.GetType().GetGenericTypeDefinition() : target.GetType();

                var map = typeDef.GetInterfaceMap(interfaceDef);
                var index = Array.IndexOf(map.InterfaceMethods, methodDef);

                if (index != -1)
                {
                    methodImpl = map.TargetMethods[index];
                }
            }

            bool isTransactional = !method.GetCustomAttributes(typeof(NonTransactionalAttribute), false).Any()
                && (methodImpl == null || !methodImpl.GetCustomAttributes(typeof(NonTransactionalAttribute), false).Any());

            IMethodReturn result = null;

            try
            {
                if (isTransactional)
                {
                    log.Debug("Method is transactional");

                    TransactionController tc = new TransactionController(context.Session, TransactionMode.NewTransactionRequired);
                    try
                    {
                        log.Debug("Inoking method");
                        result = getNext()(input, getNext);
                        log.Debug("Method invoked");

                        if (result != null && result.Exception != null)
                        {
                            tc.Rollback(result.Exception, true);
                        }
                        else
                        {
                            tc.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        log.Debug("Exception occured", e);
                        tc.Rollback(e, true);
                        throw;
                    }
                }
                else
                {
                    log.Debug("Method is not transactional");

                    log.Debug("Inoking method");
                    result = getNext()(input, getNext);
                    log.Debug("Method invoked");
                }

                if (result == null || result.Exception == null)
                {
                    log.Debug("No exception occured");
                }
                else
                {
                    log.Warn(string.Format("Exception occured in {0}.{1} method", method.DeclaringType.Name, method.Name), result.Exception);
                    result.Exception = this.ExtractException(result.Exception);
                }
            }
            catch (Exception ex)
            {
                log.Warn(string.Format("Exception occured in {0}.{1} method", method.DeclaringType.Name, method.Name), ex);
                result = input.CreateExceptionMethodReturn(this.ExtractException(ex));
            }

            return result;
        }


        private Exception ExtractException(Exception exception)
        {
            // HACK: this is shitty
            if (exception.GetType() == typeof(Exception) && exception.Message == "Error during invocation of OnCreate")
            {
                return exception.InnerException;
            }

            return exception;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return new[] { typeof(IService) };
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
