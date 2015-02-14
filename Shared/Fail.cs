using System;
using System.Globalization;

namespace SoftwareMind.Utils
{
    public static class Fail
    {
        private const string Null = "null";
        private static readonly CultureInfo ci = CultureInfo.InvariantCulture;

        public static void IfNull<T>(T value)
        {
            Fail.IfNull(value, null);
        }

        public static void IfNull<T>(T value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name ?? Null);
            }
        }

        public static void IfNullOrEmpty(string value)
        {
            Fail.IfNullOrEmpty(value, null);
        }

        public static void IfNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw (value == null)
                    ? new ArgumentNullException(name ?? Null)
                    : new ArgumentException(name ?? Null);
            }
        }

        public static void IfNotEqual<T>(T value, T compareTo) where T : IEquatable<T>
        {
            Fail.IfNotEqual(value, compareTo, null);
        }

        public static void IfEqual<T>(T value, T compareTo) where T : IEquatable<T>
        {
            Fail.IfEqual(value, compareTo, null);
        }

        public static void IfNotEqual<T>(T value, T compareTo, string name) where T : IEquatable<T>
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(compareTo, "compareTo");

            if (!value.Equals(compareTo))
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "value != {0}", compareTo));
            }
        }

        public static void IfEqual<T>(T value, T compareTo, string name) where T : IEquatable<T>
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(compareTo, "compareTo");

            if (value.Equals(compareTo))
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "value == {0}", compareTo));
            }
        }

        public static void IfNotReferenceEqual<T>(T value, T compareTo) where T : class
        {
            Fail.IfNotReferenceEqual(value, compareTo, null);
        }

        public static void IfNotReferenceEqual<T>(T value, T compareTo, string name) where T : class
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(compareTo, "compareTo");

            if (value != compareTo)
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "value != {0}", compareTo));
            }
        }

        public static void IfReferenceEqual<T>(T value, T compareTo) where T : class
        {
            Fail.IfReferenceEqual(value, compareTo, null);
        }

        public static void IfReferenceEqual<T>(T value, T compareTo, string name) where T : class
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(compareTo, "compareTo");

            if (value == compareTo)
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "value == {0}", compareTo));
            }
        }

        public static void IfLessThan<T>(T value, T max) where T : IComparable<T>
        {
            Fail.IfLessThan(value, max, null);
        }

        public static void IfLessThan<T>(T value, T max, string name) where T : IComparable<T>
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(max, "max");

            if (value.CompareTo(max) < 0)
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "value < {0}", max));
            }
        }

        public static void IfLessThanOrEqual<T>(T value, T max) where T : IComparable<T>
        {
            Fail.IfLessThanOrEqual(value, max, null);
        }

        public static void IfLessThanOrEqual<T>(T value, T max, string name) where T : IComparable<T>
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(max, "max");

            if (value.CompareTo(max) <= 0)
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "value <= {0}", max));
            }
        }

        public static void IfGreaterThan<T>(T value, T min) where T : IComparable<T>
        {
            Fail.IfGreaterThan(value, min, null);
        }

        public static void IfGreaterThan<T>(T value, T min, string name) where T : IComparable<T>
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(min, "min");

            if (value.CompareTo(min) > 0)
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "value > {0}", min));
            }
        }

        public static void IfGreaterThanOrEqual<T>(T value, T min) where T : IComparable<T>
        {
            Fail.IfGreaterThanOrEqual(value, min, null);
        }

        public static void IfGreaterThanOrEqual<T>(T value, T min, string name) where T : IComparable<T>
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(min, "min");

            if (value.CompareTo(min) >= 0)
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "value >= {0}", min));
            }
        }

        public static void IfOutOfRangeInclusive<T>(T value, T min, T max) where T : IComparable<T>
        {
            Fail.IfOutOfRangeInclusive(value, min, max, null);
        }

        public static void IfOutOfRangeInclusive<T>(T value, T min, T max, string name) where T : IComparable<T>
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(min, "min");
            Fail.IfNull(max, "max");

            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "{0} < value < {1}", min, max));
            }
        }

        public static void IfOutOfRangeExclusive<T>(T value, T min, T max) where T : IComparable<T>
        {
            Fail.IfOutOfRangeExclusive(value, min, max, null);
        }

        public static void IfOutOfRangeExclusive<T>(T value, T min, T max, string name) where T : IComparable<T>
        {
            Fail.IfNull(value, "value");
            Fail.IfNull(min, "min");
            Fail.IfNull(max, "max");

            if (value.CompareTo(min) <= 0 || value.CompareTo(max) >= 0)
            {
                throw new ArgumentOutOfRangeException(name ?? Null, value, string.Format(ci, "{0} <= value <= {1}", min, max));
            }
        }

        public static void Because(string message)
        {
            throw new ArgumentException(message);
        }

        public static void IfTrue(bool condition)
        {
            Fail.IfNotEqual(condition, false);
        }

        public static void IfTrue(bool condition, string message)
        {
            Fail.IfNotEqual(condition, false, message);
        }

        public static void IfFalse(bool condition)
        {
            Fail.IfNotEqual(condition, true);
        }

        public static void IfFalse(bool condition, string message)
        {
            Fail.IfNotEqual(condition, true, message);
        }
    }
}
