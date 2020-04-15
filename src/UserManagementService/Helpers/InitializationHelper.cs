using MemorizeThat.EmailManagement.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagementService.Abstractions;
using UserManagementService.Data;
using UserManagementService.Models.Configuration;
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
        IEmailSender _emailSender;
        EmailConfiguration _emailConfiguration;
        AppConfiguration _appConfiguration;

        public InitializationHelper(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IInitializationHelper> logger,
            IEmailSender emailSender,
            IOptions<EmailConfiguration> emailConfiguration,
            IOptions<AppConfiguration> appConfiguration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
            _emailConfiguration = emailConfiguration.Value;
            _appConfiguration = appConfiguration.Value;
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
                var passwordResetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                _emailSender.SendEmailAsync(
                    userRecord.Email,
                    _emailConfiguration.SystemFrom,
                    _emailConfiguration.SystemFromName, 
                    "Reset your password", 
                    string.Format("Please reset your password: {0}ResetPassword?passwordResetToken={1}", _appConfiguration.ApplicationDomain, passwordResetToken));
            }
            _logger.LogInformation("Database Initialized.");
        }
    }
}
