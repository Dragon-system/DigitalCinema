using Microsoft.AspNetCore.Identity;

namespace DigitalCinema.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        public string ImageProfile { get; set; } = string.Empty;
        public DateOnly DOB { get; set; } = new DateOnly();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
    }
}
