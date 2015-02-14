using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SoftwareMind.Shared.Helper;

namespace SoftwareMind.Shared.Config
{
    internal class ConfigurationWrapper
    {
        private static readonly TypeFilter dictionaryFilter = (i, o) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        private static readonly TypeFilter enumerableFilter = (i, o) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        private readonly NameValueCollection _config;
        private readonly string _prefix;

        internal ConfigurationWrapper(NameValueCollection config, string prefix)
        {
            this._config = config;
            this._prefix = prefix;
        }

        public object Get(Type type, string key, DefaultValueAttribute defaultValue)
        {
            if (type.IsEnum)
            {
                return this.GetEnum(type, key, defaultValue);
            }
            if (dictionaryFilter(type, null) || type.FindInterfaces(dictionaryFilter, null).Any())
            {
                return this.GetDictionary(type, key, defaultValue);
            }
            if ((enumerableFilter(type, null) || type.FindInterfaces(enumerableFilter, null).Any()) && !typeof(string).IsAssignableFrom(type))
            {
                return this.GetEnumerable(type, key, defaultValue);
            }

            return this.GetPrimitive(type, key, defaultValue);
        }

        private object GetPrimitive(Type type, string key, DefaultValueAttribute defaultValue)
        {
            object value = this._config[this.GetKey(key)];
            if (value == null && defaultValue != null && defaultValue.Value != null)
            {
                if (type.IsInstanceOfType(defaultValue.Value))
                {
                    return defaultValue.Value;
                }
                value = defaultValue.Value;
            }
            if (value != null)
            {
                return type.IsInstanceOfType(value) ? value : Convert.ChangeType(value, type);
            }

            // Default value
            return type.IsValueType || type.IsPrimitive ? Activator.CreateInstance(type) : null;
        }

        private object GetEnum(Type type, string key, DefaultValueAttribute defaultValue)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException();
            }

            object value = this._config[this.GetKey(key)];
            if (value == null && defaultValue != null && defaultValue.Value != null)
            {
                if (type.IsInstanceOfType(defaultValue.Value))
                {
                    return defaultValue.Value;
                }
                value = defaultValue.Value;
            }
            if (value != null)
            {
                return Enum.Parse(type, value.ToString());
            }

            // Default value
            return Activator.CreateInstance(type);
        }

        private object GetEnumerable(Type type, string key, DefaultValueAttribute defaultValue)
        {
            object value = this._config[this.GetKey(key)];
            if (value == null && defaultValue != null && defaultValue.Value != null)
            {
                value = defaultValue.Value;
            }
            if (value != null)
            {
                Type enumType = enumerableFilter(type, null) ? type : type.FindInterfaces(enumerableFilter, null).Single();
                Type valueType = enumType.GetGenericArguments()[0];

                return this.ToEnumerable(Convert.ToString(value).Split(','), valueType);
            }
            return null;
        }

        private object GetDictionary(Type type, string key, DefaultValueAttribute defaultValue)
        {
            object value = this._config[this.GetKey(key)];
            if (value == null && defaultValue != null && defaultValue.Value != null)
            {
                value = defaultValue.Value;
            }
            if (value != null)
            {
                Type dictType = dictionaryFilter(type, null) ? type : type.FindInterfaces(dictionaryFilter, null).Single();
                Type keyType = dictType.GetGenericArguments()[0];
                Type valueType = dictType.GetGenericArguments()[1];

                return this.ToDictionary(Convert.ToString(value).Split(',').Select(v => v.Split('=')), keyType, valueType);
            }
            return null;
        }

        private static readonly IDictionary<Type, Func<object, object>> EnumerableConverterCache = new Dictionary<Type, Func<object, object>>();
        private static Func<object, object> CreateToEnumerableMethod(Type valueType)
        {
            if (!EnumerableConverterCache.ContainsKey(valueType))
            {
                IEnumerable<string> values = null;
                MethodInfo method = ReflectionHelper
                    .GetMethodInfo(() => values.Select(x => x))
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(typeof(string), valueType);

                ParameterExpression valueParameter = Expression.Parameter(typeof(string), "y");

                Delegate valueSelector = Expression.Lambda(valueParameter, valueParameter).Compile();

                return EnumerableConverterCache[valueType] = v => method.Invoke(null, new object[] { v, valueSelector });
            }

            return EnumerableConverterCache[valueType];
        }

        private object ToEnumerable(IEnumerable<string> values, Type valueType)
        {
            Func<object, object> method = CreateToEnumerableMethod(valueType);

            return method(values);
        }

        private static readonly IDictionary<Tuple<Type, Type>, Func<object, object>> DictionaryConverterCache = new Dictionary<Tuple<Type, Type>, Func<object, object>>();
        private static Func<object, object> CreateToDictionaryMethod(Type keyType, Type valueType)
        {
            var key = new Tuple<Type, Type>(keyType, valueType);
            if (!DictionaryConverterCache.ContainsKey(key))
            {
                IEnumerable<string[]> values = null;
                MethodInfo method = ReflectionHelper
                    .GetMethodInfo(() => values.ToDictionary(x => x, x => x))
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(typeof(string[]), keyType, valueType);

                ParameterExpression keyParameter = Expression.Parameter(typeof(string[]), "x");
                ParameterExpression valueParameter = Expression.Parameter(typeof(string[]), "y");

                Delegate keySelector = Expression.Lambda(Expression.ArrayAccess(keyParameter, Expression.Constant(0)), keyParameter).Compile();
                Delegate valueSelector = Expression.Lambda(Expression.ArrayAccess(valueParameter, Expression.Constant(1)), valueParameter).Compile();

                return DictionaryConverterCache[key] = v => method.Invoke(null, new object[] { v, keySelector, valueSelector });
            }

            return DictionaryConverterCache[key];
        }

        private object ToDictionary(IEnumerable<string[]> values, Type keyType, Type valueType)
        {
            Func<object, object> method = CreateToDictionaryMethod(keyType, valueType);

            return method(values);
        }

        private string GetKey(string key)
        {
            return this._prefix == null ?
                key :
                string.Format("{0}.{1}", this._prefix, key);
        }
    }
}