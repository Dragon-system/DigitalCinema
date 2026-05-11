using DigitalCinema.Models;
using DigitalCinema.Utility;
using DigitalCinema.Utility.DbInitializers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace DigitalCinema.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        // 🔥 التعديل الأهم: الربط بالكرسي الفعلي اللي في العرض
        public int ShowSeatId { get; set; }
        public ShowSeat ShowSeat { get; set; } = null!;

        public string TicketCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
    }
}
