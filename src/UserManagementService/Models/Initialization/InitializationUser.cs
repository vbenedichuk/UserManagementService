using Newtonsoft.Json;
using System.Collections.Generic;

namespace UserManagementService.Models.Initialization
{
    /// <summary>
    /// Provides structure for User information in initialization file
    /// </summary>
    public class InitializationUser
    {
        /// <summary>
        /// User Name
        /// </summary>
        [JsonProperty("user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// User claims
        /// </summary>
        [JsonProperty("claims")]
        public Dictionary<string, string> Claims { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
    }
}
