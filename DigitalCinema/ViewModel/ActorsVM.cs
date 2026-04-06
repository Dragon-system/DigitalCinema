namespace  DigitalCinema.ViewModels
{
    public class ActorsVM
    {
        public IEnumerable<Actor> Actors { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        //public string Query { get; set; } 
    }
}
