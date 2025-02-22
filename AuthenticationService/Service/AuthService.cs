﻿using AuthenticationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Service
{
    public interface IAuthService
    {
        Task<ApplicationUser> GetUserAsync(string username);
        Task<IdentityResult> UpdateUserAsync(UpdateUserModel model);
        Task<IdentityResult> DeleteUserAsync(string username);
        Task<AuthResult> LoginAsync(LoginModel model);
        Task<IdentityResult> RegisterAsync(RegisterModel model);
    }
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResult> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new AuthResult
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                };
            }
            return null;
        }


        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            var user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<IdentityResult> DeleteUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User {Username} not found.", username);
                return IdentityResult.Failed(new IdentityError { Description = "User not found!" });
            }

            _logger.LogInformation("Attempting to delete user {Username}.", username);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Error deleting user {Username}: {Errors}", username, result.Errors);
            }
            else
            {
                _logger.LogInformation("User {Username} deleted successfully.", username);
            }

            return result;
        }

        async Task<ApplicationUser> IAuthService.GetUserAsync(string username )
        {
            return await _userManager.FindByNameAsync(username);
        }

        Task<IdentityResult> IAuthService.UpdateUserAsync(UpdateUserModel model)
        {
            throw new NotImplementedException();
        }
    }
}
