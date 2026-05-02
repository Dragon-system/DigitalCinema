using DigitalCinema.Areas.Admin.Controllers;
using DigitalCinema.Models;
using DigitalCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class ActorController : Controller
    {

        // ===================== Repositories =====================

        private readonly IRepository<Actor> _actorRepository;


        // ===================== Services =====================
        private readonly IImgesService _ImegService;

        // ===================== Constructor =====================
        public ActorController(

            IRepository<Actor> actorRepository,

            IImgesService ImegService)
        {

            _actorRepository = actorRepository;

            _ImegService = ImegService;
        }
        public async Task<IActionResult> Index(int page =1, string? query = null, CancellationToken cancellationToken = default)
        {
            const int pageSize = 3;

            var actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken);

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();

                actors = actors.Where(c =>
                    c.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = actors.Count();

            var pagedActors = actors
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var totalPages = Math.Ceiling(totalCount / (double)pageSize);

            return View(new ActorsVM()
            {
                Actors = pagedActors,
                TotalPages = totalPages,
                CurrentPage = page,
                Query = query
            });
        }
         
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Create ()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Create(Actor actor, IFormFile Img,CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            if (Img is not null && Img.Length > 0)
            {

                var fileName = await _ImegService.CreateFileAsync(Img, "Actor");

                actor.Img = fileName;
            }

            await _actorRepository.CreateAsync(actor);
            await _actorRepository.CommitAsync(cancellationToken: cancellationToken);
            TempData["success-notification"] = "Add Actor Successfully";
            return RedirectToAction(nameof(Index));
        }
         
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int id)
        {
            var actor = await _actorRepository.GetOneAsync(c => c.Id == id);

            if (actor is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
           
            return View(actor);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(Actor actor, IFormFile Img, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            var actorInDB = await _actorRepository.GetOneAsync(c => c.Id == actor.Id, tracking: false, cancellationToken: cancellationToken);
            if (actorInDB is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            if (Img is not null && Img.Length > 0)
            {

                var fileName = await _ImegService.CreateFileAsync(Img, "Actor");

                var oldFilePath = _ImegService.GetOldFilePath(actorInDB.Img, "Actor");

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
            _actorRepository.Update(actor);
            await _actorRepository.CommitAsync(cancellationToken: cancellationToken);
            TempData["success-notification"] = "Update Actor Successfully";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (actor is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            _actorRepository.Delete(actor);
            await _actorRepository.CommitAsync(cancellationToken: cancellationToken);
            TempData["success-notification"] = "Delete Actor Successfully";
            return RedirectToAction(nameof(Index));
        }
       
    }
}
