using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SoftwareMind.Shared.Mef
{
    /// <summary>
    /// Eksportuje element z metadaną(aliasem)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExportWithMetadataAttribute : ExportAttribute
    {
        /// <summary>
        /// Tworzy atrybut
        /// </summary>
        /// <param name="type">Typ exportowanego interfejsu</param>
        /// <param name="metaData"> </param>
        public ExportWithMetadataAttribute(Type type, params object[] metaData)
            : base(GetContractName(type, metaData), type)
        {
        }

        public static string GetContractName(Type type, params KeyValuePair<string, object>[] metaData)
        {
            return AttributedModelServices.GetContractName(type) + "~" + string.Join(
                "|",
                metaData
                    .OrderBy(m => m.Key)
                    .Select(m => string.Format("{0}={1}", m.Key, ConvertValueToString(m.Value)))
            );
        }

        private static string GetContractName(Type type, object[] metaData)
        {
            if (metaData.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid metaData format", "metaData");
            }

            KeyValuePair<string, object>[] newMetaData = new KeyValuePair<string, object>[metaData.Length / 2];
            for (int i = 0, j = 0; i < metaData.Length; i += 2, j++)
            {
                if (!(metaData[i] is string))
                {
                    throw new ArgumentException("Invalid metaData format", "metaData");
                }

                newMetaData[j] = new KeyValuePair<string, object>((string) metaData[i], metaData[i + 1]);
            }

            return GetContractName(type, newMetaData);
        }

        private static string ConvertValueToString(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value is string)
            {
                return string.Format(@"""{0}""", value);
            }

            if (value is Type)
            {
                return ((Type) value).FullName;
            }

            if (value.GetType().IsPrimitive)
            {
                return Convert.ToString(value);
            }

            throw new ArgumentException("Not supported type", "value");
        }
    }
}
