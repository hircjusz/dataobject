using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validators
{
    public class ValidatorID : Validator
    {
        public ValidatorID()
            : base("RegexID", "WeightsID", "ModuloID", null)
        { }

        /// <summary>
        /// Validates ID number
        /// </summary>
        /// <param name="sequence">ID number</param>
        /// <returns>True if validated, false otherwise</returns>
        public override bool Validate(string sequence)
        {
            sequence = sequence.ToUpper();
            if (!validateRegex(sequence, regex))
                return false;

            int sum = 0;
            //for (int i = 0; i < 3; i++)
            //    sum += ((int)sequence[i] - shift) * weights[i];
            //sum += sumOfProducts(sequence, weights, 4, weights.Length);

            sum = sumOfProducts(sequence, weights);

            if (!compareModulo(sum, modulo, sequence[3]))
                return false;

            return true;
        }
    }
}
