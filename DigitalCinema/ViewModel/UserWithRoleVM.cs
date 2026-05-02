using Microsoft.AspNetCore.Identity;

namespace DigitalCinema.ViewModels
{
    public class UserWithRoleVM
    {
        public string Id { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string RoleName { get; set; } = string.Empty;

        public IEnumerable<IdentityRole> identityRoles { get; set; } = [];


    }
}
