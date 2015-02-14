using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    [Serializable]
    public class MetaDataAttribute : Attribute
    {
        public string Key;
        public string Value;

        public MetaDataAttribute(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
