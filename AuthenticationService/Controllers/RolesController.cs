using AuthenticationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest(new { Status = "Error", Message = "Role name cannot be empty!" });

            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest(new { Status = "Error", Message = "Role already exists!" });

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                _logger.LogError("Role creation failed: {Errors}", result.Errors);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Role creation failed!" });
            }

            _logger.LogInformation("Role {RoleName} created successfully", roleName);
            return Ok(new { Status = "Success", Message = "Role created successfully!" });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleAssignment model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.RoleName))
                return BadRequest(new { Status = "Error", Message = "Invalid user role assignment data!" });

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return NotFound(new { Status = "Error", Message = "User not found!" });

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (!result.Succeeded)
            {
                _logger.LogError("Role assignment failed: {Errors}", result.Errors);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Role assignment failed!" });
            }

            _logger.LogInformation("Role {RoleName} assigned to user {Username} successfully", model.RoleName, model.Username);
            return Ok(new { Status = "Success", Message = "Role assigned successfully!" });
        }
    }
}
