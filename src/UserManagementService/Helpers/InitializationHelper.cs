using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagementService.Abstractions;
using UserManagementService.Data;
using UserManagementService.Models.Database;
using UserManagementService.Models.Initialization;

namespace UserManagementService.Helpers
{
    /// <summary>
    /// Initialize database if necessary.
    /// </summary>
    class InitializationHelper : IInitializationHelper
    {
        ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        ILogger<IInitializationHelper> _logger;

        public InitializationHelper(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IInitializationHelper> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public void Initialize()
        {
            if(_context.Users.Any() || _context.Roles.Any())
            {
                _logger.LogInformation("DB is already initialized.");
                throw new ApplicationException("Initialization failed.");
            }
            _logger.LogInformation("Initialization started.");

            var fileString = File.ReadAllText("Initialize.json");
            var data = JsonConvert.DeserializeObject<InitializationFile>(fileString);
            foreach(var role in data.Roles)
            {
                _roleManager.CreateAsync(new IdentityRole
                {
                    Name = role,
                    NormalizedName = role.ToLower()
                }).Wait();
            }

            foreach(var userRecord in data.Users)
            {
                var user = new ApplicationUser
                {
                    Email = userRecord.Email,
                    UserName = userRecord.UserName
                };

                var result = _userManager.CreateAsync(user).Result;
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        _logger.LogError("{0} {1}", error.Code, error.Description);
                    }
                    throw new ApplicationException("Initialization failed.");
                }
                foreach(var userRole in userRecord.Roles)
                {
                    var addRoleResult = _userManager.AddToRoleAsync(user, userRole).Result;
                    if (!addRoleResult.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError("{0} {1}", error.Code, error.Description);
                        }
                        throw new ApplicationException("Initialization failed.");
                    }
                }
                foreach(var key in userRecord.Claims.Keys)
                {
                    var addClaimResult = _userManager.AddClaimAsync(user, new Claim(key, userRecord.Claims[key])).Result;
                    if (!addClaimResult.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError("{0} {1}", error.Code, error.Description);
                        }
                        throw new ApplicationException("Initialization failed.");
                    }
                }
            }
            _logger.LogInformation("Database Initialized.");
        }
    }
}
