using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Konwerter Typów na string i odwrotnie.Generuję listę rozwijaną z interfejsami znaleionymi w AppDomain.
    /// Wymaga użycia atrybutu TypeSelectorInterfaceType do wskazania typu interfejsu z jakiego obiekty mają implemwntować
    /// oraz DefaultValue wskazującego domyslną implementację.
    /// </summary>
    [Serializable]
    public class TypeSelector : SelectorConverter<Type>
    {
        /// <summary>
        /// Domyslny typ
        /// </summary>
        private Type defaultType;
        /// <summary>
        /// Cache
        /// </summary>
        private static ConcurrentDictionary<Type, Dictionary<string, Type>> cache = new ConcurrentDictionary<Type, Dictionary<string, Type>>();

        /// <summary>
        /// Czyści cache
        /// </summary>
        public static void ClearCache()
        {
            cache.Clear();
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
            var atr = context.PropertyDescriptor.Attributes.OfType<TypeSelectorInterfaceTypeAtribute>().FirstOrDefault();
            if (atr == null)
                throw new InvalidOperationException("Konwerter TypeSelector wymaga użycia również atrybutu TypeSelectorInterfaceType");

            Dictionary<string, Type> list;
            cache.TryGetValue(atr.Type, out list);

            if (list == null)
            {
                 var artd = context.PropertyDescriptor.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
                if(artd == null)
                    throw new InvalidOperationException("Konwerter TypeSelector wymaga użycia również atrybutu DefaultValue");

                defaultType = (Type)artd.Value;
                Type type = atr.Type;

                Type[] typesInAppDomain = TypeHelper.GetAllTypes().ToArray();

                Type[] types = typesInAppDomain.
                    Where(x => type.IsAssignableFrom(x) && !x.IsAbstract && x.IsClass).Distinct().ToArray();
                types = types.GroupBy(x => x.FullName).Select(x => x.First()).ToArray();
                if (types.Select(x => x.Name).Distinct().Count() == types.Length)
                    list = types.ToDictionary(x => x.Name, x => x);
                else
                    list = types.ToDictionary(x => x.FullName, x => x);
                if (defaultType == null)
                    list.Add("(null)", null);
                cache[atr.Type] = list;
            }
            return list;
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
            var atr = context.PropertyDescriptor.Attributes.OfType<TypeSelectorInterfaceTypeAtribute>().FirstOrDefault();
            if (atr == null)
                return false;
            return true;
        }


    }
}
