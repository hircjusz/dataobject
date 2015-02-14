using System;
using System.Collections.Generic;

namespace SoftwareMind
{
    /// <summary>
    /// Pozwala tworzyć instancje IEqualityComparer na podstawie wyrażeń lambda.
    /// Przed porównaniem lub pobraniem hashcodu wejście jest przepuszczane przez wyrażenie lambda, dzięki czemu można dokonać jakiejś konwersji (np. s => s.ToLower()).
    /// </summary>
    /// <typeparam name="T">Typ porównywanych danych</typeparam>
    public class LambdaEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, object> convert;

        public LambdaEqualityComparer(Func<T, object> convert)
        {
            this.convert = convert;
        }

        public bool Equals(T x, T y)
        {
            return convert(x).Equals(convert(y));
        }

        public int GetHashCode(T obj)
        {
            return convert(obj).GetHashCode();
        }
    }
}
