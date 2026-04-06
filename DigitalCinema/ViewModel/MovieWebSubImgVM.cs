namespace DigitalCinema.ViewModel
{
    public class MovieWebSubImgVM
    {
       
            public Movie Movie  { get; set; } = null!;

            public IEnumerable<SupImg> SubImgs { get; set; } = [];
            public IEnumerable<Category> Categories { get; set; } = [];
            public IEnumerable<Cinema> Cinemas { get; set; } = [];
            public IEnumerable<ActorMovie> ActorMovie { get; set; } = [];
            public IEnumerable<Actor> AllActors { get; set; } = [];
    
    }
}
