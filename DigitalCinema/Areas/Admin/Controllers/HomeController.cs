using DigitalCinema.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ImgesService movieService;

        public HomeController()
        {
            _context = new ApplicationDbContext();
            movieService = new ImgesService();
        }
        public IActionResult Index()
        {
            // جلب الأعداد من قاعدة البيانات
            ViewBag.MoviesCount = _context.Movies.Count();
            ViewBag.CinemasCount = _context.Cinemas.Count();
            ViewBag.ActorsCount = _context.Actors.Count();

            // إحصائية إضافية (اختياري): عدد الصور الفرعية الإجمالي
            ViewBag.SubImgsCount = _context.SupImgs.Count();

            return View();
        }

        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
