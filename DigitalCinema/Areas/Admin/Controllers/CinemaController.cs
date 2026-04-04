using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CinemaController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CinemaController()
        {
            _context = new ApplicationDbContext();
        }
        public IActionResult Index(int page =1, string? query = null)
        {
            var cinemas = _context.Cinemas.AsQueryable();
            //Filter
            //var retrivedQuery = query;
            if (query is not null)
            {
                cinemas = cinemas.Where(c => c.Name.ToLower().Contains(query.Trim().ToLower()));
                ViewBag.Query = query;
                //ViewData["Query"] = query;
            }
            //pagination
            double totalPages = Math.Ceiling(cinemas.Count() / 3.0);

            cinemas = cinemas.Skip((page - 1) * 3).Take(3);

            return View(new CinemasVM()
            {
                Cinemas = cinemas.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page
            });
        }
         
        [HttpGet]  
        public IActionResult Create ()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile Img)
        {
            if (Img is not null && Img.Length > 0)
            {

                var fileName = CreateFile(Img);

                cinema.Img = fileName;
            }

            _context.Cinemas.Add(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
         
        [HttpGet]
        public IActionResult Update(int id)
        {
            var cinema = _context.Cinemas.Find(id);

            if (cinema is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            return View(cinema);
        }
        [HttpPost]
        public IActionResult Update(Cinema cinema, IFormFile Img)
        {
            var cinemaInDB = _context.Cinemas.AsNoTracking().FirstOrDefault(c => c.Id == cinema.Id);
            if (Img is not null && Img.Length > 0)
            {

                var fileName = CreateFile(Img);

                var oldFilePath = GetFilePath(cinemaInDB.Img);

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
            _context.Cinemas.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.Find(id);

            if (cinema is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        private string CreateFile(IFormFile Img)
        {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + Path.GetExtension(Img.FileName);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Cinema", fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }
            return fileName;
        }

        private string GetFilePath(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Cinema", fileName);
            return filePath;
        }
    }
}
