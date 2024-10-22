using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthenticationService
{


    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleSeeder> _logger;

        public RoleSeeder(RoleManager<IdentityRole> roleManager, ILogger<RoleSeeder> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedRolesAsync()
        {
            string[] roleNames = { "Admin", "SeniorAccountant", "JuniorAccountant", "Auditor", "SalesManager" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            _logger.LogError("Error creating role {RoleName}: {Error}", roleName, error.Description);
                        }
                        throw new Exception($"Failed to create role {roleName}");
                    }
                    else
                    {
                        _logger.LogInformation("Role {RoleName} created successfully.", roleName);
                    }
                }
                else
                {
                    _logger.LogInformation("Role {RoleName} already exists.", roleName);
                }
            }
        }
    }
}