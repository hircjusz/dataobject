using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SoftwareMind.Scripts
{
    /// <summary>
    /// Reprezentuje b��d kompilacji
    /// </summary>
    [Serializable]
    public class CompilerErrorException : Exception
    {

        /// <summary>
        /// Inicjalizuj� obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="errors">B��dy.</param>
        /// <param name="message">Komunikat.</param>
        /// <param name="innerException">Wewn�trzny b��d.</param>
        public CompilerErrorException(CompilerErrorCollection errors, string message, Exception innerException)
            : base(message, innerException)
        {
            Errors = errors;
        }

        /// <summary>
        /// Inicjalizuj� obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="errors">B��dy.</param>
        /// <param name="message">Komunikat</param>
        public CompilerErrorException(CompilerErrorCollection errors, string message)
            : this(errors, message, null)
        {
        }

        /// <summary>
        /// Inicjalizuj� obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="message">Komunikat.</param>
        /// <param name="innerException">Wewn�trzny b��d.</param>
        public CompilerErrorException(string message, Exception innerException)
            : this(new CompilerErrorCollection(), message, innerException)
        {
        }

        /// <summary>
        /// Inicjalizuj� obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/> przetrzymuje zserializowane w�a�ciwosci obiektu.</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/> zawiera kontekst wykonania.</param>
        /// <exception cref="T:System.ArgumentNullException">jest rzucany je�li <paramref name="info"/> jest nullem. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">Je�li nazwa klasy jest nullem albo <see cref="P:System.Exception.HResult"/> wynosi (0). </exception>
        protected CompilerErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Inicjalizuj� obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        /// <param name="message">Komunikat.</param>
        public CompilerErrorException(string message)
            : this(new CompilerErrorCollection(), message)
        {
        }

        /// <summary>
        /// Inicjalizuj� obiekt klasy <see cref="CompilerErrorException"/>.
        /// </summary>
        public CompilerErrorException()
            : this(new CompilerErrorCollection(), String.Empty)
        {
        }



        /// <summary>
        /// Pozwala pora� i ustawi� kolekcj� b��d�w.
        /// </summary>
        /// <value>Kolekcja b��d�w kompilacji.</value>
        public CompilerErrorCollection Errors
        {
            get;
            private set;
        }




        /// <summary>
        /// Ustawia <see cref="T:System.Runtime.Serialization.SerializationInfo"/> w�a�ciwosciami obiektu
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/> przetrzymuje zserializowane w�a�ciwosci obiektu.</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/> zawiera aktualny kontkest wykonania.</param>
        /// <exception cref="T:System.ArgumentNullException">jest rzucany je�li <paramref name="info"/> jest nullem. </exception>
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
