namespace DigitalCinema.ViewModels
{
    public class MoviesVM
    {
        public IEnumerable<Movie> Movies { get; set; } =null!;
        public IEnumerable<Category> Categories { get; set; } = null!;
        public IEnumerable<Cinema> Cinemas { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        //public string Query { get; set; } 
        public string? FilterName { get; set; }
        public decimal? FilterMinPrice { get; set; }
        public decimal? FilterMaxPrice { get; set; }
        public int? FilterCategoryId { get; set; }
        public int? FilterCinemaId { get; set; }
        public MoviesVM()
        {

        }
    }
}
