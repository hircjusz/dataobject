
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    namespace SoftwareMind.SimpleWorkflow.Designer
    {
        /// <summary>
        /// Konwerter typów serwisów na string i odwrotnie.Generuję listę rozwijaną z interfejsami znaleionymi w AppDomain.
        /// </summary>
        [Serializable]
        public class ServiceTypeSelector : SelectorConverter<Type>
        {
            /// <summary>
            /// Cache
            /// </summary>
            private static Dictionary<string, Type> cache;

            /// <summary>
            /// Czyści cache
            /// </summary>
            public static void ClearCache()
            {
                cache = null;
            }

            /// <summary>
            /// Zwraca listę typów
            /// </summary>
            /// <param name="context">Kontekst.</param>
            /// <returns>Lista typów.</returns>
            /// <exception cref="InvalidOperationException">Jest rzucany, gdy właściwosć nie jest również opatrzona atrybutami
            /// TypeSelectorInterfaceType i DefaultValue</exception>
            protected override Dictionary<string, Type> GetList(ITypeDescriptorContext context)
            {
                if (cache == null)
                {
                    var typesInAppDomain = TypeHelper.GetAllTypes().ToArray();

                    Type type = typeof(System.ServiceModel.ClientBase<>);
                    Type[] types = typesInAppDomain.
                        Where(x => x.BaseType != null && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == type && !x.IsAbstract && x.IsClass && x.IsPublic).Distinct().ToArray();

                    if (types.Select(x => x.Name).Distinct().Count() == types.Length)
                        cache = types.ToDictionary(x => x.Name, x => x);
                    else
                    {
                        types = types.GroupBy(x => x.FullName).Select(x => x.First()).ToArray();
                        cache = types.ToDictionary(x => x.FullName, x => x);
                    }
                    cache.Add("(null)", null);
                }
                return cache;
            }

            /// <summary>
            /// Zwraca true jeśli odpowienie atrybuty są przypisane do właściwosci- konwerter potrafi wygenerować listę.
            /// </summary>
            /// <param name="context">Kontekst</param>
            /// <returns>
            /// Zawsze prawdę jeśli wskazane atrybuty są przypisane do obiektu.
            /// </returns>
            public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
            {
                return true;
            }
        }
    }

}
