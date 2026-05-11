namespace DigitalCinema.ViewModel
{
    public class BookingReceiptVM
    {
        public string BookingCode { get; set; }
        public string UserName { get; set; }
        public string MovieTitle { get; set; }
        public string HallName { get; set; }
        public DateTime ShowTime { get; set; }
        public List<string> Seats { get; set; }
        public decimal TotalPrice { get; set; }
        public TicketStatus Status { get; set; }
    }
}

