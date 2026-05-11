using DigitalCinema.Models;
using DigitalCinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;// عشان الـ Area Name

namespace DigitalCinema.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IRepository<Show> _showRepository;
        private readonly IRepository<ShowSeat> _showSeatRepository;
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(
            IRepository<Show> showRepository,
            IRepository<ShowSeat> showSeatRepository,
            IRepository<Booking> bookingRepository,
            IRepository<Ticket> ticketRepository,
            IRepository<Order> OrderRepository,
            UserManager<ApplicationUser> userManager)
        {
            _showRepository = showRepository;
            _showSeatRepository = showSeatRepository;
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _orderRepository = OrderRepository;
            _userManager = userManager;
        }

        // 1. عرض خريطة الكراسي لليوزر
        [HttpGet]
        public async Task<IActionResult> SelectSeats(int id)
        {
            // جلب العرض مع الفيلم والقاعة
            var show = await _showRepository.GetOneAsync(s => s.Id == id,
                includes: [s => s.ShowMovieHall.Movie, s => s.ShowMovieHall.Hall]);

            if (show == null) return NotFound();

            // جلب الكراسي مرتبة
            var seats = await _showSeatRepository.GetAsync(ss => ss.ShowId == id,
                includes: [ss => ss.Seat]);

            ViewBag.ShowSeats = seats.OrderBy(s => s.Seat.SeatNumber);
            return View(show);
        }

        // 2. تنفيذ عملية الحجز (بعد ضغط زر احجز الآن)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ConfirmBooking(int showId, string selectedSeatIds)
        {
            if (string.IsNullOrEmpty(selectedSeatIds))
            {
                TempData["Error"] = "من فضلك اختر كرسياً واحداً على الأقل.";
                return RedirectToAction(nameof(SelectSeats), new { id = showId });
            }

            var show = await _showRepository.GetOneAsync(s => s.Id == showId);
            var currentUser = await _userManager.GetUserAsync(User);

            if (show == null || currentUser == null) return NotFound();

            var seatIdList = selectedSeatIds.Split(',').Select(int.Parse).ToList();

            // 1. إنشاء الحجز بحالة Pending (انتظار الدفع)
            var booking = new Booking
            {
                UserId = currentUser.Id,
                ShowId = showId,
                BookingDate = DateTime.Now,
                TotalPrice = seatIdList.Count * show.TicketPrice,
                TicketStatus = TicketStatus.Pending // تغيير الحالة لانتظار الدفع
            };

            await _bookingRepository.CreateAsync(booking);
            await _bookingRepository.CommitAsync();

            // 2. إنشاء التذاكر (لكن حالة الكرسي تفضل Available أو PendingPayment)
            foreach (var seatId in seatIdList)
            {
                var showSeat = await _showSeatRepository.GetOneAsync(ss => ss.Id == seatId);

                if (showSeat != null && showSeat.Status == SeatStatus.Available)
                {
                    // ملاحظة: لو عايز "تحجزهم مؤقتاً" غير الحالة لـ Reserved
                    // لو عايز تسيبهم لحد ما يدفع، سيبها Available
                    // showSeat.Status = SeatStatus.Available; 

                    var ticket = new Ticket
                    {
                        BookingId = booking.Id,
                        ShowSeatId = seatId,
                        TicketCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        IsUsed = false
                    };
                    await _ticketRepository.CreateAsync(ticket);
                }
            }

            await _ticketRepository.CommitAsync();

            // 3. التوجيه لعملية الدفع (Checkout) بدل صفحة النجاح
            return RedirectToAction("Pay", "Checkout", new { area = "Customer" });
        }

        // 3. صفحة نجاح الحجز وعرض التذاكر
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BookingSuccess(int id)
        {
            var booking = await _bookingRepository.GetOneAsync(b => b.Id == id,
                includes: [
                    b => b.Show.ShowMovieHall.Movie,
                    b => b.Show.ShowMovieHall.Hall,
                    b => b.Tickets
                ]);

            if (booking == null) return NotFound();

            // جلب تفاصيل كراسي التذاكر (أرقام الكراسي)
            var tickets = await _ticketRepository.GetAsync(t => t.BookingId == id,
                includes: [t => t.ShowSeat.Seat]);

            ViewBag.Tickets = tickets;
            return View(booking);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyHistory()
        {
            // جلب الـ User الحالي
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            // جلب كل حجوزات اليوزر ده مع تفاصيل العرض والفيلم والقاعة والتذاكر
            var bookings = await _bookingRepository.GetAsync(
                b => b.UserId == currentUser.Id,
                includes: [
                    b => b.Show.ShowMovieHall.Movie,
            b => b.Show.ShowMovieHall.Hall,
            b => b.Tickets
                ]
            );

            // تحويل البيانات لـ ViewModel (عرض من الأحدث للأقدم)
            var history = bookings.OrderByDescending(b => b.BookingDate).Select(b => new UserBookingHistoryVM
            {
                BookingId = b.Id,
                BookingCode = b.BookingCode,
                MovieName = b.Show.ShowMovieHall.Movie.Name,
                HallName = b.Show.ShowMovieHall.Hall.Name,
                ShowTime = b.Show.StartTime,
                TotalPrice = b.TotalPrice,
                TicketCount = b.Tickets.Count,
                Status = b.TicketStatus
                // ملاحظة: لو محتاج أرقام الكراسي هنا، لازم تعمل Include للـ ShowSeat.Seat
            }).ToList();

            return View(history);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            // جلب الحجز مع التذاكر والمقاعد المرتبطة
            var booking = await _bookingRepository.GetOneAsync(b => b.Id == id && b.UserId == user.Id,
                includes: [e => e.Tickets, e => e.Show.ShowSeats]);

            if (booking == null) return NotFound();

            // اختياري: إذا كان الحجز ملغى أو قديم، نعيد فتح المقاعد
            foreach (var ticket in booking.Tickets)
            {
                if (ticket.ShowSeat != null)
                {
                    ticket.ShowSeat.Status = SeatStatus.Available;
                }
            }

            // حذف الحجز (سيقوم بحذف التذاكر تلقائياً إذا كان هناك Cascade Delete)
            _bookingRepository.Delete(booking);
            await _bookingRepository.CommitAsync();

            TempData["Success"] = "تم حذف الحجز بنجاح.";
            return RedirectToAction(nameof(MyHistory));
        }

    }
}
