using Newtonsoft.Json;
using System.Collections.Generic;

namespace UserManagementService.Models.Initialization
{
    /// <summary>
    /// Defines initialization file structure.
    /// </summary>
    public class InitializationFile
    {
        /// <summary>
        /// Roles that should be created
        /// </summary>
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        /// <summary>
        /// Users that should be created.
        /// </summary>
        [JsonProperty("users")]
        public List<InitializationUser> Users { get; set; }
    }
}
