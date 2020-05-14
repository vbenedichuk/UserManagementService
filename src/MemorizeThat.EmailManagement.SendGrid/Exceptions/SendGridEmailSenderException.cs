using System;

namespace MemorizeThat.EmailManagement.SendGrid.Exceptions
{
    /// <summary>
    /// Send grid email sender exception.
    /// </summary>
    [Serializable]
    public class SendGridEmailSenderException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="SendGridEmailSenderException"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        public SendGridEmailSenderException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="SendGridEmailSenderException"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception.</param>
        public SendGridEmailSenderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="SendGridEmailSenderException"/>.
        /// </summary>
        /// <param name="info"> The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown</param>
        /// <param name="context"> The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        protected SendGridEmailSenderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
