using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa zawiera pomocnicze metody dla klasy MethodBase
    /// </summary>
    [Serializable]
    public static class MethodBaseHelper
    {
        /// <summary>
        /// Generuje unikalną nazwę dla metody bazujac na jej nazwie i parametrach.
        /// </summary>
        /// <param name="method">Metoda</param>
        /// <returns>Unikalna nazwa</returns>
        public static string GenerateUniqueMethodName(MethodBase method)
        {
            return method.ToString();
        }

        /// <summary>
        /// Zwraca typ opisujący metodę bazując na jej unikalnej nazwie
        /// </summary>
        /// <param name="type">Typ</param>
        /// <param name="uniquename">Unikalna nazwa metody</param>
        /// <returns>Metoda</returns>
        public static MethodBase GetMethodBase(Type type, string uniquename)
        {
            if(uniquename[0] == '.')
                return type.GetConstructors().Where(x => GenerateUniqueMethodName(x) == uniquename).FirstOrDefault();
            else
                return type.GetMethods().Where(x => GenerateUniqueMethodName(x) == uniquename).FirstOrDefault();
        }
    }
}
