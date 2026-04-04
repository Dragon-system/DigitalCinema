namespace Ecommerce.ViewModels
{
    public class MoviesVM
    {
        public IEnumerable<Movie> Movies { get; set; } = null!;
        public IEnumerable<Category> Categories { get; set; } = null!;
        public IEnumerable<Cinema> Cinemas { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        //public string Query { get; set; } 
    }
}
