using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using SoftwareMind.SimpleWorkflow.Designer;
using SoftwareMind.SimpleWorkflow.Exceptions;
using SoftwareMind.SimpleWorkflow.Misc;

namespace SoftwareMind.SimpleWorkflow.Actions
{
    [Serializable]
    public class WfActivityActionContainer : IWFTemplateElement, ICloneable
    {
        public const string TemplateName = "WfActivityActionContainer";

        public string Name { get; set; }

        private Type action;

        [TypeConverter(typeof (TypeSelector))]
        [TypeSelectorInterfaceTypeAtribute(typeof (IWfActivityAction))]
        [DefaultValue(null)]
        public Type Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
                if (action != null)
                {
                    Parameters = CreateActionParameters(action);
                }
                else
                {
                    Parameters = null;
                }
            }
        }

        [Editor(typeof(DetailsUIEditor), typeof(UITypeEditor))]
        public IWfActivityActionParameters Parameters { get; set; }

        public IWfActivityAction CreateAction()
        {
            return CreateAction(Action);
        }

        #region IWFTemplateElement Members

        public void ReadTemplateFromXmlElement(XElement element)
        {
            Name = (element.Attribute("Name") != null) ? element.Attribute("Name").Value : null;
            if (!string.IsNullOrEmpty("Action"))
            {
                Action = Type.GetType(element.Attribute("Action").Value);
            }
            if (Action != null)
            {
                var parameters = CreateActionParameters(Action);
                parameters.ReadTemplateFromXmlElement(element);
                Parameters = parameters;
            }
        }

        public XElement WriteTemplateToXmlElement()
        {
            var result = new XElement(TemplateName);
            {
                result.Add(new XAttribute("Name", Name));
            }
            if (Action != null)
            {
                result.Add(new XAttribute("Action", Action.GetShortName()));
            }
            if (Parameters != null)
            {
                result.Add(Parameters.WriteTemplateToXmlElement());
            }
            return result;
                
        }

        #endregion

        public object Clone()
        {
            return new WfActivityActionContainer
            {
                Action = Action,
                Name = Name,
                Parameters = (IWfActivityActionParameters)Parameters.Clone()
            };
        }

        private IWfActivityAction CreateAction(Type actionType)
        {
            var objectHandle = Activator.CreateInstanceFrom(actionType.Assembly.CodeBase, actionType.FullName);

            return (IWfActivityAction)objectHandle.Unwrap();
        }

        private IWfActivityActionParameters CreateActionParameters(Type actionType)
        {
            var instance = CreateAction(actionType);

            return instance.CreateDefaultParameter();
        }

        public void Validate()
        {
            if (Action == null)
            {
                throw new WFDesignException(string.Format("Action - typ akcji nie może być pusty"));
            }
            if (Parameters == null)
            {
                throw new WFDesignException(string.Format("Parameters - parametry akcji nie może być puste"));
            }
            Parameters.Validate();
        }
    }
}
