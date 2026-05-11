namespace DigitalCinema.ViewModel
{
    public class AttendanceReportVM
    {
        public int ShowId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public DateTime ShowTime { get; set; }
        public int TotalBookedTickets { get; set; }
        public int AttendedCount { get; set; }
        public int RemainingCount => TotalBookedTickets - AttendedCount;

        public List<TicketDetailsVM> Tickets { get; set; } = new();
    }
}
