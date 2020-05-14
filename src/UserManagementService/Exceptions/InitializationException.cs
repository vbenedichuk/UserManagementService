using System;

namespace UserManagementService.Exceptions
{
    /// <summary>
    /// Initialization exception.
    /// </summary>
    [Serializable]
    public class InitializationException : Exception
    {

        /// <summary>
        /// Initializes new instance of <see cref="InitializationException"/>.
        /// </summary>
        /// <param name="message">Error message.</param>
        public InitializationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="InitializationException"/>.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public InitializationException(string message, Exception innerException) : base(message, innerException)
        {
        }


        /// <summary>
        /// Initializes new instance of <see cref="InitializationException"/>.
        /// </summary>
        /// <param name="info"> The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown</param>
        /// <param name="context"> The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        protected InitializationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
