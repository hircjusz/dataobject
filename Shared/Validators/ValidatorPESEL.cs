using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validators
{
    public class ValidatorPESEL : Validator
    {
        public ValidatorPESEL()
            : base("RegexPESEL", "WeightsPESEL", "ModuloPESEL", "ComparatorPESEL")
        { }

        /// <summary>
        /// Validates PESEL
        /// http://www.mswia.gov.pl/portal/pl/381/32/PESEL.html
        /// http://isap.sejm.gov.pl/DetailsServlet?id=WDU20061390993
        /// </summary>
        /// <param name="sequence">PESEL</param>
        /// <returns>True if validated, false otherwise</returns>
        public override bool Validate(string sequence)
        {
            if (!validateRegex(sequence, regex))
                return false;
            int sum = sumOfProducts(sequence, weights);
            if (!compareModulo(sum, modulo, comparator))
                return false;

            return true;
        }

        /// <summary>
        /// Executes full validation
        /// http://www.mswia.gov.pl/portal/pl/381/32/PESEL.html
        /// http://isap.sejm.gov.pl/DetailsServlet?id=WDU20061390993
        /// </summary>
        /// <param name="sequence">PESEL</param>
        /// <param name="male">Sex</param>
        /// <param name="birthday">Day of birth</param>
        /// <returns>True if validated, false otherwise</returns>
        public bool Validate(string sequence, bool male, DateTime birthday)
        {
            return Validate(sequence) && (GetDateOfBirth(sequence) == birthday) &&
                (IsMale(sequence) == male);
        }

        /// <summary>
        /// Returns birthday from given PESEL sequence
        /// </summary>
        /// <param name="sequence">PESEL</param>
        /// <returns>Birthday</returns>
        public DateTime GetDateOfBirth(string sequence)
        {
            //if (!Validate(sequence))
            //    throw new Exception("PESEL is invalid: " + sequence);

            int year = int.Parse(sequence.Substring(0, 2));
            int month = int.Parse(sequence.Substring(2, 2));
            int day = int.Parse(sequence.Substring(4, 2));

            if (month > 80)
            {
                month -= 80;
                year += 1800;
            }
            else if (month < 13)
            {
                year += 1900;
            }
            else if ((month > 20) && (month < 33))
            {
                month -= 20;
                year += 2000;
            }
            else if ((month > 40) && (month < 53))
            {
                month -= 40;
                year += 2100;
            }
            else if ((month > 60) && (month < 73))
            {
                month -= 60;
                year += 2200;
            }

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Returns sex from given PESEL
        /// </summary>
        /// <param name="sequence">PESEL</param>
        /// <returns>True if male, false if female</returns>
        public bool IsMale(string sequence)
        {
            //if (!Validate(sequence))
            //    throw new Exception("PESEL is invalid: " + sequence);

            string male = "13579";
            return male.Contains(sequence[9]);
        }
    }
}
