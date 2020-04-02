namespace AuthService.Models.Configuration
{
    /// <summary>
    /// CertificateConfiguration config section.
    /// </summary>
    public class CertificateConfiguration
    {
        /// <summary>
        /// Certificate file name including path.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Certificate password.
        /// </summary>
        public string Password { get; set; }
    }
}
