namespace DigitalCinema.Models
{
    public class Hall
    {
        public int Id { get; set; }

        public string Name { get; set; }=string.Empty;

        public ICollection<Seat> Row { get; set; } = new List<Seat>();

        public ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
