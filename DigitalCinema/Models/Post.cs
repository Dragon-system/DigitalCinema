using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImagePath { get; set; }
        public string? VideoPath { get; set; }

        // علاقة مع اليوزر (صاحب البوست)
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        // البوست له كومنتات كتير
        public ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
        public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    }
}
