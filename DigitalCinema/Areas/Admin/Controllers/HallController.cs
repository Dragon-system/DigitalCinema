using DigitalCinema.Models;
using DigitalCinema.Utility; // تأكد من مكان كلاس SD
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class HallController : Controller
    {
        private readonly IRepository<Show> _showRepository;
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Hall> _hallRepository;
        private readonly IRepository<ShowSeat> _showSeatRepository;
        private readonly IRepository<ShowMovieHall> _showMovieHallRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HallController(
            IRepository<Show> showrepository,
            IRepository<Seat> seatRepository,
            IRepository<ShowSeat> showSeatRepository,
            IRepository<Hall> hallRepository,
            IRepository<Movie> movieRepository,
            IRepository<ShowMovieHall> showMovieHallRepository,
            UserManager<ApplicationUser> userManager)
        {
            _showRepository = showrepository;
            _hallRepository = hallRepository;
            _showMovieHallRepository = showMovieHallRepository;
            _userManager = userManager;
            _movieRepository = movieRepository;
            _seatRepository = seatRepository;
            _showSeatRepository = showSeatRepository;
        }

        // عرض قائمة القاعات

        public async Task<IActionResult> Index()
        {
            var vm = new FultterShow_HallVM
            {
                // جلب القاعات مع الكراسي
                Halls = await _hallRepository.GetAsync(includes: [h => h.Row]),

                // جلب جداول الربط (التي تحتوي على العروض والأفلام والقاعات)
                // تأكد من أسماء الـ Navigation Properties في موديل ShowMovieHall
                ShowMovieHalls = await _showMovieHallRepository.GetAsync(
              includes: [smh => smh.Movie, smh => smh.Hall, smh => smh.Shows])
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public IActionResult CreateHall()
        {
            return View();
        }

        // 1. إنشاء القاعة وتوليد كراسيها تلقائياً
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> CreateHall(string hallName, int rows, int cols)
        {
            if (string.IsNullOrEmpty(hallName) || rows <= 0 || cols <= 0)
            {
                return BadRequest("بيانات القاعة غير صحيحة");
            }

            var hall = new Hall { Name = hallName };

            // توليد الكراسي (A1, A2, B1...)
            for (int i = 1; i <= rows; i++)
            {
                char rowLetter = (char)('A' + i - 1);
                for (int j = 1; j <= cols; j++)
                {
                    hall.Row.Add(new Seat
                    {
                        SeatNumber = $"{rowLetter}{j}"
                    });
                }
            }

            await _hallRepository.CreateAsync(hall);
            await _hallRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CreateShow()
        {
            var vm = new CreateShowVM
            {
                Movies = await _movieRepository.GetAsync(),
                Halls = await _hallRepository.GetAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> CreateShow(CreateShowVM vm)
        {
            if (vm.MovieId > 0 && vm.HallId > 0)
            {
                // 1. إنشاء سجل الربط (الوسيط) بين الفيلم والقاعة
                var movieHall = new ShowMovieHall
                {
                    MovieId = vm.MovieId,
                    HallId = vm.HallId
                };
                await _showMovieHallRepository.CreateAsync(movieHall);
                await _showMovieHallRepository.CommitAsync();

                // 2. إنشاء العرض الفعلي (الميعاد والسعر) وربطه بسجل الوسيط
                var show = new Show
                {
                    StartTime = vm.StartTime,
                    TicketPrice = vm.TicketPrice,
                    ShowMovieHallId = movieHall.Id // الربط الإجباري اللي في المايجريشن
                };
                await _showRepository.CreateAsync(show);
                await _showRepository.CommitAsync();

                // 3. توليد كراسي العرض (ShowSeats) وربطها بـ "العرض" (Show)
                var seats = await _seatRepository.GetAsync(s => s.HallId == vm.HallId);
                foreach (var seat in seats)
                {
                    await _showSeatRepository.CreateAsync(new ShowSeat
                    {
                        ShowId = show.Id, // الربط الصحيح بجدول المواعيد
                        SeatId = seat.Id,
                        Status = SeatStatus.Available
                    });
                }
                await _showSeatRepository.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            // في حالة وجود خطأ، نعيد ملء القوائم ونرجع للـ View
            vm.Movies = await _movieRepository.GetAsync();
            vm.Halls = await _hallRepository.GetAsync();
            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> ShowDetails(int id)
        {
            // جلب العرض مع الفيلم والقاعة وكراسي العرض (ShowSeats)
            var show = await _showRepository.GetOneAsync(
                s => s.Id == id,
                includes: [
                    s => s.ShowMovieHall.Movie,
            s => s.ShowMovieHall.Hall,
            s => s.ShowSeats]
            );

            if (show == null) return NotFound();

            // جلب بيانات الكراسي بالتفصيل (عشان نعرف رقم الكرسي)
            // ملاحظة: لو الـ Repository مش بيعمل Include داخلي، ممكن نحتاج نجيب الـ ShowSeats منفصلة
            var showSeats = await _showSeatRepository.GetAsync(
                ss => ss.ShowId == id,
                includes: [ss => ss.Seat]
            );

            ViewBag.ShowSeats = showSeats;
            return View(show);
        }

        // حذف قاعة (سيقوم بحذف الكراسي تلقائياً بسبب Cascade Delete في المايجريشن)
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> DeleteHall(int id)
        {
            // 1. هات القاعة مع العروض المرتبطة بيها
            var hall = await _hallRepository.GetOneAsync(h => h.Id == id, includes: [h => h.Row]);
            if (hall == null) return NotFound();

            // 2. ابحث عن أي سجلات ربط (ShowMovieHall) مرتبطة بالقاعة دي وامسحها
            var relatedLinks = await _showMovieHallRepository.GetAsync(smh => smh.HallId == id);
            foreach (var link in relatedLinks)
            {
                _showMovieHallRepository.Delete(link);
            }
            await _showMovieHallRepository.CommitAsync();

            // 3. دلوقتي تقدر تمسح القاعة بأمان
            _hallRepository.Delete(hall);
            await _hallRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
        public async Task<IActionResult> DeleteShow(int id)
        {
            // 1. البحث عن العرض
            var show = await _showRepository.GetOneAsync(s => s.Id == id);

            if (show == null)
            {
                return NotFound();
            }

            // 2. تنفيذ الحذف
            // (بما أن ShowSeats مربوط بـ Show، سيتم حذف الكراسي المرتبطة بهذا الموعد تلقائياً)
            _showRepository.Delete(show);
            await _showRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
