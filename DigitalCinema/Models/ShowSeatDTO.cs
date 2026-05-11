namespace DigitalCinema.Models
{
    public class ShowSeatDTO
    {
        public int ShowSeatId { get; set; }  // ShowSeat.Id
            public int? SeatId { get; set; }  // Seat.Id
            public string? SeatNumber { get; set; }  // "A1", "B3"
            public SeatStatus Status { get; set; }  // Available / Reserved / Booked
    }
}
