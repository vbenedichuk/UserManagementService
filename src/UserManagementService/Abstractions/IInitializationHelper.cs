namespace UserManagementService.Abstractions
{
    /// <summary>
    /// Initialization helper abstraction.
    /// </summary>
    interface IInitializationHelper
    {
        /// <summary>
        /// Initialize database with users and roles.
        /// </summary>
        void Initialize();
    }
}
