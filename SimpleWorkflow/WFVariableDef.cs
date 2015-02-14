using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using SoftwareMind.Scripts;
using SoftwareMind.SimpleWorkflow.Attributes;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow
{
    /// <summary>
    /// Klasa reprezentująca zmienną
    /// </summary>
    [Serializable]
    public class WFVariableDef : IWFTemplateElement, ICloneable
    {

        private string defaultValue;
        /// <summary>
        /// Wskazuje czy zmienna przechowywuje kolekcję obiektów.
        /// </summary>
        private bool isCollection;
        /// <summary>
        /// Nazwa zmiennej
        /// </summary>
        private string name;
        /// <summary>
        /// Pozwala pobrać i ustawić typ zmiennej
        /// </summary>
        /// <value>The type.</value>
        protected Type type;



        public WFVariableDef(string name, Type type, bool isCollection) : this(name, type)
        {
            this.IsCollection = isCollection;
        }

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFVariable"/>.
        /// </summary>
        /// <param name="name">Nazwa.</param>
        /// <param name="type"></param>
        public WFVariableDef(string name, Type type)
        {
            Name = name;
            Type = type;
            Direction = WFVariableType.InOut;
        }

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFVariable"/>.
        /// </summary>
        public WFVariableDef()
        {
            Name = "VariableName";
            Type = typeof(int);
            Direction = WFVariableType.InOut;
        }

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFVariable"/>.
        /// </summary>
        /// <param name="name">Nazwa.</param>
        /// <param name="type"></param>
        /// <param name="defaultValue"></param>
        public WFVariableDef(string name, Type type, string defaultValue)
            : this(name, type)
        {
            DefaultValue = defaultValue;
        }


        /// <summary>
        /// Kolekcja do której nalezy zmienna
        /// </summary>
        /// <value>Kolekcja.</value>
        internal WFVariableDefCollection Collection { get; set; }

        /// <summary>
        /// Pozwala pobrać i ustawić domyślną wartość
        /// </summary>
        /// <value>The default valur.</value>
        public string DefaultValue {
            get
            {
                return defaultValue;
            }
            set
            {
                if (value != null)
                    ValidateDefaultValue(value, Type, IsCollection);

                defaultValue = value;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić informacje o tym czy zmienna jest wejściowa i wyjściowa
        /// </summary>
        /// <value>The direction.</value>
        [DefaultValue(WFVariableType.InOut)]
        public WFVariableType Direction { get; set; }

        /// <summary>
        /// Wskazuje czy zmienna przechowywuje kolekcję obiektów.
        /// </summary>
        /// <value>
        ///     <c>true</c> jeśli jest kolekcją; w przeciwnym wypadku, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool IsCollection {
            get
            {
                return isCollection;
            }
            set
            {
                ValidateDefaultValue(DefaultValue, Type, value);
                isCollection = value;
            }
        }

        /// <summary>
        /// Pozwala pobrać i ustawić nazwę zmiennej.
        /// </summary>
        /// <value>The name.</value>
        /// <exception cref="ArgumentException">Jest rzucane jeśli nazwa zmiennej jest pusta, nulem, albo istnieje już
        /// w kolekcji.</exception>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Nazwa nie moze być pusta i być nullem");
                if (name != value && Collection != null)
                {
                    if (Collection.Contains(value))
                        throw new ArgumentException(String.Format("Zmienna o nazwie '{0}' już istnieje w kolekcji.", value));
                    var col = Collection;
                    Collection.Remove(name);
                    name = value;
                    col.Add(this);
                }
                else
                    name = value;
            }
        }

        [TypeConverter(typeof(VariableTypeSelector))]
        public Type Type
        {
            get
            {
                return this.type;
            }
            set
            {
                ValidateType(value);
                ValidateDefaultValue(DefaultValue, value, IsCollection);
                type = value;
            }
        }




        /// <summary>
        /// Porónuje obiekty. Wykorzystywane do współpracy z klasą Dictionary
        /// </summary>
        /// <param name="obj">Obiekt do porónania</param>
        /// <returns>
        ///     <c>true</c> jeśli obiekty majataką samą nazwę; w przeciwnym wypadku, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return name.Equals((obj as WFVariableDef).Name);
        }

        /// <summary>
        /// Zwraca domyślną wartość po sprarwosaniu jej.
        /// </summary>
        /// <returns>Domyślna wartość.</returns>
        public object GetDefaultValue()
        {
            return GetDefaultValue(Type);
        }

        /// <summary>
        /// Zwraca domyślną wartość po sprarwosaniu jej.
        /// </summary>
        /// <returns>Domyślna wartość.</returns>
        private object GetDefaultValue(Type type)
        {
            try
            {
                if (type == typeof(string))
                    return DefaultValue;
                else if (type == typeof(char))
                {
                    if (DefaultValue.Length != 1)
                        throw new ArgumentException("Długosć domyślnej wartości musi wynosić 1.");
                    return DefaultValue[0];
                }
                else if (type == typeof(byte))
                    return byte.Parse(DefaultValue);
                else if (type == typeof(short))
                    return short.Parse(DefaultValue);
                else if (type == typeof(int))
                    return int.Parse(DefaultValue);
                else if (type == typeof(long))
                    return long.Parse(DefaultValue);
                else if (type == typeof(bool))
                    return bool.Parse(DefaultValue);
                else if (type == typeof(DateTime))
                    return DateTime.Parse(DefaultValue);
                else
                    throw new ArgumentException("Nieznany typ: " + type.Name);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Nie udało się sparsować wartości {0} na typ {1}.", ex);
            }
        }

        /// <summary>
        /// Zwraca typ definicji.
        /// </summary>
        /// <returns>Typ</returns>
        public Type GetDefType()
        {
            if (Type == null)
                return Type;
            else if (Type != null)
            {
                if (!IsCollection)
                    return Type;
                else
                {
                    var type = typeof(ICollection<>);
                    return type.MakeGenericType(Type);
                }
            }
            else
                return typeof(DynamicScriptObject);
        }

        /// <summary>
        /// Zwraca hash code obiektu, będący hash codem nazwy. Wykorzystywane do współpracy ze słownikiem
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <summary>
        /// Sprawdza poprawność domyślnej wartości.
        /// </summary>
        /// <param name="value">Domyślnej wartości.</param>
        /// <param name="type"></param>
        /// <param name="isCollection"></param>
        private void ValidateDefaultValue(string value, Type type, bool isCollection)
        {
            if (value == null)
                return;

            if (type == null)
                throw new InvalidOperationException("Przed wskazaniem domyślnej wartosci wybierz typ danych.");
            if (isCollection)
                throw new InvalidOperationException("Domyślny typ danych nie moze być ustawiony dla kolkcji.");
            if (defaultValue != null && !GetSimpleTypes().Contains(Type))
                throw new ArgumentException(String.Format("Wartosć domyślna dla typu {0} nie jest obsługiwana", Type.FullName));

            string old = defaultValue;
            try
            {
                defaultValue = value;
                GetDefaultValue(type);
            }
            catch
            {
                defaultValue = old;
                throw new ArgumentException(String.Format("Nie udało się sparsować wartości {0} na typ {1}.", value, Type));
            }
        }

        /// <summary>
        /// Sprawdza serializacja wskazanego typu jest mozliwa.
        /// </summary>
        /// <param name="type">Typ.</param>
        private void ValidateType(Type type)
        {
            bool customSerialization = type.GetCustomAttributes(typeof(WFCustomSerializationAttribute), true).Length > 0;

            if (!type.IsPrimitive && type != typeof(string) && ! customSerialization && type != typeof(DateTime))
                throw new ArgumentException("Ten typ nie jest obsługiwany");
        }

        /// <summary>
        /// Zwraca listę typów prostych
        /// </summary>
        /// <returns></returns>
        internal static Type[] GetSimpleTypes()
        {
            return new Type[] { typeof(string), typeof(char), typeof(byte), typeof(short), typeof(int),
                typeof(long), typeof(bool), typeof(DateTime)};
        }




        #region IWFStateElementSerializer Members

        private static bool IsTypeCollection(Type t)
        {
            if (typeof(System.Collections.ICollection).IsAssignableFrom(t))
                return true;

            return false;
        }

        #endregion

        #region IWFTemplateElement Members

        public void ReadTemplateFromXmlElement(XElement element)
        {
            string direction = element.Attribute("Direction").Value;
            string isCollection = element.Attribute("IsCollection").Value;
            string name = element.Attribute("Name").Value;
            string type = element.Attribute("Type").Value;
            XAttribute defaultValueAttribute = element.Attribute("DefaultValue");
            string defaultValue = defaultValueAttribute != null ? defaultValueAttribute.Value : null;

            this.Direction = (WFVariableType)Enum.Parse(typeof(WFVariableType), direction);
            this.IsCollection = bool.Parse(isCollection);
            this.Name = name;
            this.Type = Type.GetType(type);
            this.DefaultValue = String.IsNullOrEmpty(defaultValue) ? null : defaultValue;
        }

        public XElement WriteTemplateToXmlElement()
        {
            return new XElement("VariableDef",
                new XAttribute("Direction", this.Direction),
                new XAttribute("IsCollection", this.IsCollection),
                new XAttribute("Name", this.Name),
                new XAttribute("Type", this.Type.GetShortName()),
                new XAttribute("DefaultValue", this.DefaultValue ?? ""));
        }

        #endregion

        public object Clone()
        {
            return new WFVariableDef
            {
                Type = this.Type,
                Name = this.Name,
                DefaultValue = this.DefaultValue,
                Direction = this.Direction
            };
        }
    }
}
