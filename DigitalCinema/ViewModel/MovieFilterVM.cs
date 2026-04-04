namespace Ecommerce.ViewModels
{
    namespace Ecommerce.ViewModels
    {
        public class MovieFilterVM//(string Name, double? MainPrice, double? MaxPrice, string CategoryName, string BrandName)
        {
            public string? Name { get; set; }
            public decimal? MainPrice { get; set; }
            public decimal? MaxPrice { get; set; }
            public int? CategoryId { get; set; }
            public int? CinemaId { get; set; }

            // Constructor فاضي (مهم)
            public MovieFilterVM()
            {
            }
        }
    }

}
