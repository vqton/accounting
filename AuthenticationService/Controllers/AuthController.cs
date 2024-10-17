using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Models;
using AuthenticationService.Service;


namespace AuthenticationService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IConfiguration _configuration;

        //public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        //{
        //    _userManager = userManager;
        //    _configuration = configuration;
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.LoginAsync(model);
            if (result == null)
                return Unauthorized();

            return Ok(result);
        }
        //public async Task<IActionResult> Login([FromBody] LoginModel model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.Username);
        //    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        var authClaims = new[]
        //        {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };

        //        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        //        var token = new JwtSecurityToken(
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            expires: DateTime.Now.AddHours(3),
        //            claims: authClaims,
        //            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //        );

        //        return Ok(new
        //        {
        //            token = new JwtSecurityTokenHandler().WriteToken(token),
        //            expiration = token.ValidTo
        //        });
        //    }
        //    return Unauthorized();
        //}

        // Create user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }
        //public async Task<IActionResult> Register([FromBody] RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

        //    ApplicationUser user = new ApplicationUser()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description).ToList();
        //        foreach (var error in errors)
        //        {
        //            switch (error)
        //            {
        //                case "Username already taken":
        //                    return BadRequest(new { Status = "Error", Message = "Username already taken. Please choose a different username." });
        //                case "Email already taken":
        //                    return BadRequest(new { Status = "Error", Message = "Email already taken. Please use a different email address." });
        //                case "Invalid email format":
        //                    return BadRequest(new { Status = "Error", Message = "Invalid email format. Please enter a valid email address." });
        //                default:
        //                    return BadRequest(new { Status = "Error", Message = "User creation failed!", Errors = errors });
        //            }
        //        }
        //    }

        //    return Ok(new { Status = "Success", Message = "User created successfully!" });
        //}

        // Read user
        [HttpGet("getuser/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var result = await _authService.GetUserAsync(username);
            if (result == null)
                return NotFound(new { Status = "Error", Message = "User not found!" });

            return Ok(result);
        }

        // Update user
        [HttpPut("updateuser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            var result = await _authService.UpdateUserAsync(model);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User update failed! Please check user details and try again." });

            return Ok(new { Status = "Success", Message = "User updated successfully!" });
        }

        // Delete user
        [HttpDelete("deleteuser/{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var result = await _authService.DeleteUserAsync(username);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User deletion failed! Please try again." });

            return Ok(new { Status = "Success", Message = "User deleted successfully!" });
        }


    }
}