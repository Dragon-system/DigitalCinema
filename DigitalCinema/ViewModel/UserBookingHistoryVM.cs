namespace DigitalCinema.ViewModel
{
    public class UserBookingHistoryVM
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string MovieName { get; set; } = string.Empty;
        public string HallName { get; set; } = string.Empty;
        public DateTime ShowTime { get; set; }
        public decimal TotalPrice { get; set; }
        public int TicketCount { get; set; }
        public TicketStatus Status { get; set; }
        public List<string> SeatNumbers { get; set; } = new();
    }
}
