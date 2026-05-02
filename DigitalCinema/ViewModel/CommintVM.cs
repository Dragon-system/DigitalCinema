namespace DigitalCinema.ViewModel
{
    public class CommintVM
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorImage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
