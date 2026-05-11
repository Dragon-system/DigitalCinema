using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalCinema.Models
{
    public enum TicketStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Used
    }
    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int ShowId { get; set; }
        public Show Show { get; set; } = null!;

        public DateTime BookingDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }

        // خليه يتولد تلقائياً زي التذكرة لسهولة التتبع
        public string BookingCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();

        public TicketStatus TicketStatus { get; set; }

        // الحجز الواحد فيه كذا تذكرة (تذكرة لكل كرسي)
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }

}
