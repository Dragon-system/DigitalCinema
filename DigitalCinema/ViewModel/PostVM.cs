namespace DigitalCinema.ViewModel
{
    public class PostVM
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string? VideoPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int LikesCount { get; set; }
        public string AuthorImage { get; set; } = string.Empty;
        public bool IsLikedByCurrentUser { get; set; }
        public List<CommintVM> Comments { get; set; } = new();
    }
}
