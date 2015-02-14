using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SoftwareMind.Scripts
{
    /// <summary>
    /// Reprezentuje b³¹d kompilacji
    /// </summary>
    [Serializable]
    public class CompilerErrorException : Exception
    {

        /// <summary>
        /// Inicjalizujê obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="errors">B³êdy.</param>
        /// <param name="message">Komunikat.</param>
        /// <param name="innerException">Wewnêtrzny b³¹d.</param>
        public CompilerErrorException(CompilerErrorCollection errors, string message, Exception innerException)
            : base(message, innerException)
        {
            Errors = errors;
        }

        /// <summary>
        /// Inicjalizujê obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="errors">B³êdy.</param>
        /// <param name="message">Komunikat</param>
        public CompilerErrorException(CompilerErrorCollection errors, string message)
            : this(errors, message, null)
        {
        }

        /// <summary>
        /// Inicjalizujê obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="message">Komunikat.</param>
        /// <param name="innerException">Wewnêtrzny b³¹d.</param>
        public CompilerErrorException(string message, Exception innerException)
            : this(new CompilerErrorCollection(), message, innerException)
        {
        }

        /// <summary>
        /// Inicjalizujê obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/> przetrzymuje zserializowane w³aœciwosci obiektu.</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/> zawiera kontekst wykonania.</param>
        /// <exception cref="T:System.ArgumentNullException">jest rzucany jeœli <paramref name="info"/> jest nullem. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">Jeœli nazwa klasy jest nullem albo <see cref="P:System.Exception.HResult"/> wynosi (0). </exception>
        protected CompilerErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Inicjalizujê obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="message">Komunikat.</param>
        public CompilerErrorException(string message)
            : this(new CompilerErrorCollection(), message)
        {
        }

        /// <summary>
        /// Inicjalizujê obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        public CompilerErrorException()
            : this(new CompilerErrorCollection(), String.Empty)
        {
        }



        /// <summary>
        /// Pozwala poraæ i ustawiæ kolekcjê b³êdów.
        /// </summary>
        /// <value>Kolekcja b³êdów kompilacji.</value>
        public CompilerErrorCollection Errors
        {
            get;
            private set;
        }




        /// <summary>
        /// Ustawia <see cref="T:System.Runtime.Serialization.SerializationInfo"/> w³aœciwosciami obiektu
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/> przetrzymuje zserializowane w³aœciwosci obiektu.</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/> zawiera aktualny kontkest wykonania.</param>
        /// <exception cref="T:System.ArgumentNullException">jest rzucany jeœli <paramref name="info"/> jest nullem. </exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Errors", Errors);
        }
    }
}
