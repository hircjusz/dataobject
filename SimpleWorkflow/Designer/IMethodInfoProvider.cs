using System;
using System.Reflection;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Interfejs znakujący klasę jako majacą właściwość, w której trzeba wskazac metodę danego typu.
    /// </summary>
    public interface IMethodInfoProvider
    {
        /// <summary>
        /// Zwraca Typ dla któego będą wyliczne dostępne metody
        /// </summary>
        /// <returns>Typ</returns>
        Type GetTypeToEnum();

        /// <summary>
        /// Pozwala pobrać metodę
        /// </summary>
        /// <value>Metoda.</value>
        MethodBase Method
        {
            get;
        }
    }
}
