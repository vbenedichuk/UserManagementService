using System;
using System.Collections.Generic;
using System.Text;

namespace MemorizeThat.EmailManagement.SendGrid.Exceptions
{
    /// <summary>
    /// Send grid email sender exception.
    /// </summary>
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
    }
}
