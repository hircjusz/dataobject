using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoftwareMind.SimpleWorkflow.Attributes;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa konwerter dla typu zmiennej. Zawiera listę rozwijana z typami prostymi oraz typami implementującymi IDataObject.
    /// </summary>
    [Serializable]
    public class VariableTypeSelector : TypeConverter
    {

        /// <summary>
        /// Cache
        /// </summary>
        private static List<string> cache = new List<string>();
        /// <summary>
        /// Słownik typów do konwersji.
        /// </summary>
        private static Dictionary<string, Type> converterDictionary;
        /// <summary>
        /// Lista typów prostych
        /// </summary>
        private static Type[] simpleTypes = GetSimpleTypes();




        /// <summary>
        /// Czy wartosć moze być konwertowana z zadanego typu.
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <param name="sourceType">Typ docelowy</param>
        /// <returns>
        /// Zawsze prawda
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        /// <summary>
        /// Czy wartosć moze być konwertowana do zadanego typu.
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <param name="destinationType">Typ docelowy</param>
        /// <returns>
        /// Zawsze prawda
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        /// <summary>
        /// Czyści cache
        /// </summary>
        public static void ClearCache()
        {
            cache.Clear();
        }

        /// <summary>
        /// Konwertuje zadane wartości
        /// </summary>
        /// <param name="context">Kontekst</param>
        /// <param name="culture">Informacje o kontekscie kulturowym</param>
        /// <param name="value">Wartosc do konwersji.</param>
        /// <returns>
        /// <see cref="T:System.Object"/> reprezentujacy skonwertowaną wartość.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                Type result;
                if (!converterDictionary.TryGetValue((string)value, out result))
                    throw new ArgumentException("Typ o podanej nazwie nie został wczytany.");
                return result;
            }
            return value;
        }

        /// <summary>
        /// Konwertuje zadane wartości
        /// </summary>
        /// <param name="context">Kontekst</param>
        /// <param name="culture">Informacje o kontekscie kulturowym</param>
        /// <param name="value">Wartosc do konwersji.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> Typ wyjściowy.</param>
        /// <returns>
        /// <see cref="T:System.Object"/> reprezentujacy skonwertowaną wartość.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }
            else if (destinationType == typeof(Type))
            {
                return Type.GetType((string)value);
            }
            return value;
        }

        /// <summary>
        /// Zwraca listę wartosci domyślnych
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <returns>
        /// Wartosć domyślna.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetList());
        }

        /// <summary>
        /// Zwraca wartość prawda/fałsz informującą o tym czy watości mogą być wprowadzane ręcznie przez urzytkownika.
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <returns>
        /// Zawsze fałsz.
        /// </returns>
        public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Zwraca wartosć prawda fałsz informującą o tym czy obsługiwane są wyłącznie wartosci z listy rozwijanej.
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <returns>
        /// Zwraca zawszę prawdę.
        /// </returns>
        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Cachuje elementy.
        /// </summary>
        public static void BuildCache()
        {
        }


        /// <summary>
        /// Lista typów
        /// </summary>
        /// <returns>Lista typó domyslnych.</returns>
        private static List<string> GetList( )
        {
            if (cache.Count == 0)
            {
                var typesInAppDomain = TypeHelper.GetAllTypes()
                    .Where(x => x.IsPublic && x.GetCustomAttributes(typeof(WFCustomSerializationAttribute), true).Count() > 0).ToArray();
                var allType = simpleTypes.Union(typesInAppDomain).Distinct();
                converterDictionary = TypeHelper.GetAllTypes()
                    .ToLookup(x => x.FullName, x => x).ToDictionary(x => x.Key, x => x.First());

                foreach (Type type in allType)
                    cache.Add(type.FullName);
            }
            return cache;
        }

        /// <summary>
        /// Zwraca listę typów prostych
        /// </summary>
        /// <returns></returns>
        private static Type[] GetSimpleTypes()
        {
            return WFVariableDef.GetSimpleTypes();
        }
    }
}
