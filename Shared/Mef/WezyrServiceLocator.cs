using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace SoftwareMind.Shared.Mef
{
    public class WezyrServiceLocator : IWezyrServiceLocator
    {
        private CompositionContainer _container;

        public WezyrServiceLocator(ComposablePartCatalog kernel, params ExportProvider[] exportProvider)
        {
            this._container = new CompositionContainer(kernel, exportProvider);
        }

        private object GetService(Type serviceType, string key)
        {
            Export export = this._container.GetExports(
                new ContractBasedImportDefinition(
                    key,
                    AttributedModelServices.GetTypeIdentity(serviceType),
                    null,
                    ImportCardinality.ZeroOrMore,
                    false,
                    false,
                    CreationPolicy.Any
                )
            ).FirstOrDefault();

            return export == null ? null : export.Value;
        }

        private IEnumerable<object> GetAllServices(Type serviceType)
        {
            IEnumerable<Export> exports = this._container.GetExports(
                new ContractBasedImportDefinition(
                    AttributedModelServices.GetContractName(serviceType),
                    AttributedModelServices.GetTypeIdentity(serviceType),
                    null,
                    ImportCardinality.ZeroOrMore,
                    false,
                    false,
                    CreationPolicy.Any
                )
            );

            return exports.Select(e => e.Value);
        }

        public object GetService(Type serviceType)
        {
            return this.GetService(serviceType, AttributedModelServices.GetContractName(serviceType));
        }

        public object GetInstance(Type serviceType)
        {
            return this.GetService(serviceType, AttributedModelServices.GetContractName(serviceType));
        }

        public object GetInstance(Type serviceType, string key)
        {
            return this.GetService(serviceType, key);
        }

        public object GetInstance(Type serviceType, params KeyValuePair<string, object>[] metaData)
        {
            return this.GetService(serviceType, ExportWithMetadataAttribute.GetContractName(serviceType, metaData));
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return this.GetAllServices(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return (TService)this.GetService(typeof(TService), AttributedModelServices.GetContractName(typeof(TService)));
        }

        public TService GetInstance<TService>(string key)
        {
            return (TService)this.GetService(typeof(TService), key);
        }

        public TService GetInstance<TService>(params KeyValuePair<string, object>[] metaData)
        {
            return (TService)this.GetService(typeof(TService), ExportWithMetadataAttribute.GetContractName(typeof(TService), metaData));
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return this.GetAllServices(typeof(TService)).Cast<TService>();
        }

        public Lazy<TService> GetLazy<TService>()
        {
            return new Lazy<TService>(() => this.GetInstance<TService>());
        }

        public Lazy<TService> GetLazy<TService>(string key)
        {
            return new Lazy<TService>(() => this.GetInstance<TService>(key));
        }

        public Lazy<TService> GetLazy<TService>(params KeyValuePair<string, object>[] metaData)
        {
            return new Lazy<TService>(() => this.GetInstance<TService>(metaData));
        }
    }
}