using DigitalCinema.Areas.Admin.Controllers;
using Ecommerce.ViewModels;
using Ecommerce.ViewModels.Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class MovieController : Controller
    {

        private readonly ApplicationDbContext _context;

        public MovieController()
        {
            _context = new ApplicationDbContext();
        }
        public IActionResult Index(MovieFilterVM MovieFilterVM, int page = 1)
        {
            var Movies = _context.Movies
                .Include(c => c.Category)
                .Include(c => c.Cinema)
                .AsQueryable();
            //Filter
            //var retrivedQuery = query;
            if (MovieFilterVM.Name is not null)
            {
                Movies = Movies.Where(c => c.Name.ToLower().Contains(MovieFilterVM.Name.Trim().ToLower()));
                ViewBag.Name = MovieFilterVM.Name;
                //ViewData["Query"] = query;
            }
            if (MovieFilterVM.MainPrice is not null)
            {
                Movies = Movies.Where(c => c.Price > MovieFilterVM.MainPrice);
                ViewBag.MainPrice = MovieFilterVM.MainPrice;

            }
            if (MovieFilterVM.MaxPrice is not null)
            {
                Movies = Movies.Where(c => c.Price < MovieFilterVM.MaxPrice);
                ViewBag.MaxPrice = MovieFilterVM.MaxPrice;

            }
            if (MovieFilterVM.CategoryId is not null)
            {
                Movies = Movies.Where(c => c.Category.Id == MovieFilterVM.CategoryId);
                ViewBag.CategoryId = MovieFilterVM.CategoryId;

            }
            if (MovieFilterVM.CinemaId is not null)
            {
                Movies = Movies.Where(c => c.Cinema.Id == MovieFilterVM.CinemaId);
                ViewBag.CinemaId = MovieFilterVM.CinemaId;

            }
            //pagination
            double totalPages = Math.Ceiling(Movies.Count() / 3.0);


            Movies = Movies.Skip((page - 1) * 3).Take(3);

            return View(new MoviesVM()
            {
                Movies = Movies.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Categories = _context.Categories.AsEnumerable(),
                Cinemas = _context.Cinemas.AsEnumerable(),
                //Query = retrivedQuery

            });

        }

        // صفحة عرض إنشاء قسم جديد
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Cinemas = _context.Cinemas.AsEnumerable();
            return View();

        }
        // صفحة إنشاء قسم جديد
        [HttpPost]
        public IActionResult Create(Movie Movie, IFormFile Img)
        {
            if (Img is not null && Img.Length > 0)
            {

                var fileName = CreateFile(Img);

                Movie.MainImg = fileName;
            }

            _context.Movies.Add(Movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // صفحة عرض تعديل قسم
        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Movies.Find(id);

            if (category is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            ViewBag.Categories = _context.Categories.AsEnumerable();
            ViewBag.Cinemas = _context.Cinemas.AsEnumerable();
            return View(category);
        }
        // صفحة تعديل قسم
        [HttpPost]
        public IActionResult Update(Movie Movie, IFormFile Img)
        {
            var MovieInDB = _context.Movies.AsNoTracking().FirstOrDefault(c => c.Id == Movie.Id);
            if (Img is not null && Img.Length > 0)
            {

                var fileName = CreateFile(Img);

                var oldFilePath = GetFilePath(MovieInDB.MainImg);

                if (Img is not null && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                Movie.MainImg = fileName;
            }
            else
            {
                Movie.MainImg = MovieInDB.MainImg;
            }
            _context.Movies.Update(Movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Movies.Find(id);

            if (category is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            _context.Movies.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        private string CreateFile(IFormFile Img)
        {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + Path.GetExtension(Img.FileName);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movies", fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }
            return fileName;
        }

        private string GetFilePath(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movies", fileName);
            return filePath;
        }
    }
}
