using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Validators
{
    public abstract class Validator : IValidator
    {
        public Validator()
        { }

        public Validator(string keyRegex, string keyWeights, string keyModulo, string keyComparator)
        {
            getParametersFromConfigFile(keyRegex, keyWeights, keyModulo, keyComparator);
        }

        /// <summary>
        /// Validates sequence against special requirements
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public virtual bool Validate(string sequence)
        {
            if (sequence != String.Empty)
                return true;
            else
                return false;
        }

        protected const int shift = 55;
        protected string regex;
        protected int[] weights;
        protected int modulo;
        protected int comparator;

        /// <summary>
        /// Retrieves parameters from configuration file
        /// </summary>
        /// <param name="keyRegex">Configuration, app settings key</param>
        /// <param name="keyWeights">Configuration, app settings key</param>
        /// <param name="keyModulo">Configuration, app settings key</param>
        protected virtual void getParametersFromConfigFile(string keyRegex, string keyWeights, string keyModulo, string keyComparator)
        {
            try
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyRegex))
                    regex = ConfigurationManager.AppSettings[keyRegex];
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyWeights))
                    weights = (from n in ConfigurationManager.AppSettings[keyWeights]
                               select int.Parse(n.ToString())).ToArray<int>();
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyModulo))
                    modulo = int.Parse(ConfigurationManager.AppSettings[keyModulo]);
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyComparator))
                    comparator = int.Parse(ConfigurationManager.AppSettings[keyComparator]);
            }
            catch (Exception exc)
            {
                string error = "There was an exception while reading configuration file: " + exc.Message;
                throw new Exception(error, exc);
            }
        }

        /// <summary>
        /// Validates number against regex
        /// </summary>
        /// <param name="sequence">Sequence to validate</param>
        /// <param name="pattern">Regular expression</param>
        /// <returns>True if validated, otherwise false</returns>
        protected bool validateRegex(string sequence, string pattern)
        {
            return Regex.IsMatch(sequence, pattern);
        }

        /// <summary>
        /// Calculates sum of weight * char products
        /// </summary>
        /// <param name="sequence">Sequence to validate</param>
        /// <param name="weights">Array of weights</param>
        /// <returns>Sum of weight * char products</returns>
        protected int sumOfProducts(string sequence, int[] weights)
        {
            int sum = 0;
            int temp;
            for (int i = 0; i < weights.Length; i++)
            {
                if (!int.TryParse(sequence.Substring(i, 1), out temp))
                    temp = (int)sequence[i] - shift;

                sum += temp * weights[i];
            }

            return sum;
        }

        /// <summary>
        /// Calculates sum of digits weight * char products
        /// </summary>
        /// <param name="sequence">Sequence to validate</param>
        /// <param name="weights">Array of weights</param>
        /// <returns>Sum of digits weight * char products</returns>
        protected int sumOfProductsDigits(string sequence, int[] weights)
        {
            int sum = 0;
            for (int i = 0; i < weights.Length; i++) //:) suma cyfr:
                sum += (int.Parse(sequence.Substring(i, 1)) * weights[i])
                    .ToString().Sum(n => int.Parse(n.ToString()));

            return sum;
        }


        /// <summary>
        /// Calculates subset of sum of weight * char products
        /// </summary>
        /// <param name="sequence">Sequence to validate</param>
        /// <param name="weights">Array of weights</param>
        /// <param name="from">Start index</param>
        /// <param name="to">End index</param>
        /// <returns>Subset of sum of weight * char products</returns>
        protected int sumOfProducts(string sequence, int[] weights, int from, int to)
        {
            int sum = 0;
            for (int i = from; i < to; i++)
                sum += int.Parse(sequence.Substring(i, 1)) * weights[i];

            return sum;
        }

        /// <summary>
        /// Compares product of modulo operation with given sign
        /// </summary>
        /// <param name="sum">Sum of weight * char product</param>
        /// <param name="modulo">Modulo number</param>
        /// <param name="comparator">Sign to compare with</param>
        /// <returns>True if product of modulo operation equals comparator, otherwise false</returns>
        protected bool compareModulo(int sum, int modulo, char comparator)
        {
            return (sum % modulo) == int.Parse(comparator.ToString());
        }

        /// <summary>
        /// Compares product of modulo operation with given sign
        /// </summary>
        /// <param name="sum">Sum of weight * char product</param>
        /// <param name="modulo">Modulo number</param>
        /// <param name="number">Sign to compare with</param>
        /// <returns>True if product of modulo operation equals comparator, otherwise false</returns>
        protected bool compareModulo(int sum, int modulo, int number)
        {
            return (sum % modulo) == number;
        }
    }
}
