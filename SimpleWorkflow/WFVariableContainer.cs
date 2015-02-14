using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using System;
using log4net;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public abstract class WFVariableContainer : MarshalByRefObject, SoftwareMind.SimpleWorkflow.IWFVariableContainer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WFVariableContainer));

        [Editor(typeof(WFVariableCollectionEditor), typeof(UITypeEditor))]
        protected IDictionary<string, WFVariable> variables { get; set; }

        protected WFVariableContainer()
        {
            this.variables = new Dictionary<string, WFVariable>();
        }

        public IDictionary<string, WFVariable> Variables
        {
            get
            {
                return variables;
            }
        }

        public void AddVariable(WFVariable variable)
        {
            if (variable != null)
            {
                variable.Container = this;

                if (!this.ContainsVariable(variable.Name))
                {
                    this.variables.Add(variable.Name, variable);
                }
                else
                {
                    this.variables[variable.Name] = variable;
                }
            }
        }

        public void AddVariableValue(string name, object value)
        {
            AddVariable(new WFVariable(name, value));
        }

        public virtual void SetVariableValue(string name, object value)
        {
            if (variables.ContainsKey(name))
                GetVariable(name).Value = value;
            else
                AddVariableValue(name, value);
        }

        public WFVariable GetVariable(string name)
        {
            try
            {
                return this.variables[name];
            }
            catch
            {
                log.Error("Nie znaleziono zmiennej " + (name != null ? name : "(null)"));
                throw;
            }
        }

        public object GetVariableValue(string name)
        {
            try
            {
                return this.variables[name].Value;
            }
            catch
            {
                log.Error("Nie znaleziono zmiennej " + (name != null ? name : "(null)"));
                throw;
            }
        }

        /// <summary>
        /// Zwraca wartosæ zmiennej albo jej domyœln¹ wartoœæ.
        /// </summary>
        /// <param name="name">Nazwa zmeinnej.</param>
        /// <returns>Wartoœæ zmiennej</returns>
        /// <exception cref="System.ArgumentException">Jest rzucany jeœli nazwa zamiennej nie jest kluczem kolekcji aktualnych zmiennych i nie ma wartosci domyœlnej.</exception>
        public object GetVariableValueOrDefault(string name)
        {
            if (variables.ContainsKey(name))
                return variables[name];

            WFVariableDefCollection defs = this.GetDefContainer().VariableDefs;
            if (defs.ContainsKey(name))
                return defs[name].GetDefaultValue();

            throw new ArgumentException(String.Format("Zmienna {0} nie znajduje siê w kolekcji zmiennych i nie ma wartoœci domyœlnej", name));
        }

        public virtual IDictionary<string, object> GetVariables()
        {
            return GetSelfVariables();
        }

        protected IDictionary<string, object> GetSelfVariables()
        {
            IDictionary<string, object> result = new Dictionary<string, object>();

            foreach (var e in this.Variables)
                result.Add(e.Key, e.Value.Value);

            // jesli zmienna nie zosta³a 'explicitly' ustawiona musimu dodaæ jej wartoœæ z
            // definicji
            WFVariableDefCollection definitionCollection = this.GetDefContainer().VariableDefs;
            foreach (var variableDef in definitionCollection)
            {
                if (!result.ContainsKey(variableDef.Key))
                {
                    Object value = null;

                    if (string.IsNullOrWhiteSpace(variableDef.Value.DefaultValue))
                    {
                        if (variableDef.Value.Type.IsValueType)
                        {
                            value = Activator.CreateInstance(variableDef.Value.Type);
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else
                    {
                        value = Convert.ChangeType(variableDef.Value.DefaultValue, variableDef.Value.Type);
                    }

                    result.Add(variableDef.Key, value);
                }
            }

            return result;
        }

        public bool ContainsVariable(string name)
        {
            return this.variables.Keys.Contains(name);
        }

        public abstract WFVariableDefContainer GetDefContainer();

        protected void DeleteVariables()
        {
            this.variables.Clear();
        }

        public void RemoveVariable(string name)
        {
            this.variables.Remove(name);
        }

        public virtual void AddVariables(IDictionary<string, object> activityArguments)
        {
            if (activityArguments != null)
            {
                foreach (KeyValuePair<string, object> pair in activityArguments)
                {
                    this.AddVariableValue(pair.Key, pair.Value);
                }
            }
        }

        protected void WriteVariablesStateToXmlElement(XElement element)
        {
            var elementsToAdd = (from v in this.Variables
                where v.Value.IsDefinedVariable()
                select v.Value.WriteStateToXmlElement()).ToArray();
            if (elementsToAdd.Length > 0)
                element.Add(new XElement("Variables", elementsToAdd));
        }

        protected void ReadVariablesStateFromXmlElement(XElement element)
        {
            XElement variables = element.Descendants("Variables").FirstOrDefault();
            if (variables != null && !variables.IsEmpty)
            {
                foreach (XElement child in variables.Descendants("Variable"))
                {
                    WFVariable singleVariable = new WFVariable();
                    singleVariable.Container = this;
                    singleVariable.ReadStateFromXmlElement(child);
                    this.AddVariable(singleVariable);
                }
            }
        }


    }
}