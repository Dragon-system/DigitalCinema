using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.Models
{
    public class Movie
    {
       public int Id { get; set; }
        [Required]
       [CustomLength(3,60)]
       public string Name { get; set; } = string.Empty;
        [CustomLength(3, 255)]
        public string Description { get; set; } = string.Empty;
        
        public string MainImg { get; set; } = string.Empty;
        [Required]
        [Range(1, 100000)]
        public decimal Price { get; set; }
       
        public DateTime ShowTime { get; set; } = DateTime.Now;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public Category Category { get; set; } = null!;
        
        [Required]
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;

        public List<ActorMovie> ActorMovies { get; set; } = new();
    }
}
