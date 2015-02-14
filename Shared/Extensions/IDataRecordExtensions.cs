using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SoftwareMind.Utils.Extensions
{
    public static class IDataRecordExtensions
    {
        private static readonly IDictionary<Type, Func<IDataRecord, string, object>> typeToMethodMap = new Dictionary<Type, Func<IDataRecord, string, object>>(){
            { typeof(Boolean),  GetBooleanOrNull },
            { typeof(Boolean?),  GetBooleanOrNull },
            { typeof(DateTime), GetDateTimeOrNull },
            { typeof(Decimal),  GetDecimalOrNull },
            { typeof(Int32),    GetInt32OrNull },
            { typeof(Int64),    GetInt64OrNull },
            { typeof(Decimal?),  GetDecimalOrNull },
            { typeof(Int32?),    GetInt32OrNull },
            { typeof(Int64?),    GetInt64OrNull },
            { typeof(String),   GetStringOrNull }
        };

        public static object GetByType(this IDataRecord record, string name, Type type)
        {
            Func<IDataRecord, string, object> callback = typeToMethodMap[type];
            return callback(record, name);
        }

        public static object GetByType<T>(this IDataRecord record, string name)
        {
            return record.GetByType(name, typeof(T));
        }

        public static object GetBooleanOrNull(this IDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetInt32(index) > 0;
        }

        public static object GetBooleanOrNull(this IDataRecord record, string name)
        {
            return record.GetBooleanOrNull(record.GetOrdinal(name));
        }

        public static object GetDateTimeOrNull(this IDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetDateTime(index);
        }

        public static object GetDateTimeOrNull(this IDataRecord record, string name)
        {
            return record.GetDateTimeOrNull(record.GetOrdinal(name));
        }

        public static object GetDecimalOrNull(this IDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetDecimal(index);
        }

        public static object GetDecimalOrNull(this IDataRecord record, string name)
        {
            return record.GetDecimalOrNull(record.GetOrdinal(name));
        }

        public static object GetInt32OrNull(this IDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetInt32(index);
        }

        public static object GetInt32OrNull(this IDataRecord record, string name)
        {
            return record.GetInt32OrNull(record.GetOrdinal(name));
        }

        public static object GetInt64OrNull(this IDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetInt64(index);
        }

        public static object GetInt64OrNull(this IDataRecord record, string name)
        {
            return record.GetInt64OrNull(record.GetOrdinal(name));
        }

        public static object GetStringOrNull(this IDataRecord record, int index)
        {
            if (record.IsDBNull(index))
                return null;

            return record.GetString(index);
        }

        public static object GetStringOrNull(this IDataRecord record, string name)
        {
            return record.GetStringOrNull(record.GetOrdinal(name));
        }
    }
}
