using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// konwerter pozwalajacy wybrać metody.
    /// </summary>
    public class MethodSelector : SelectorConverter<MethodInfo>
    {
        /// <summary>
        /// Zwraca listę typów
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <returns>Lista typów.</returns>
        protected override Dictionary<string, MethodInfo> GetList(ITypeDescriptorContext context)
        {
            IMethodInfoProvider provider = context.Instance as IMethodInfoProvider;
            if (provider == null)
                throw new InvalidOperationException("Klasa musi impelenować interferjs IMethodInfoProvider.");
            Dictionary<string, MethodInfo> cache = null;
            Type type = provider.GetTypeToEnum();
            if (type == null)
                cache = new Dictionary<string, MethodInfo>();
            else
            {
                Type contract = type.GetInterfaces().
                    Where(x => x.GetCustomAttributes(typeof(ServiceContractAttribute), true).FirstOrDefault() != null).
                    FirstOrDefault();
                var methods = contract.GetMethods().Where(x => x.IsPublic);

                cache = methods.ToDictionary(x => MethodBaseHelper.GenerateUniqueMethodName(x), x => x);
            }
            cache.Add("(null)", null);

            return cache;
        }
    }
}
