using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Validators
{
    public class ValidatorREGON : Validator
    {
        public ValidatorREGON() : base()
        {
            getParametersFromConfigFile("RegexREGON9", "WeightsREGON9", 
                "RegexREGON14", "WeightsREGON14", "ModuloREGON");
        }

        /// <summary>
        /// Validates REGON
        /// </summary>
        /// <param name="sequence">REGON</param>
        /// <returns>True if validated, false otherwise</returns>
        public override bool Validate(string sequence)
        {
            if (!validateRegex(sequence, regex9) && !validateRegex(sequence, regex14))
                return false;

            //validate 9 numbers
            int sum = sumOfProducts(sequence, weights9);

            int mod = sum % modulo;
            if (mod != 10)
            {
                if (mod != int.Parse(sequence[8].ToString()))
                    return false;
            }
            else
                if (0 != int.Parse(sequence[8].ToString()))
                    return false;

            //validate 14 numbers
            if (sequence.Length == 14)
            {
                sum = sumOfProducts(sequence, weights14);
                int mod14 = sum % modulo;
                if (mod14 != 10)
                {
                    if (!compareModulo(sum, modulo, sequence.Last()))
                        return false;   
                }
                else
                    if (0 != int.Parse(sequence.Last().ToString()))
                        return false;
            }

            return true;
        }

        protected string regex9;
        protected string regex14;
        protected int[] weights9;
        protected int[] weights14;
        new protected int modulo;

        /// <summary>
        /// Retrieves parameters from configuration file
        /// </summary>
        /// <param name="keyRegex">Configuration, app settings key</param>
        /// <param name="keyRegex">Configuration, app settings key</param>
        /// <param name="keyWeights">Configuration, app settings key</param>
        /// <param name="keyWeights">Configuration, app settings key</param>
        /// <param name="keyModulo">Configuration, app settings key</param>
        protected void getParametersFromConfigFile(string keyRegex9, 
            string keyWeights9, string keyRegex14, 
            string keyWeights14, string keyModulo)
        {
            try
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyRegex9))
                    regex9 = ConfigurationManager.AppSettings[keyRegex9];
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyRegex14))
                    regex14 = ConfigurationManager.AppSettings[keyRegex14];
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyWeights9))
                    weights9 = (from n in ConfigurationManager.AppSettings[keyWeights9]
                               select int.Parse(n.ToString())).ToArray<int>();
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyWeights14))
                    weights14 = (from n in ConfigurationManager.AppSettings[keyWeights14]
                                select int.Parse(n.ToString())).ToArray<int>();
                if (ConfigurationManager.AppSettings.AllKeys.Contains(keyModulo))
                    modulo = int.Parse(ConfigurationManager.AppSettings[keyModulo]);
            }
            catch (Exception exc)
            {
                string error = "There was an exception while reading configuration file: " + exc.Message;
                throw new Exception(error, exc);
            }
        }
    }
}
