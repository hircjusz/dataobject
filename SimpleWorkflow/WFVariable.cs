using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Attributes;
using log4net;


namespace SoftwareMind.SimpleWorkflow
{
    /// <summary>
    /// Klasa reprezentująca zmienną
    /// </summary>
    [Serializable]
    public class WFVariable : IWFStateElement
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFVariable));

        /// <summary>
        /// Nazwa zmiennej
        /// </summary>
        private string name;
        /// <summary>
        /// Wartosc zmiennej
        /// </summary>
        /// <value>Wartosć</value>
        private object value;



        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFVariable"/>.
        /// </summary>
        /// <param name="name">Nazwa.</param>
        /// <param name="value">Wartość.</param>
        public WFVariable(string name, object value) : this()
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="WFVariable"/>.
        /// </summary>
        public WFVariable()
        {
            IsInitializedAfterRestore = true;
            name = "VariableName";
        }



        /// <summary>
        /// Kolekcja do któej nalezy zmienna
        /// </summary>
        /// <value>Kolekcja.</value>
        internal WFVariableContainer Container { get; set; }

        /// <summary>
        /// Czy zmienna została odtworzona po deserjalizacji. W związku z tym ze może być to proces tdługotrwały np w
        /// przypadku DataObject, zmienne nie są odtwarzane na starcie tylko przy pierwszym użyciu
        /// </summary>
        private Boolean IsInitializedAfterRestore { get; set; }

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
                if (name != value && Container != null)
                {
                    if (Container.ContainsVariable(value))
                        throw new ArgumentException(String.Format("Zmienna o nazwie '{0}' już istnieje w kolekcji.", value));
                    var col = Container;
                    Container.RemoveVariable(name);
                    name = value;
                    col.AddVariableValue(name, Value);
                }
                else
                    name = value;
            }
        }

        [Browsable(false)]
        public object Value
        {
            get
            {
                if (this.IsInitializedAfterRestore == false)
                {
                    WFVariableDef def = this.Container.GetDefContainer().GetVariable(this.Name);
                    bool customSerialization = def.Type.GetCustomAttributes(typeof(WFCustomSerializationAttribute), true).Length > 0;

                    if (customSerialization)
                    {
                        WFCustomSerializationAttribute[] attributes = (WFCustomSerializationAttribute[])def.Type.GetCustomAttributes(typeof(WFCustomSerializationAttribute), true);
                        IWFVariableSerializer serializer = (IWFVariableSerializer)Activator.CreateInstance(attributes[0].WFVariableSerializer);
                        this.value = serializer.Deserialize(this.value, this.Container.Variables);
                        this.IsInitializedAfterRestore = true;
                    }
                    else
                    {
                        throw new ArgumentException("Podana zmienna nie jest zainicjalizowana oraz nie implementuje IWFVariableSerializer");
                    }
                }

                return value;
            }
            set
            {
                this.value = value;
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
            return name.Equals((obj as WFVariable).Name);
        }

        /// <summary>
        /// Zwraca hash code obiektu, będący hash codem nazwy. Wykorzystywane do współpracy ze słownikiem
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }




        #region IWFStateElementSerializer Members

        public void ReadStateFromXmlElement(System.Xml.Linq.XElement element)
        {
            string name = element.Attribute("Name").Value;
            this.Name = name;

            WFVariableDef def = this.Container.GetDefContainer().GetVariable(this.Name);

            if (def.IsCollection)
            {
                ReadCollectionStateFromXmlElement(element);
            }
            else
            {
                ReadVariableStateFromXmlElement(element);
            }
        }

        private void ReadCollectionStateFromXmlElement(System.Xml.Linq.XElement element)
        {
            log.Debug("ReadCollectionStateFromXmlElement");

            bool isNull = Boolean.Parse(element.Attribute("IsNull").Value);
            string value = element.Attribute("Value").Value;
            WFVariableDef def = this.Container.GetDefContainer().GetVariable(this.Name);
            this.Value = Activator.CreateInstance(def.Type);
            foreach (var variableElement in element.Elements("Variable"))
            {
                Type varType = Type.GetType(variableElement.Attribute("Type").Value);
                string varValue = variableElement.Attribute("Value").Value;
                string isVarNull = variableElement.Attribute("IsNull").Value;

                log.DebugFormat("Wczytywanie typ: {0} wartosc: {1} null?: {2}", varType, varValue, isVarNull);

                object variable = Activator.CreateInstance(varType);
                variable = Convert.ChangeType(varValue, varType);
                (this.Value as IList).Add(variable);
            }
        }

        private void ReadVariableStateFromXmlElement(System.Xml.Linq.XElement element)
        {
            bool isNull = Boolean.Parse(element.Attribute("IsNull").Value);
            string value = element.Attribute("Value").Value;
            WFVariableDef def = this.Container.GetDefContainer().GetVariable(this.Name);
            bool customSerialization = def.Type.GetCustomAttributes(typeof(WFCustomSerializationAttribute), true).Length > 0;

            if (customSerialization)
            {
                // obiekt sam zjamie sie deserjalizacją
                this.IsInitializedAfterRestore = false;
                this.Value = value;
            }
            else
            {
                this.Value = isNull == true ? null : Convert.ChangeType(value, def.Type);
            }
        }

        public bool IsDefinedVariable()
        {
            return this.Container.GetDefContainer().ContainsVariable(this.Name);
        }

        public System.Xml.Linq.XElement WriteStateToXmlElement()
        {
            WFVariableDef def = this.Container.GetDefContainer().GetVariable(this.Name);

            if (def == null) throw new ArgumentNullException("Serjalizacja nie obejmuje zmiennych bez definicji");

            if (def.IsCollection)
            {
                return WriteCollectionStateToXmlElement();
            }
            else
            {
                return WriteVariableStateToXmlElement(def);
            }
        }

        private XElement WriteVariableStateToXmlElement(WFVariableDef def)
        {
            bool customSerialization = def.Type.GetCustomAttributes(typeof(WFCustomSerializationAttribute), true).Length > 0;

            if (customSerialization)
            {
                WFCustomSerializationAttribute[] attributes = (WFCustomSerializationAttribute[])def.Type.GetCustomAttributes(typeof(WFCustomSerializationAttribute), true);

                IWFVariableSerializer serializer = (IWFVariableSerializer)Activator.CreateInstance(attributes[0].WFVariableSerializer);
                this.value = serializer.SerializeValue(this.Value);
                this.IsInitializedAfterRestore = false;
            }

            return new System.Xml.Linq.XElement("Variable",
                new XAttribute("Name", this.Name),
                new XAttribute("Value", this.value == null ? "" : this.value.ToString()),
                new XAttribute("IsNull", this.value == null ? "true" : "false")
                );
        }



        private XElement WriteCollectionStateToXmlElement()
        {
                return new System.Xml.Linq.XElement("Variable",
                       new XAttribute("Name", this.Name),
                       new XAttribute("IsNull", this.Value == null ? "true" : "false"),
                       from element in ((IEnumerable<object>)this.Value)
                       select new System.Xml.Linq.XElement("Variable",
                        new XAttribute("Type", element.GetType().FullName),
                        new XAttribute("Value", element == null ? "" : element.ToString()),
                        new XAttribute("IsNull", this.Value == null ? "true" : "false")
                        )
                    );
        }

        #endregion
    }
}
