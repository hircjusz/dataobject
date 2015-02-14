using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace SoftwareMind.Shared.Mef.ExportProviders
{
    public class InstanceExportProvider : ExportProvider
    {
        private InstanceExportDefinition exportDefinition;

        public InstanceExportProvider(object instance, Type serviceType = null, IDictionary<string, object> metadata = null)
        {
            this.exportDefinition = new InstanceExportDefinition(
                serviceType ?? instance.GetType(),
                instance,
                metadata ?? new Dictionary<string, object>()
            );
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            if (definition.ContractName == this.exportDefinition.ContractName)
            {
                yield return new Export(this.exportDefinition.ContractName, this.exportDefinition.Metadata, this.ExportedValueGetter);
            }
        }

        private object ExportedValueGetter()
        {
            return this.exportDefinition.Instance;
        }

        private class InstanceExportDefinition : ExportDefinition
        {
            public InstanceExportDefinition(Type contract, object instance, IDictionary<string, object> metadata)
                : base(contract.FullName, metadata)
            {
                this.Instance = instance;
            }

            public object Instance { get; private set; }
        }
    }
}