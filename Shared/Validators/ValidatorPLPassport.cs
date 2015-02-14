using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validators
{
    public class ValidatorPLPassport : Validator
    {
        public ValidatorPLPassport()
            : base("RegexPLPassport", "WeightsPLPassport", "ModuloPLPassport", null)
        { }

        /// <summary>
        /// Validates PL Passport number only against regex
        /// </summary>
        /// <param name="sequence">PL Passport number</param>
        /// <returns>True if validated, false otherwise</returns>
        public override bool Validate(string sequence)
        {
            sequence = sequence.ToUpper();
            if (!validateRegex(sequence, regex))
                return false;

            //sequence = sequence.Replace('<', '0');

            //int sum = 0;
            //for (int i = 0; i < 2; i++)
            //    sum += ((int)sequence[i] - 55) * weights[i];
            //sum += sumOfProducts(sequence, weights, 2, weights.Length);

            //if (!compareModulo(sum, modulo, sequence.Last()))
            //    return false;

            return true;
        }
    }
}
