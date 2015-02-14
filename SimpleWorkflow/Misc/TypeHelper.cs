using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    /// <summary>
    /// Klasa przechowywująca metody pomocnicze do obsługi typów.
    /// </summary>
    [Serializable]
    public static class TypeHelper
    {
        public static IEnumerable<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.FullName.Contains("System.") && !a.FullName.Contains("Miscrosoft."))
                .SelectMany(a => a.GetTypes());
        }

        /// <summary>
        /// Zwraca typ bazując na jego nazwie
        /// </summary>
        /// <param name="name">Nazwa.</param>
        /// <returns></returns>
        public static Type GetType(string name)
        {
            Type type = Type.GetType(name);
            if (type == null)
            {
                if (name.Contains(','))
                {
                    type = TypeHelper.GetAllTypes()
                        .Where(x => x.AssemblyQualifiedName == name).FirstOrDefault();
                }
                else
                {
                    type = TypeHelper.GetAllTypes()
                        .Where(x => x.FullName == name).FirstOrDefault();
                }
            }
            return type;
        }

        /// <summary>
        /// Zwraca nazwę typu bez versji
        /// </summary>
        /// <param name="type">Typ</param>
        /// <returns>Nazwa</returns>
        public static string GetShortName(this Type type)
        {
            string result = type.AssemblyQualifiedName;
            int start = result.IndexOf(',');
            if (start > 0)
            {
                start = result.IndexOf(',', start + 1);
                if (start > 0)
                    result = result.Remove(start);
            }
            return result;
        }
    }
}
