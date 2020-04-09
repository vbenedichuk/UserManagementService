using UserManagementService.Models.UI;
using UserManagementService.Models.UI.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Administators only")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(
            [NotNull] RoleManager<IdentityRole> roleManager,
            [NotNull] IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Return list of roles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public UiList<UiRoleListItem> Get()
        {
            var result = new UiList<UiRoleListItem>();
            result.TotalCount = _roleManager.Roles.Count();
            result.Items = _mapper.Map<List<UiRoleListItem>>(_roleManager.Roles.ToList());
            return result;
        }

        /// <summary>
        /// Role by Id.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var result = _roleManager.Roles.FirstOrDefault(x => x.Id == id);
            if(result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, UiRoleListItem roleDetails)
        {
            if (roleDetails == null || !ModelState.IsValid || string.IsNullOrWhiteSpace(roleDetails.Id) || id != roleDetails.Id)
            {
                return BadRequest(new UiResponse(false, "invalid_input", "Invalid input."));
            }
            var errorList = new List<UiResponseMessage>();
            var roleWithTheSameName = await _roleManager.FindByNameAsync(roleDetails.Name);
            if(roleWithTheSameName != null && roleDetails.Id != roleWithTheSameName.Id)
            {
                errorList.Add(new UiResponseMessage { Code = "duplicate_name", Message = "Role with the same name is already exists." });
            }

            if (errorList.Count > 0)
            {
                return BadRequest(new UiResponse(false, errorList));
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                errorList.Add(new UiResponseMessage
                {
                    Code = "internal_error",
                    Message = "Role is not available for editing."
                });
            }

            if (errorList.Count > 0)
            {
                return BadRequest(new UiResponse(false, errorList));
            }

            role.Name = roleDetails.Name;
            var updateResults = await _roleManager.UpdateAsync(role);
            errorList.AddRange(IdentityResultToResponseMessages(updateResults));
            if (errorList.Count > 0)
            {
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

        [HttpPost]
        public async Task<IActionResult> Post(UiRoleListItem roleDetails)
        {
            if (roleDetails == null || !ModelState.IsValid || roleDetails.Id != "new")
            {
                return BadRequest(new UiResponse(false, "invalid_input", "Invalid input."));
            }

            var errorList = new List<UiResponseMessage>();
            var roleWithTheSameName = await _roleManager.FindByNameAsync(roleDetails.Name);
            if(roleWithTheSameName != null)
            {
                errorList.Add(new UiResponseMessage { Code = "duplicate_name", Message = "Role with the same name already exists." });
            }

            if(errorList.Count > 0)
            {
                return BadRequest(new UiResponse(false, errorList));
            }

            if (roleDetails.Id == "new")
            {
                roleDetails.Id = Guid.NewGuid().ToString();
            }
            var role = _mapper.Map<IdentityRole>(roleDetails);
            var creationResults = await _roleManager.CreateAsync(role);

            if (!creationResults.Succeeded)
            {
                foreach (var identityError in creationResults.Errors)
                {
                    errorList.Add(new UiResponseMessage { Code = "identity_error", Message = identityError.Description });
                }
                return BadRequest(new UiResponse(false, errorList));
            }

            return Ok(new UiResponse(true, "role_created", "Role created successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null) {
                await _roleManager.DeleteAsync(role);
            }
            return Ok();
        }


    }
}
