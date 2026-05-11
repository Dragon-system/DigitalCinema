namespace DigitalCinema.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public decimal UnitPrice { get; set; }
        public int Count { get; set; }
        public int ShowId { get; set; }
        public Show Show { get; set; } = null!;
    }
}
