using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using UserManagementService.Models.Database;
using UserManagementService.Models.UI;
using UserManagementService.Models.UI.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(
            [NotNull] UserManager<ApplicationUser> userManager,
            [NotNull] IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        public UiList<UiUserListItem> Get([FromQuery]string name,
            [FromQuery]int? skip,
            [FromQuery]int? take)
        {
            var result = new UiList<UiUserListItem>();
            var query = _userManager.Users;
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.UserName.Contains(name, System.StringComparison.OrdinalIgnoreCase));
            }
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }
            result.TotalCount = query.Count();
            result.Items = _mapper.Map<List<UiUserListItem>>(query.ToList());
            return result;
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return Ok(_mapper.Map<UiUserDetails>(user));
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UiUserDetails userDetails)
        {
            if(userDetails == null || !ModelState.IsValid || userDetails.Id != "new")
            {
                return BadRequest(new UiResponse(false, "invalid_input", "Invalid input."));
            }

            var errorList = new List<UiResponseMessage>();

            var userWithTheSameName = await _userManager.FindByNameAsync(userDetails.UserName);
            if(userWithTheSameName != null)
            {
                errorList.Add(new UiResponseMessage { Code = "duplicate_name", Message = "User with the same name is already exists." });
            }

            var userWithTheSameEmail = await _userManager.FindByEmailAsync(userDetails.Email);
            if(userWithTheSameEmail != null)
            {
                errorList.Add(new UiResponseMessage {
                    Code = "duplicate_email", Message = "User with the same email is already exists."});
            }

            if(errorList.Count > 0)
            {
                return BadRequest(new UiResponse(false, errorList));
            }

            if(userDetails.Id == "new")
            {
                userDetails.Id = Guid.NewGuid().ToString();
            }

            var applicationUser = _mapper.Map<ApplicationUser>(userDetails);
            
            var userCreationResult = await _userManager.CreateAsync(applicationUser, userDetails.Password);

            if (!userCreationResult.Succeeded)
            {
                foreach(var identityError in userCreationResult.Errors)
                {
                    errorList.Add(new UiResponseMessage { Code = "identity_error", Message = identityError.Description });
                }
                return BadRequest(new UiResponse(false, errorList));                
            }

            return Ok(new UiResponse(true, "user_created", "User created successfully."));

        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UiUserDetails userDetails)
        {
            if (userDetails == null || !ModelState.IsValid || userDetails.Id != id || string.IsNullOrWhiteSpace(userDetails.Id))
            {
                return BadRequest(new UiResponse(false, "invalid_input", "Invalid input."));
            }

            var errorList = new List<UiResponseMessage>();

            var userWithTheSameName = await _userManager.FindByNameAsync(userDetails.UserName);
            if (userWithTheSameName != null && userWithTheSameName.Id != id)
            {
                errorList.Add(new UiResponseMessage { Code = "duplicate_name", Message = "User with the same name is already exists." });
            }

            var userWithTheSameEmail = await _userManager.FindByEmailAsync(userDetails.Email);
            if (userWithTheSameEmail != null && userWithTheSameEmail.Id != id)
            {
                errorList.Add(new UiResponseMessage
                {
                    Code = "duplicate_email",
                    Message = "User with the same email is already exists."
                });
            }

            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                errorList.Add(new UiResponseMessage
                {
                    Code = "internal_error",
                    Message = "User is not available for editing."
                });
            }

            if (errorList.Count > 0)
            {
                return BadRequest(new UiResponse(false, errorList));
            }
            user.Email = userDetails.Email;
            user.PhoneNumber = userDetails.PhoneNumber;
            user.UserName = userDetails.UserName;
            var updateUserResult = await _userManager.UpdateAsync(user);
            errorList.AddRange(IdentityResultToResponseMessages(updateUserResult));
            if (errorList.Count > 0)
            {
                return BadRequest(new UiResponse(false, errorList));
            }
            if(!string.IsNullOrWhiteSpace(userDetails.Password) && 
                userDetails.Password == userDetails.Password2)
            {
                if (await _userManager.HasPasswordAsync(user))
                {
                    await _userManager.RemovePasswordAsync(user);
                }
                var updatePasswordResult = await _userManager.AddPasswordAsync(user, userDetails.Password);

                errorList.AddRange(IdentityResultToResponseMessages(updatePasswordResult));
                return BadRequest(new UiResponse(false, errorList));
            }
            return Ok(new UiResponse(true, "user_updated", "User updated successfully."));
        }

        //TODO: Move to helper class. 
        private IEnumerable<UiResponseMessage> IdentityResultToResponseMessages(IdentityResult identityResult)
        {
            var result = new List<UiResponseMessage>();

            if (!identityResult.Succeeded)
            {
                foreach (var identityError in identityResult.Errors)
                {
                    result.Add(new UiResponseMessage { Code = "identity_error", Message = identityError.Description });
                }
            }
            return result;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                var result = await _userManager.DeleteAsync(user);
            }
            return Ok();
        }
    }
}
