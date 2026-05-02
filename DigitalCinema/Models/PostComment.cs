using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.Models
{
    public class PostComment
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        // علاقة مع البوست
        public int PostId { get; set; }
        public Post Post { get; set; } = null!; 
        // علاقة مع اليوزر (اللي كتب الكومنت)
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
