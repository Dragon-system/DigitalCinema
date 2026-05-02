
using DigitalCinema.Models;
using DigitalCinema.Services;
using DigitalCinema.ViewModel;
using DigitalCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;


namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class MovieController : Controller
    {
        // ===================== Repositories =====================
        private readonly IRepository<Movie> _repository;
        private readonly IMovieSubImgRepository _subImageRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<ActorMovie> _actorMovieRepository;

        // ===================== Services =====================
        private readonly IImgesService _movieService;

        // ===================== Constructor =====================
        public MovieController(
            IRepository<Movie> repository,
            IMovieSubImgRepository subImageRepository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<Actor> actorRepository,
            IRepository<ActorMovie> actorMovieRepository,
            IImgesService movieService)
        {
            _repository = repository;
            _subImageRepository = subImageRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _actorMovieRepository = actorMovieRepository;
            _movieService = movieService;
        }
        public async Task<IActionResult> Index(MoviesVM filter, int page = 1, CancellationToken cancellationToken = default)
        {
            // 1. استعلام الأفلام مع الجداول الأساسية
            //var query = _context.Movies
            //    .Include(c => c.Category)
            //    .Include(c => c.Cinema)
            //    .AsQueryable();

            var query = await _repository.GetAsync(includes: [e => e.Cinema, e => e.Category], cancellationToken: cancellationToken);

            // 2. تطبيق الفلترة
            if (!string.IsNullOrEmpty(filter.FilterName))
                query = query.Where(c => c.Name.ToLower().Contains(filter.FilterName.Trim().ToLower()));

            if (filter.FilterMinPrice.HasValue)
                query = query.Where(c => c.Price >= filter.FilterMinPrice);

            if (filter.FilterMaxPrice.HasValue)
                query = query.Where(c => c.Price <= filter.FilterMaxPrice);

            if (filter.FilterCategoryId.HasValue && filter.FilterCategoryId != 0)
                query = query.Where(c => c.CategoryId == filter.FilterCategoryId);

            if (filter.FilterCinemaId.HasValue && filter.FilterCinemaId != 0)
                query = query.Where(c => c.CinemaId == filter.FilterCinemaId);
            // 3. الترقيم (Pagination)
            int totalCount = query.Count();
            double totalPages = Math.Ceiling(totalCount / 3.0);
            var moviesResult = query.Skip((page - 1) * 3).Take(3).ToList();

            // 4. جلب الممثلين لكل فيلم عبر Dictionary لعدم وجود علاقة مباشرة في الموديل
            var movieActorsDict = new Dictionary<int, List<ActorMovie>>();
            foreach (var movie in moviesResult)
            {
                //var actors = _context.ActorMovies
                //    .Include(am => am.Actor)
                //    .Where(am => am.MovieId == movie.Id)
                //    .ToList();
                var actors = await _actorMovieRepository.GetAsync(includes: [e => e.Actor], expression: m => m.MovieId == movie.Id, cancellationToken: cancellationToken);
                movieActorsDict.Add(movie.Id, actors.ToList());
            }
            // إرسال الـ Dictionary للـ View عبر ViewBag
            //Vi = movieActorsDict;

            // 5. إرسال الموديل للـ View
            return View(new MoviesVM()
            {
                Movies = moviesResult,
                TotalPages = totalPages,
                CurrentPage = page,
                Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken),
                Cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken),
                // إرسال بيانات الفلترة للـ View لعرضها في الحقول المناسبة
                FilterName = filter.FilterName,
                FilterMaxPrice = filter.FilterMaxPrice,
                FilterMinPrice = filter.FilterMinPrice,
                FilterCategoryId = filter.FilterCategoryId,
                FilterCinemaId = filter.FilterCinemaId
            });
        }
        // صفحة عرض إنشاء قسم جديد
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);
            //var movies = await _repository.GetAsync(cancellationToken: cancellationToken);
            var allActors = await _actorRepository.GetAsync(cancellationToken: cancellationToken); // جلب كل الممثلين من جدول الممثلين

            return View(new MovieCreateModelVM
            {
                Categories = categories,
                Cinemas = cinemas,
                Actors = allActors
            });
        }
        // صفحة إنشاء قسم جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Create(Movie Movie, IFormFile Img, List<IFormFile>? SubImgs, List<int>? selectedActorIds, CancellationToken cancellationToken = default)
        {
            if (Movie == null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            
            // 1. التعامل مع الصورة الأساسية
            if (Img is { Length: > 0 })
            {
                var fileName = await _movieService.CreateFileAsync(Img, "Movie");
                Movie.MainImg = fileName;
            }

            // 2. إضافة الفيلم أولاً للحصول على الـ ID (Primary Key)
            // 1. إضافة الفيلم
            await _repository.CreateAsync(Movie, cancellationToken);
            await _repository.CommitAsync(cancellationToken); // علشان ياخد Id

            // 2. الصور الفرعية
            if (SubImgs != null && SubImgs.Count > 0)
            {
                foreach (var item in SubImgs)
                {
                    var fileName = await _movieService.CreateFileAsync(item, "Movie/SubImgs");

                    await _subImageRepository.CreateAsync(new SupImg
                    {
                        MovieId = Movie.Id,
                        SubImg = fileName
                    }, cancellationToken);
                }
                await _repository.CommitAsync(cancellationToken);
            }

            // 3. الممثلين
            if (selectedActorIds != null && selectedActorIds.Count > 0)
            {
                foreach (var actorId in selectedActorIds)
                {
                    await _actorMovieRepository.CreateAsync(new ActorMovie
                    {
                        MovieId = Movie.Id,
                        ActorId = actorId
                    }, cancellationToken);
                }
            }

            // 4. حفظ نهائي
            await _repository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Add Movie Successfully";

            return RedirectToAction(nameof(Index));
            }


        // ميثود عرض صفحة التعديل (HttpGet) - أضف هذا الجزء
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            //var movie = _context.Movies.Find(id);
            var movie = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            
            if (movie == null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            var viewModel = new MovieWebSubImgVM
            {
                Movie = movie,
                Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken),
                Cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken),
                SubImgs = await _subImageRepository.GetAsync(s => s.MovieId == id, cancellationToken: cancellationToken),
                // القائمة التي تعرض في شريط التمرير (تحتاج Include لبيانات الممثل)
               
                ActorMovie = await _actorMovieRepository.GetAsync(includes: [e => e.Actor],
                expression: m => m.MovieId == id, cancellationToken: cancellationToken),

                // القائمة التي تعرض في الـ Select للاختيار
                AllActors = await _actorRepository.GetAsync(cancellationToken: cancellationToken)
            };

            return View(viewModel);
        }




        // صفحة تعديل قسم
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(Movie Movie, IFormFile? Img, List<IFormFile>? SubImgs, List<int>? selectedActorIds, CancellationToken cancellationToken = default)
        {
           
            // 1. جلب البيانات القديمة بدون تتبع (AsNoTracking) للمقارنة
            //var movieInDb = _context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == Movie.Id);
            var movieInDb = await _repository.GetOneAsync(e => e.Id == Movie.Id, cancellationToken: cancellationToken, tracking: false);
            if (movieInDb is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            if (Img is not null && Img.Length > 0)
            {

                var fileName = await _movieService.CreateFileAsync(Img, "Movie");

                var oldFilePath = _movieService.GetOldFilePath(movieInDb.MainImg, "Movie");

                if (Img is not null && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                Movie.MainImg = fileName;
            }
            else
            {
                Movie.MainImg = movieInDb.MainImg;
            }
            _repository.Update(Movie);
          
            //حذف الصور الفرعية القديمة
            var oldSubImgs = await _subImageRepository
                .GetAsync(x => x.MovieId == Movie.Id, cancellationToken: cancellationToken);

            foreach (var img in oldSubImgs)
            {
                _subImageRepository.Delete(img);
            }
            // 2. إضافة الصور الفرعية الجديدة
            if (SubImgs != null && SubImgs.Count > 0)
            {
                foreach (var file in SubImgs)
                {
                    if (file.Length > 0)
                    {
                        var fileName = await _movieService.CreateFileAsync(file, "Movie/SubImgs");

                        await _subImageRepository.CreateAsync(new SupImg
                        {
                            MovieId = Movie.Id,
                            SubImg = fileName
                        }, cancellationToken);
                    }
                }
            }
            // 2. حذف الممثلين القدامى
            var oldActors = await _actorMovieRepository
                .GetAsync(am => am.MovieId == Movie.Id, cancellationToken: cancellationToken);

            foreach (var actor in oldActors)
            {
                _actorMovieRepository.Delete(actor);
            }

            // 3. إضافة الجديد
            if (selectedActorIds != null && selectedActorIds.Count > 0)
            {
                foreach (var actorId in selectedActorIds)
                {
                    await _actorMovieRepository.CreateAsync(new ActorMovie
                    {
                        MovieId = Movie.Id,
                        ActorId = actorId
                    }, cancellationToken);
                }
            }

            // 4. حفظ مرة واحدة بس
            await _repository.CommitAsync(cancellationToken);
                TempData["success-notification"] = "Update Movie Successfully";

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var category =await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (category is null) return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            _repository.Delete(category);
            await _repository.CommitAsync(cancellationToken: cancellationToken);
            TempData["success-notification"] = "Delete Movie Successfully";
            return RedirectToAction(nameof(Index));
        }
        
    }
}

