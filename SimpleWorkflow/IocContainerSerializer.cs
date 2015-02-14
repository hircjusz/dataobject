using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SoftwareMind.Shared.Mef;

namespace SoftwareMind.SimpleWorkflow
{
    public class IocContainerSerializer : IWFVariableSerializer
    {
        public string SerializeValue(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var type = obj.GetType();
            var exportAttribute = type.GetCustomAttributes(typeof (ExportAttribute), true).OfType<ExportAttribute>().SingleOrDefault();

            if (exportAttribute == null)
            {
                throw new NotSupportedException("Type cannot be serialized");
            }

            return exportAttribute.ContractType.FullName;
        }

        public object Deserialize(object obj, IDictionary<string, WFVariable> otherVariables)
        {
            string typeName = (string) obj;
            Type type = null;
            foreach (var assemb in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("System.")))
            {
                try
                {
                    type = assemb.GetTypes().SingleOrDefault(t => t.FullName == typeName);
                    if (type != null)
                        break;
                }
                //{"Inheritance security rules violated by type: 'System.Web.Mvc.CompareAttribute'. Derived types must either match the security accessibility of the base type or be less accessible.":"System.Web.Mvc.CompareAttribute"}
                catch (Exception)
                {
                }
            }
            //Type type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Single(t => t.FullName == typeName);
            //Type type = Type.GetType(typeName);

            return ServiceLocator.Current.GetInstance(type);
        }
    }
}