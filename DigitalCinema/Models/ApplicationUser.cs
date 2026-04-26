using Microsoft.AspNetCore.Identity;

namespace DigitalCinema.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        public string ImageProfile { get; set; } = string.Empty;
        public DateOnly DOB { get; set; } = new DateOnly();

    }
}
