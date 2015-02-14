
using System;
namespace SoftwareMind.SimpleWorkflow.Misc
{
    /// <summary>
    /// Rozszerzeniea klasy String
    /// </summary>
    [Serializable]
    public static class StringExtensions
    {
        /// <summary>
        /// Zwraca indeks pierwszego znaku nie będącego <paramref name="c"/>.
        /// </summary>
        /// <param name="str">Napis.</param>
        /// <param name="c">Znak.</param>
        /// <returns>Zwraca liczbę wiekszą bądź równą 0 jeśli znajdzie znak nie będący <paramref name="c"/>. W przeciwnym wypadku
        /// zwraca -1.</returns>
        public static int IndexOfNo(this string str, char c)
        {
            int i = 0;
            while (i < str.Length)
                if (str[i] != c)
                    return i;
                else
                    i++;
            return -1;
        }
    }
}
