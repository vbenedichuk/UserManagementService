using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementService.Models.UI
{
    public class UiUserDetails : UiUserListItem
    {
        public UiUserDetails()
        {
            Roles = new List<UiRoleListItem>();
        }

        public string Password { get; set; }
        public string Password2 { get; set; }
        public List<UiRoleListItem> Roles { get; set; }
    }
}
