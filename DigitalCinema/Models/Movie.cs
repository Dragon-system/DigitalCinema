namespace DigitalCinema.Models
{
    public class Movie
    {
       public int Id { get; set; }
       public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MainImg { get; set; } = string.Empty;
      
        public decimal Price { get; set; }
        public DateTime ShowTime { get; set; } = DateTime.Now;
        
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;

        public List<ActorMovie> ActorMovies { get; set; } = new();
    }
}
