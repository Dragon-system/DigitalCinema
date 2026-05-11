namespace DigitalCinema.ViewModel
{
    public class TicketDetailsVM
    {
        public string TicketCode { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}
