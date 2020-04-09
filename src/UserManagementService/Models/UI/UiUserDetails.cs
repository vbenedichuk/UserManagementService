namespace UserManagementService.Models.UI
{
    /// <summary>
    /// User details data structure.
    /// </summary>
    public class UiUserDetails : UiUserListItem
    {
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Password 2
        /// </summary>
        public string Password2 { get; set; }
    }
}
