using DigitalCinema.Models;
using DigitalCinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace DigitalCinema.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IBockingRepository _IbookingRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ShowSeat> _showSeatRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            IRepository<Order> orderRepository,
            IRepository<ShowSeat> showSeatRepository,
            IRepository<OrderItem> orderItemRepository,
            ILogger<CheckoutController> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IRepository<Booking> bookingRepository,
            IBockingRepository IbookingRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _logger = logger;
            _context = context;
            _bookingRepository = bookingRepository;
            _IbookingRepository = IbookingRepository;
            _showSeatRepository = showSeatRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            // 1. جلب الحجوزات الحالية (Pending) فقط لمنع تراكم الأسعار القديمة
            var userBooking = await _bookingRepository.GetAsync(
                e => e.UserId == user.Id && e.TicketStatus == TicketStatus.Pending,
                includes:[e=>e.Show, e=>e.Show.ShowMovieHall.Movie, e=>e.Show.ShowMovieHall.Hall ]);

            if (!userBooking.Any()) return RedirectToAction("Index", "Home");

            // 2. إنشاء الطلب الجديد بالإجمالي الصحيح للحجز الحالي
            Order order = new()
            {
                ApplicationUserId = user.Id,
                TotalPrice = (double)userBooking.Sum(b => b.TotalPrice),
                CreatedAt = DateTime.Now,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            };

            await _orderRepository.CreateAsync(order);
            await _orderRepository.CommitAsync();

            // 3. إعداد بوابة Stripe مع تفاصيل الفيلم والقاعة لتظهر للمستخدم
            var options = new SessionCreateOptions
            {
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel?orderId={order.Id}",
            };

            foreach (var item in userBooking)
            {
                options.LineItems.Add(new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Show?.ShowMovieHall?.Movie?.Name ?? "Movie Ticket",
                            Description = $"Hall: {item.Show?.ShowMovieHall?.Hall?.Name} | Time: {item.Show?.StartTime:g}",
                        },
                        UnitAmount = (long)(item.TotalPrice * 100),
                    },
                    Quantity = 1,
                });
            }

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            order.SessionId = session.Id;
            await _orderRepository.CommitAsync();

            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Success(int orderId)
        {
            if (orderId <= 0) return BadRequest("Invalid Order ID");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _orderRepository.GetOneAsync(e => e.Id == orderId, includes: [e => e.ApplicationUser]);
                if (order is null) return NotFound();

                var service = new SessionService();
                Session session = await service.GetAsync(order.SessionId);

                if (session.PaymentStatus.ToLower() != "paid") return BadRequest("Payment not completed.");

                // تحديث حالة الطلب
                order.PaymentIntentId = session.PaymentIntentId;
                order.PaymentStatus = PaymentStatus.Succeeded;
                order.OrderStatus = OrderStatus.Paid;

                // 4. جلب الحجوزات الـ Pending فقط وتأكيدها وتلوين الكراسي
                var userBookings = await _bookingRepository.GetAsync(
                    e => e.UserId == order.ApplicationUserId && e.TicketStatus == TicketStatus.Pending,
                    includes: [e=>e.Show,e => e.Tickets]);

                if (userBookings != null)
                {
                    foreach (var booking in userBookings)
                    {
                        int ticketsCount = booking.Tickets?.Count ?? 0;

                        await _orderItemRepository.CreateAsync(new OrderItem
                        {
                            OrderId = orderId,
                            ShowId = booking.ShowId,
                            Count = ticketsCount,
                            UnitPrice = ticketsCount > 0 ? (booking.TotalPrice / ticketsCount) : 0
                        });

                        foreach (var ticket in booking.Tickets)
                        {
                            var seat = await _showSeatRepository.GetOneAsync(s => s.Id == ticket.ShowSeatId);
                            if (seat != null)
                            {
                                seat.Status = SeatStatus.Booked; // تحويل الكرسي لأحمر (محجوز)
                                _context.Entry(seat).State = EntityState.Modified;
                            }
                        }
                        booking.TicketStatus = TicketStatus.Confirmed; // تأكيد الحجز
                        _context.Entry(booking).State = EntityState.Modified;
                    }
                }

                // حفظ كل التغييرات دفعة واحدة لضمان تلوين الكراسي وتأكيد الحجز
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return View(new PaymentSuccessViewModel
                {
                    OrderId = order.Id,
                    TotalPrice = order.TotalPrice,
                    Email = order.ApplicationUser?.Email ?? ""
                });
            }
            catch (Exception ex)
            {
                if (_context.Database.CurrentTransaction != null) await transaction.RollbackAsync();
                _logger.LogError(ex, "Error in Success Action");
                return BadRequest("Internal Error: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            // تنظيف الحجوزات المعلقة عند الإلغاء لمنع تراكم الأسعار في المرة القادمة
            var pendingBookings = await _bookingRepository.GetAsync(
                b => b.UserId == user.Id && b.TicketStatus == TicketStatus.Pending);

            if (pendingBookings.Any())
            {
                _IbookingRepository.DeleteRange(pendingBookings);
                await _bookingRepository.CommitAsync();
            }

            return View();
        }
    }
}
