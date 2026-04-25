using DigitalCinema.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class HomeController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //private readonly ImgesService movieService;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Movie> _repository;
        private readonly IMovieSubImgRepository _subImageRepository;
        public HomeController(IRepository<Cinema> cinemaRepository, IRepository<Actor> actorRepository,
            IRepository<Movie> repository, IMovieSubImgRepository subImageRepository)
        {
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _repository = repository;
            _subImageRepository = subImageRepository;
        }
        public async Task<IActionResult> Index()
        {
            // جلب الأعداد من قاعدة البيانات
            var MoviesCount =  (await _repository.GetAsync()).Count();
           var CinemasCount = (await _cinemaRepository.GetAsync()).Count();
            var ActorsCount = (await _actorRepository.GetAsync()).Count();

            // إحصائية إضافية (اختياري): عدد الصور الفرعية الإجمالي
            var SubImgsCount =(await _subImageRepository.GetAsync()).Count();
            return View(new MACSHomeVM
            {
                MoviesCount = MoviesCount,
                CinemasCount = CinemasCount,
                ActorsCount = ActorsCount,
                SubImgsCount = SubImgsCount
            });

        }

        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
