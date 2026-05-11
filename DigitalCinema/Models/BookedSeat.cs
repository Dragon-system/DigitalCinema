using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalCinema.Models
{
    public class BookingSeat
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public int? SeatId { get; set; }
        public Seat? Seat { get; set; }
    }
}

