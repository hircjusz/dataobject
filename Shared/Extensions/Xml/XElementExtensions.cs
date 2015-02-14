using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SoftwareMind.Utils.Extensions.Xml
{
    public static class XElementExtensions
    {
        public static string GetAttributeStringValueOrNull(this XElement element, string attributeCode)
        {
            if(element.Attribute(attributeCode) != null)
                return element.Attribute(attributeCode).Value;

            return null;
        }

        public static Boolean GetAttributeBooleanValueOrDefault(this XElement element, string attributeCode, bool defaultValue = default(bool))
        {
            if (element.Attribute(attributeCode) != null)
                return Convert.ToBoolean(element.Attribute(attributeCode).Value);

            return defaultValue;
        }

        public static int? GetAttributeIntValueOrNull(this XElement element, string attributeCode)
        {
            int result; 

            string value = GetAttributeStringValueOrNull(element, attributeCode);
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static string GetElemenetValueOrNull(this XElement element, string elementName)
        {
            if (element.Element(elementName) != null)
                return element.Element(elementName).Value;

            return null;
        }
    }
}
