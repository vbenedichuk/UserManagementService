﻿using System.Collections.Generic;

namespace UserManagementService.Models.UI
{
    /// <summary>
    /// User list item.
    /// </summary>
    public class UiUserListItem
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// User name.
        /// </summary>
        public virtual string UserName { get; set; }
        /// <summary>
        /// Is two factor authentication enabled for the user.
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public virtual string Email { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        public virtual string PhoneNumber { get; set; }
        /// <summary>
        /// Is phone number confirmed.
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }
        /// <summary>
        /// User roles
        /// </summary>
        public List<UiRoleListItem> Roles { get; set; }

        public UiUserListItem()
        {
            Roles = new List<UiRoleListItem>();
        }
    }
}
