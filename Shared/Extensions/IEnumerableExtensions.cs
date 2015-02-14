using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Collections;

namespace SoftwareMind.Utils.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable Union(this IEnumerable firstEnumerable, IEnumerable secondEnumerable)
        {
            foreach (object item in firstEnumerable)
                yield return item;

            foreach (object item in secondEnumerable)
                yield return item;

        }

        public static IEnumerable<KeyValuePair<string, object>> ToEnumerable(this NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            return collection.Cast<string>().Select(key => new KeyValuePair<string, object>(key, collection[key]));
        }
    }
}
