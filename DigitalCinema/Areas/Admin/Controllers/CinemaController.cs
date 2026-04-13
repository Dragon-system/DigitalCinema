using DigitalCinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CinemaController : Controller
    {

        // ===================== Repositories =====================

        private readonly IRepository<Cinema> _cinemaRepository;


        // ===================== Services =====================
        private readonly IImgesService _ImegService;

        // ===================== Constructor =====================
        public CinemaController(
         
            IRepository<Cinema> cinemaRepository,
       
            IImgesService ImegService)
        {
            //_repository = repository;
            //_subImageRepository = subImageRepository;
        
            _cinemaRepository = cinemaRepository;
         
            _ImegService = ImegService;
        }
        public async Task<IActionResult> Index(int page =1, string? query = null, CancellationToken cancellationToken = default)
        {
            const int pageSize = 3;

            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();

                cinemas = cinemas.Where(c =>
                    c.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = cinemas.Count();

            var pagedCinemas = cinemas
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var totalPages = Math.Ceiling(totalCount / (double)pageSize);

            return View(new CinemasVM()
            {
                Cinemas = pagedCinemas,
                TotalPages = totalPages,
                CurrentPage = page,
                Query = query
            });
        }
         
        [HttpGet]  
        public IActionResult Create ()
        {
            return View(new Cinema());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile Img, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }
            if (Img is not null && Img.Length > 0)
            {

                var fileName = await _ImegService.CreateFileAsync(Img, "Cinema");

                cinema.Img = fileName;
            }

            await _cinemaRepository.CreateAsync(cinema, cancellationToken);
            await _cinemaRepository.CommitAsync(cancellationToken: cancellationToken);
            TempData["success-notification"] = "Add Cinema Successfully";
            return RedirectToAction(nameof(Index));
        }
         
        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            var cinema =await _cinemaRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (cinema is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            return View(cinema);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Cinema cinema, IFormFile Img, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }
            //var cinemaInDB = _context.Cinemas.AsNoTracking().FirstOrDefault(c => c.Id == cinema.Id);
            var cinemaInDB =await _cinemaRepository.GetOneAsync(e => e.Id == cinema.Id,tracking:false ,cancellationToken: cancellationToken);
            if (cinemaInDB is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            if (Img is not null && Img.Length > 0)
            {

                var fileName = await _ImegService.CreateFileAsync(Img, "Cinema");

                var oldFilePath = _ImegService.GetOldFilePath(cinemaInDB.Img, "Cinema");

                if (Img is not null && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                cinema.Img = fileName;
            }
            else
            {
                cinema.Img = cinemaInDB.Img;
            }
            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Update Cinema Successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var cinema =await _cinemaRepository.GetOneAsync(e=>e.Id == id ,cancellationToken: cancellationToken);
            
            if (cinema is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync(cancellationToken: cancellationToken);
            TempData["success-notification"] = "Delete Cinema Successfully";
            return RedirectToAction(nameof(Index));
        }
      
    }
}
