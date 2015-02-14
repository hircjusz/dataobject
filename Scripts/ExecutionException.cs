using System;

namespace SoftwareMind.Scripts
{
    /// <summary>
    /// Reprezentuje bład wykonania skryptu
    /// </summary>
    [Serializable]
    public class ExecutionException : Exception
    {

        /// <summary>
        /// Inicjalizuję obiekt klasy <see cref="ExecutionException"/>.
        /// </summary>
        /// <param name="message">Komunikat.</param>
        /// <param name="inner">Wewnetrzny błąd.</param>
        public ExecutionException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Inicjalizuję obiekt klasy <see cref="ExecutionException"/>.
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/> przetrzymuje zserializowane właściwosci obiektu.</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/> zawiera kontekst wykonania.</param>
        /// <exception cref="T:System.ArgumentNullException">jest rzucany jeśli <paramref name="info"/> jest nullem. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">Jeśli nazwa klasy jest nullem albo <see cref="P:System.Exception.HResult"/> wynosi (0). </exception>
        protected ExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Inicjalizuję obiekt klasy <see cref="ExecutionException"/>.
        /// </summary>
        /// <param name="message">Komunikat.</param>
        public ExecutionException(string message) : base(message) { }

        /// <summary>
        /// Inicjalizuję obiekt klasy <see cref="ExecutionException"/>.
        /// </summary>
        public ExecutionException() { }
    }
}
