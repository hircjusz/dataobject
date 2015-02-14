using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SoftwareMind.Utils.Extensions
{
    public static class IDictionaryExtensions
    {
        public static bool HasNotNullElement(this IDictionary<string, string> dict, string key)
        {
            return dict.ContainsKey(key) && !String.IsNullOrEmpty(dict[key]);
        }

        public static void AddNotExistentEntries(this IDictionary<string, object> destination, IDictionary<string, object> source)
        {
            if(source != null)
                foreach (var kvp in source)
                {
                    if (destination.ContainsKey(kvp.Key) == false)
                        destination.Add(kvp);
                }
        }
    }
}
