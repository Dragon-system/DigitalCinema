namespace DigitalCinema.ViewModel
{
    public class TicketCheckResultVM
    {
        public string? TicketCode { get; set; }
        public bool IsValid { get; set; }
        public string? Message { get; set; }

        // بيانات للعرض لو التذكرة سليمة
        public string? MovieName { get; set; }
        public string? HallName { get; set; }
        public string? SeatNumber { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? UsedAt { get; set; }
        public bool AlreadyUsed { get; set; }
    }
}
