using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [CustomLength(3, 60)]
        public string Name { get; set; } = string.Empty;
        
        public string Img { get; set; } = string.Empty;
        [Required]
        [CustomLength(3, 100)]
        public string Role { get; set; } = string.Empty;
    }
}
