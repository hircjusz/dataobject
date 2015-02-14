using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.Utils.Collections
{
    public class CommaSeparatedValueCollection : List<string>
    {
        public CommaSeparatedValueCollection(string commaDelimitedValues)
        {
            if (!string.IsNullOrEmpty(commaDelimitedValues))
            {
                string[] values = commaDelimitedValues.Split(',');
                this.AddRange(values);
            }
        }

        public string ToCommaDelimitedString()
        {
            return string.Join(",", this);
        }
        
    }
}
