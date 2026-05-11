namespace DigitalCinema.Models
{
    public enum SeatStatus
    {
        Available,
        Reserved,
        Booked,
        Pending
    }
    public class ShowSeat
    {
        public int Id { get; set; }

        public int ShowId { get; set; }
        public Show Show { get; set; } =null!;

        public int SeatId { get; set; }
        public Seat Seat { get; set; } = null!;

        public SeatStatus Status { get; set; }
    }
}
