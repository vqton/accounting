using AuthenticationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public class UserSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserSeeder> _logger;

        public UserSeeder(UserManager<ApplicationUser> userManager, ILogger<UserSeeder> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SeedAdminUserAsync()
        {
            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var userResult = await _userManager.CreateAsync(user, "P@ssw0rd");
                if (userResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation("Admin user created and assigned to Admin role successfully.");
                    }
                    else
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            _logger.LogError("Error assigning Admin role to user {Username}: {Error}", user.UserName, error.Description);
                        }
                        throw new Exception("Failed to assign Admin role to admin user.");
                    }
                }
                else
                {
                    foreach (var error in userResult.Errors)
                    {
                        _logger.LogError("Error creating admin user: {Error}", error.Description);
                    }
                    throw new Exception("Failed to create admin user.");
                }
            }
            else
            {
                _logger.LogInformation("Admin user already exists.");
            }
        }
    }
}