using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    [Serializable]
    public abstract class SelectorConverter<T> : System.ComponentModel.TypeConverter where T : class
    {
        /// <summary>
        /// Zwraca listę typów
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <returns>Lista typów.</returns>
        /// <exception cref="InvalidOperationException">Jest rzucany, gdy właściwosć nie jest również opatrzona atrybutami
        /// TypeSelectorInterfaceType i DefaultValue</exception>
        protected abstract Dictionary<string, T> GetList(ITypeDescriptorContext context);

        /// <summary>
        /// Zwraca true jeśli odpowienie atrybuty są przypisane do właściwosci- konwerter potrafi wygenerować listę.
        /// </summary>
        /// <param name="context">Kontekst</param>
        /// <returns>
        /// Zawsze prawda
        /// </returns>
        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Zwraca wartosć prawda/fałsz informującą o tym czy obsługiwane są wyłącznie wartosci z listy rozwijanej.
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <returns>
        /// Zwraca zawszę prawdę.
        /// </returns>
        public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
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
            return new StandardValuesCollection(GetList(context).Keys);
        }

        /// <summary>
        /// Czy wartosć moze być konwertowana z zadanego typu.
        /// </summary>
        /// <param name="context">Kontekst.</param>
        /// <param name="sourceType">Typ źródłowy</param>
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
            if (value == null)
                return null;
            string str = (string)value;
            if (String.IsNullOrEmpty(str))
                return null;
            T result = GetList(context)[str];
            return result;
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
            if (value == null)
                return null;
            if (value.GetType() != typeof(string))
            {
                var result = GetList(context).Where(x => (x.Value != null && x.Value.ToString() == (value as T).ToString()) || (x.Value == null && x.Value == value)).FirstOrDefault();
                if (result.Key != null)
                    return result.Key;
                else
                    return null;
            }
            else
            {
                string str = (string)value;
                if (destinationType == typeof(string))
                    return str;
                if (String.IsNullOrEmpty(str))
                    return null;
                T result = GetList(context)[str];
                return result;
            }
        }
    }
}
