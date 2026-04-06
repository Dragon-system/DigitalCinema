using DigitalCinema.Areas.Admin.Controllers;
using DigitalCinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ActorController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ActorController()
        {
            _context = new ApplicationDbContext();
        }
        public IActionResult Index(int page =1, string? query = null)
        {
            var actors = _context.Actors.AsQueryable();
            //Filter
            //var retrivedQuery = query;
            if (query is not null)
            {
                actors = actors.Where(c => c.Name.ToLower().Contains(query.Trim().ToLower()));
                ViewBag.Query = query;
                //ViewData["Query"] = query;
            }
            //pagination
            double totalPages = Math.Ceiling(actors.Count() / 3.0);

            actors = actors.Skip((page - 1) * 3).Take(3);

            return View(new ActorsVM()
            {
                Actors = actors.AsEnumerable(),
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
        public IActionResult Create(Actor actor, IFormFile Img)
        {
            if (Img is not null && Img.Length > 0)
            {

                var fileName = CreateFile(Img);

                actor.Img = fileName;
            }

            _context.Actors.Add(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
         
        [HttpGet]
        public IActionResult Update(int id)
        {
            var actor = _context.Actors.Find(id);

            if (actor is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            return View(actor);
        }
        [HttpPost]
        public IActionResult Update(Actor actor, IFormFile Img)
        {
            var actorInDB = _context.Actors.AsNoTracking().FirstOrDefault(c => c.Id == actor.Id);
            if (Img is not null && Img.Length > 0)
            {

                var fileName = CreateFile(Img);

                var oldFilePath = GetFilePath(actorInDB.Img);

                if (Img is not null && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                actor.Img = fileName;
            }
            else
            {
                actor.Img = actorInDB.Img;
            }
            _context.Actors.Update(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.Find(id);

            if (actor is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            _context.Actors.Remove(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        private string CreateFile(IFormFile Img)
        {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + Path.GetExtension(Img.FileName);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Actor", fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }
            return fileName;
        }

        private string GetFilePath(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Actor", fileName);
            return filePath;
        }
    }
}
