using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models.Controllers
{
    /// <summary>
    /// Reset password UI model
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Password length length can't be less than 8 or greater than 15")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "Password shuold contain at least one number, one uppercase letter, one lowercase letter and one special symbol.")]
        public string Password { get; set; }
        /// <summary>
        /// Password copy
        /// </summary>
        [Required]
        [Display(Name = "Repeat password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string Password2 { get; set; }
        /// <summary>
        /// Password reset code
        /// </summary>
        [HiddenInput]
        public string Code { get; set; }
        /// <summary>
        /// User Id.
        /// </summary>
        [HiddenInput]
        public string UserId { get; set; }
    }
}
