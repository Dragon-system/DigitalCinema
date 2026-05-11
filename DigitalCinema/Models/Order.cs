using Stripe;
using DigitalCinema.Models;

namespace DigitalCinema.Models
{

    public enum PaymentMthod
    {
        Visa,
        Cash
    }

    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Failed,
        Refunded,
        COD,
    }

    public enum OrderStatus
    {
        Pending,
        Paid,
        Cancelled,
        Expired
    }

    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public double TotalPrice { get; set; }

        // Stripe
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        // Status
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public PaymentMthod PaymentMthod { get; set; } = PaymentMthod.Visa;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // User
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;

    }
}