namespace DigitalCinema.Models
{
    public class Show
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public decimal TicketPrice { get; set; }
        public int ShowMovieHallId { get; set; } // الربط بالوسيط
        public ShowMovieHall ShowMovieHall { get; set; } = null!;
        public ICollection<ShowSeat> ShowSeats { get; set; } = new List<ShowSeat>();

    }
}
