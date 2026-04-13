using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required]
        [CustomLength(3, 60)]
        public string Name { get; set; } = string.Empty;
        
        public string Img { get; set; } = string.Empty;
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
