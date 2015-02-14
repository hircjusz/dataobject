using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using SoftwareMind.Shared.Config;

namespace SoftwareMind.Shared.Infrastructure.Config
{
    public class ConfigurationExportProvider : ExportProvider
    {
        private readonly IUnityContainer _container;

        public ConfigurationExportProvider(IUnityContainer container)
        {
            this._container = container;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition importDefinition, AtomicComposition atomicComposition)
        {
            ContractBasedImportDefinition contractImportDefinition = importDefinition as ContractBasedImportDefinition;
            if (contractImportDefinition == null || contractImportDefinition.ContractName != contractImportDefinition.RequiredTypeIdentity)
            {
                return null;
            }

            Type type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(contractImportDefinition.RequiredTypeIdentity, false)).FirstOrDefault(t => t != null);
            if (type == null)
            {
                return null;
            }

            ConfigurationAttribute attribute = type.GetCustomAttributes(typeof(ConfigurationAttribute), false).OfType<ConfigurationAttribute>().SingleOrDefault();
            if (attribute == null)
            {
                return null;
            }

            return new[] { new Export(importDefinition.ContractName, () => this.GetValue(type, attribute)) };
        }

        protected object GetValue(Type proxyInterface, ConfigurationAttribute attribute)
        {
            var interceptor = new VirtualMethodInterceptor();

            IInterceptionBehavior[] behaviors = new IInterceptionBehavior[] {
                new ConfigurationProxyBehavior(attribute.ConfigurationType, attribute.Prefix)
            };

            var proxy = Intercept.NewInstanceWithAdditionalInterfaces<object>(
                interceptor,
                behaviors,
                new[] { proxyInterface }
            );

            return proxy;
        }
    }
}