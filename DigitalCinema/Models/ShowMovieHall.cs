namespace DigitalCinema.Models
{
    public class ShowMovieHall
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public int HallId { get; set; }
        public Hall Hall { get; set; } = null!;

        public ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
