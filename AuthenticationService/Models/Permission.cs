using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RolePermission
    {
        public int RoleId { get; set; }
        public IdentityRole Role { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }

}
