namespace DigitalCinema.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public string SeatNumber { get; set; } = string.Empty;  // A1, A2

        public int HallId { get; set; }
        public Hall Hall { get; set; } = null!;
    }
}
