using DigitalCinema.Models;

namespace DigitalCinema.ViewModel
{
    public class MovieCreateModelVM
    {
        public IEnumerable<Category> Categories { get; set; } = null!;
        public IEnumerable<Cinema> Cinemas { get; set; } = null!;
        public Movie Movies { get; set; } = null!;
        public IEnumerable<Actor> Actors{ get; set; } = null!;
        public IEnumerable<ActorMovie> ActorMovies{ get; set; } = null!;
        public IEnumerable<SupImg> SupImgs { get; set; } = null!;
        public int? ActorId { get; set; }
        public int? MovieId { get; set; }
    }
}
