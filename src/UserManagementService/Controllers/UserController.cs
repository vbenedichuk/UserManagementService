using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagementService.Models.Controllers;
using UserManagementService.Models.Database;

namespace UserManagementService.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(
            [FromQuery] string userId,
            [FromQuery] string passwordResetToken)
        {
            var model = new ResetPasswordViewModel
            {
                Code = passwordResetToken,
                UserId = userId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string button)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                var resetResult = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (resetResult.Succeeded)
                {
                    return View("PasswordChanged");
                }
                else
                {
                    foreach(var error in resetResult.Errors)
                    {
                        _logger.LogError("Unable to change password. UserId: {0}, Error Code: {1}, Error Description; {2}", model.UserId, error.Code, error.Description);
                    }
                    return View("PasswordChangeError");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to change password.", ex);
                return View("PasswordChangeError");
            }
        }
    }
}