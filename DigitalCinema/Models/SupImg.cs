namespace DigitalCinema.Models
{
    public class SupImg
    {
        public int id { get; set; }
        public string SubImg { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public Movie movie { get; set; } = null!;

    }
}
