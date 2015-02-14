using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validators
{
    public class ValidatorIBAN : Validator
    {
        public ValidatorIBAN()
            : base("RegexIBAN", null, "ModuloIBAN", "ComparatorIBAN")
        { }

        /// <summary>
        /// Validates ID number
        /// </summary>
        /// <param name="sequence">ID number</param>
        /// <returns>True if validated, false otherwise</returns>
        public override bool Validate(string sequence)
        {
            sequence = sequence.Replace(" ", String.Empty);
            sequence = sequence.ToUpper();
            if (!validateRegex(sequence, regex))
                return false;

            string prefix = sequence.Substring(0, 4);
            sequence = sequence.Substring(4) + prefix;
            string newSequence = String.Empty;

            foreach (char c in sequence)
            {
                if (Char.IsLetter(c))
                    newSequence += ((int)c - shift);
                else
                    newSequence += c;
            }

            int mod = bigNumberModulo(newSequence, modulo);
            if (mod != comparator)
                return false;

            return true;
        }

        /// <summary>
        /// Calculates modulo for of integers
        /// </summary>
        /// <param name="number">Big number</param>
        /// <param name="modulo">Modulo</param>
        /// <returns>Big number % modulo</returns>
        protected int bigNumberModulo(string number, int modulo)
        {
            string result = String.Empty;

            for (int i = 0; i < number.Length; i++)
            {
                result = result + number[i];
                result = (int.Parse(result) % modulo).ToString();
            }

            return int.Parse(result);
        }
    }
}