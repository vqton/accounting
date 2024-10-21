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



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.LoginAsync(model);
            if (result == null)
                return Unauthorized();

            return Ok(result);
        }

        // Create user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }

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
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User deletion failed! Please try again." });
            }

            return Ok(new { Status = "Success", Message = "User deleted successfully!" });
        }


    }
}