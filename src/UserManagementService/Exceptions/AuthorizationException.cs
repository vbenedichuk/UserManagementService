using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementService.Exceptions
{
    /// <summary>
    /// Authorization exception.
    /// </summary>
    [Serializable]
    public class AuthorizationException : Exception
    {

        /// <summary>
        /// Initializes new instance of <see cref="AuthorizationException"/>.
        /// </summary>
        /// <param name="message">Error message.</param>
        public AuthorizationException(string message) : base(message)
        {
        }
    }
}
