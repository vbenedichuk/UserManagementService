using System.Threading.Tasks;
using UserManagementService.Models.Database;
using UserManagementService.Models.UI;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _environment;

        public AuthController(IIdentityServerInteractionService interaction,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHostingEnvironment environment)
        {
            _interaction = interaction;
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null && context != null)
                    {
                        //await HttpContext.SignInAsync(user.SubjectId, user.UserName);
                        return new JsonResult(new { RedirectUrl = model.ReturnUrl, IsOk = true });
                    }
                }
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("Error")]
        public async Task<IActionResult> Error(string errorId)
        {
            var message = await _interaction.GetErrorContextAsync(errorId);

            if (message != null)
            {
                if (!_environment.IsDevelopment())
                {
                    message.ErrorDescription = null;
                }
            }

            return Ok(message);
        }
    }
}