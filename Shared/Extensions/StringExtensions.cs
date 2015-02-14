using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareMind.Utils.Extensions.StringExtensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Humanize a "CamelCasedString" into "Camel Cased String".
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Humanize(this string source)
        {
            StringBuilder sb = new StringBuilder();

            char last = char.MinValue;
            foreach (char c in source)
            {
                if (char.IsLower(last) == true && char.IsUpper(c) == true)
                {
                    sb.Append(' ');
                }
                sb.Append(c);
                last = c;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Zwraca liczbę wystąpień danego wzorca w bieżącej instancji
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pattern">Wzorzec, którego wystąpienia chcemy policzyć. Wzorzec nie powinien być pusty</param>
        /// <returns></returns>
        public static int Occurences(this string source, string pattern)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(pattern))
                return 0;
            return (source.Length - source.Replace(pattern, "").Length) / pattern.Length;
        }

        public static long ToId(this string source, bool checkRange = true)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException("Source is null or empty");

            long result;
            if (!long.TryParse(source, out result))
                throw new ArgumentException("Source is not valid number");

            if (checkRange && result <= 0)
                throw new ArgumentException("Source is less than or equal to 0");

            return result;
        }

        public static string RemoveMultipleWhiteSpaces(this string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ");
        }

        public static bool IsEmail(this string str)
        {
            if (str == null) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(str.ToLower(), @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$");
        }

        /// <summary>
        /// Dzieli string wg podanego separatora.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns>Listę wydzielonych stringów oczyszczone z białych znaków.</returns>
        public static List<string> TrimToList(this string str, char separator = ',')
        {
            if (String.IsNullOrWhiteSpace(str) || String.IsNullOrWhiteSpace(str.Trim(separator)))
                return new List<string>();

            return str.Split(separator).Select(s => s.Trim()).ToList<string>();
        }

        public static string Next(this string code)
        {
            string ncode = code;
            char ch;
            int idx = code.Length;
            int chCode;

            while (idx > 0)
            {
                ch = ncode.Substring(idx - 1, 1).ToCharArray()[0];
                chCode = ((int)ch) + 1;

                if (chCode < (int)'0' || chCode > (int)'Z')
                    ch = '0';
                else if (chCode > (int)'9' && chCode < (int)'A')
                    ch = 'A';
                else
                    ch = (char)chCode;

                ncode = ncode.Substring(0, idx - 1) + ch + ncode.Substring(idx, ncode.Length - idx);
                idx--;

                if (ch != '0') break;
            }

            return ncode;
        }

    }
}
