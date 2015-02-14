using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace SoftwareMind.SimpleWorkflow
{
    [Serializable]
    public class WFVariableDefContainer
    {
        protected WFVariableDefCollection Variables { get; set; }

        [Editor(typeof(WFVariableCollectionEditor), typeof(UITypeEditor))]
        [Category("Common")]
        [Description("Definicja zmiennych.")]
        public WFVariableDefCollection VariableDefs
        {
            get
            {
                return Variables;
            }
            set
            {
                this.Variables = value;
            }
        }

        internal WFVariableDefContainer()
        {
            this.Variables = new WFVariableDefCollection();
        }

        public void Add(WFVariableDef variable)
        {
            if (variable != null)
            {
                if (!this.ContainsVariable(variable.Name))
                {
                    this.Variables.Add(variable.Name, variable);
                }
                else
                {
                    this.Variables[variable.Name] = variable;
                }
            }
        }

        public WFVariableDef GetVariable(string name)
        {
            return this.Variables[name];
        }

        public bool ContainsVariable(string name)
        {
            return this.Variables.Keys.Contains(name);
        }

        protected void DeleteVariables()
        {
            this.Variables.Clear();
        }
    }
}
