namespace DigitalCinema.ViewModel
{
    public class CreateShowVM
    {
        // البيانات اللي هنستقبلها من الفورم
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public decimal TicketPrice { get; set; }

        // القوائم اللي هنملا بيها الـ Dropdowns
        public IEnumerable<Movie>? Movies { get; set; }
        public IEnumerable<Hall>? Halls { get; set; }
    }
}
