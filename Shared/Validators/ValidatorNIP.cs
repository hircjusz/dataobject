using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validators
{
    public class ValidatorNIP : Validator
    {
        public ValidatorNIP()
            : base("RegexNIP", "WeightsNIP", "ModuloNIP", null)
        { }

        /// <summary>
        /// Validates NIP
        /// </summary>
        /// <param name="sequence">NIP</param>
        /// <returns>True if validated, false otherwise</returns>
        public override bool Validate(string sequence)
        {
            string country = "PL";
            //remove spaces, -
            sequence = sequence.Replace(" ", String.Empty);
            sequence = sequence.Replace("-", String.Empty);

            //remove PL sign
            if (sequence.ToUpper().StartsWith(country))
                sequence = sequence.Remove(0, country.Length);

            if (!validateRegex(sequence, regex))
                return false;

            int sum = sumOfProducts(sequence, weights);

            if (!compareModulo(sum, modulo, sequence[9]))
                return false;

            return true;
        }
    }
}
