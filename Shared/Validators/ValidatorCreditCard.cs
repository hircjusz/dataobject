using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validators
{
    public class ValidatorCreditCard : Validator
    {
        public ValidatorCreditCard()
            : base("RegexCreditCard", "WeightsCreditCard", null, null)
        { }

        protected const char zero = '0';
        protected const int lenght = 16;

        /// <summary>
        /// Validates Credit Card number
        /// </summary>
        /// <param name="sequence">Credit Card number</param>
        /// <returns>True if validated, false otherwise</returns>
        public override bool Validate(string sequence)
        {
            if (!validateRegex(sequence, regex))
                return false;

            string zeros = new string(zero, lenght - sequence.Length);
            zeros += sequence;
            sequence = zeros;

            int sum = sumOfProductsDigits(sequence, weights);

            if (sum.ToString().Last() != zero)
                return false;

            return true;
        }
    }
}
