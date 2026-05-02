namespace DigitalCinema.Models
{
    public class PostLike
    {
        public int Id { get; set; }

        // مين اللي عمل اللايك؟
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        // اللايك ده على أنهي بوست؟
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
