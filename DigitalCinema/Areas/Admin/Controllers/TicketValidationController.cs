using Microsoft.AspNetCore.Mvc;
using DigitalCinema.Models;
using DigitalCinema.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class TicketValidationController : Controller
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<Show> _showRepository;

        public TicketValidationController(IRepository<Ticket> ticketRepository, IRepository<Show> showRepository)
        {
            _ticketRepository = ticketRepository;
            _showRepository = showRepository;
        }

        // الصفحة الرئيسية لعملية المسح (Scanner Interface)
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public IActionResult Index()
        {
            return View();
        }

        // الأكشن المسئول عن التحقق من الكود (يُستدعى عبر AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> ValidateTicket(string ticketCode)
        {
            var result = new TicketCheckResultVM { TicketCode = ticketCode?.ToUpper() };

            if (string.IsNullOrEmpty(ticketCode))
            {
                result.Message = "من فضلك أدخل كود التذكرة أولاً.";
                return Json(result);
            }

            // البحث عن التذكرة مع جلب كافة البيانات المرتبطة
            var ticket = await _ticketRepository.GetOneAsync(
                t => t.TicketCode == ticketCode.ToUpper(),
                includes: [
                    t => t.ShowSeat.Seat,
                    t => t.Booking.Show.ShowMovieHall.Movie,
                    t => t.Booking.Show.ShowMovieHall.Movie,
                    t => t.Booking.User
                ]
            );

            if (ticket == null)
            {
                result.Message = "عفواً، هذا الكود غير صحيح أو لم يتم إصداره.";
                return Json(result);
            }

            var showTime = ticket.Booking.Show.StartTime;
            var expiryTime = showTime.AddDays(3);

            if (DateTime.Now > expiryTime)
            {
                result.Message = $"انتهت صلاحية هذه التذكرة. كان العرض في {showTime:dd/MM/yyyy} وانتهت الصلاحية في {expiryTime:dd/MM/yyyy}.";
                return Json(result);
            }

            // تعبئة بيانات التذكرة في الـ ViewModel
            result.MovieName = ticket.Booking.Show.ShowMovieHall.Movie.Name;
            result.SeatNumber = ticket.ShowSeat.Seat.SeatNumber;
            result.CustomerName = ticket.Booking.User.UserName;

            // التحقق إذا كانت التذكرة استُخدمت من قبل
            if (ticket.IsUsed)
            {
                result.AlreadyUsed = true;
                result.UsedAt = ticket.UsedAt;
                result.Message = "تنبيه: هذه التذكرة استُخدمت بالفعل للدخول!";
                return Json(result);
            }

            // إذا كانت سليمة، يتم تفعيلها الآن
            ticket.IsUsed = true;
            ticket.UsedAt = DateTime.Now;

            await _ticketRepository.CommitAsync();

            result.IsValid = true;
            result.Message = "تم تأكيد الدخول بنجاح. استمتع بالعرض!";
            return Json(result);
        }

        // تقرير الحضور التفصيلي لعرض معين (Attendance Report)
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> AttendanceReport(int showId)
        {
            // جلب العرض الأساسي
            var show = await _showRepository.GetOneAsync(s => s.Id == showId,
                includes: [s => s.ShowMovieHall.Movie]);

            if (show == null) return NotFound();

            // جلب كل التذاكر المحجوزة لهذا العرض
            var tickets = await _ticketRepository.GetAsync(
                t => t.Booking.ShowId == showId,
                includes: [t => t.ShowSeat.Seat]
            );

            // بناء الـ ViewModel للتقرير
            var reportVM = new AttendanceReportVM
            {
                ShowId = showId,
                MovieName = show.ShowMovieHall.Movie.Name,
                ShowTime = show.StartTime,
                TotalBookedTickets = tickets.Count(),
                AttendedCount = tickets.Count(t => t.IsUsed),
                Tickets = tickets.Select(t => new TicketDetailsVM
                {
                    TicketCode = t.TicketCode,
                    SeatNumber = t.ShowSeat.Seat.SeatNumber,
                    IsUsed = t.IsUsed,
                    UsedAt = t.UsedAt
                }).ToList()
            };

            return View(reportVM);
        }
    }
}
