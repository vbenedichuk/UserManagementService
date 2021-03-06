﻿using System;
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
using System.Security.Claims;

namespace UserManagementService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            [NotNull] UserManager<ApplicationUser> userManager,
            [NotNull] IMapper mapper,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        // GET: api/User
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Administators only")]
        public async Task<UiList<UiUserListItem>> Get(
            [FromQuery]string name,
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
            result.Items = new List<UiUserListItem>();
            foreach(var user in query.ToList())
            {
                var listItem = _mapper.Map<UiUserListItem>(user);
                listItem.Roles.AddRange((await _userManager.GetRolesAsync(user)).Select(x => new UiRoleListItem { Name = x }));
                result.Items.Add(listItem);
            }
            return result;
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get(string id)
        {
            if (!User.IsInRole("Admin") && id != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = new List<IdentityRole>();
            foreach(var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                roles.Add(role);
            }
            var uiUser = _mapper.Map<UiUserDetails>(user);
            uiUser.Roles = _mapper.Map<List<UiRoleListItem>>(roles);
            return Ok(uiUser);
        }

        // POST: api/User
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Administators only")]
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

            await _userManager.AddToRolesAsync(applicationUser, userDetails.Roles.Select(x => x.Name));
            return Ok(new UiResponse(true, "user_created", "User created successfully."));

        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Put(string id, [FromBody] UiUserDetails userDetails)
        {
            if (!User.IsInRole("Admin") && id != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return BadRequest(new UiResponse(false, "invalid_input", "Invalid input."));
            }

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

                if (!updatePasswordResult.Succeeded)
                {
                    errorList.AddRange(IdentityResultToResponseMessages(updatePasswordResult));
                    return BadRequest(new UiResponse(false, errorList));
                }
            }

            if (User.IsInRole("Admin"))
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRolesAsync(user, userDetails.Roles.Select(x => x.Name));
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

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Administators only")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return Ok();
        }
    }
}
