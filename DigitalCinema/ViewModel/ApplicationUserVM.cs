using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.ViewModels
{
    public class ApplicationUserVM
    {
        
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ImageProfile { get; set; }

    }
}
