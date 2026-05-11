namespace DigitalCinema.ViewModel
{
    public class MovieWithRelatedVM
    {
        public Movie Movie { get; set; } = null!;
        public List<Movie> SamCategory { get; set; } = new();
        public List<Movie> SamName { get; set; } = new();
        public List<SupImg> SupImgs { get; set; } = new();
        public List<ActorMovie> ActorMovies { get; set; } = new();
        public List<Show> Shows { get; set; } = new(); // ✅ أضيف دي
    }
}
