using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.ViewModel
{
    public class CreaetPostVM
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? Video { get; set; }
        
    }
}
