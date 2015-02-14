using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using MefContrib.Hosting.Filter;
using MefContrib.Hosting.Interception;
using MefContrib.Hosting.Interception.Configuration;

namespace SoftwareMind.Shared.Mef
{
    public abstract class AbstractWezyrCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly InterceptingCatalog _interceptingCatalog;
        private readonly AggregateCatalog _innerCatalog = new AggregateCatalog();

        protected virtual IEnumerable<Assembly> Assemblies
        {
            get { yield break; }
        }

        public ICollection<ComposablePartCatalog> Catalogs
        {
            get { return this._innerCatalog.Catalogs; }
        }

        protected AbstractWezyrCatalog()
        {
            foreach (var assembly in this.Assemblies)
            {
                this.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            var cfg = new InterceptionConfiguration().AddHandler(new FilteringPartHandler(this.Filter));
            this._interceptingCatalog = new InterceptingCatalog(this._innerCatalog, cfg);
        }

        public virtual bool Filter(ComposablePartDefinition part)
        {
            return true;
        }

        /// <summary>
        /// Gets the part definitions of the catalog.
        /// </summary>
        /// <value>A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the <see cref="FilteringCatalog"/>.</value>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return this._interceptingCatalog.Parts; }
        }

        /// <summary>
        /// Method which can filter exports for given <see cref="ImportDefinition"/> or produce new exports.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> instance.</param>
        /// <returns>
        /// A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.
        /// </returns>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return this._interceptingCatalog.GetExports(definition);
        }

        #region INotifyComposablePartCatalogChanged Implementation

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add { this._interceptingCatalog.Changed += value; }
            remove { this._interceptingCatalog.Changed -= value; }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add { this._interceptingCatalog.Changing += value; }
            remove { this._interceptingCatalog.Changing -= value; }
        }

        #endregion

    }
}
