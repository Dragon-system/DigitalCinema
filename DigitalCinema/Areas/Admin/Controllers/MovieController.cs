using DigitalCinema.Areas.Admin.Controllers;
using DigitalCinema.Models;
using DigitalCinema.Services;
using DigitalCinema.ViewModel;
using DigitalCinema.ViewModels.DigitalCinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        //INSTANS DbContext ind MovieService
        private readonly ApplicationDbContext _context;
        private readonly MovieService movieService;

        public MovieController()
        {
            // 1. إنشاء نسخة من الـ DbContext
            _context = new ApplicationDbContext();
            // 2. إنشاء نسخة من الـ MovieService
            movieService = new MovieService();
        }
        public IActionResult Index(MovieFilterVM filter, int page = 1)
        {
            // 1. استعلام الأفلام مع الجداول الأساسية
            var query = _context.Movies
                .Include(c => c.Category)
                .Include(c => c.Cinema)
                .AsQueryable();

            // 2. تطبيق الفلترة
            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(c => c.Name.ToLower().Contains(filter.Name.Trim().ToLower()));

            if (filter.MainPrice.HasValue)
                query = query.Where(c => c.Price >= filter.MainPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= filter.MaxPrice);

            if (filter.CategoryId.HasValue && filter.CategoryId != 0)
                query = query.Where(c => c.CategoryId == filter.CategoryId);

            if (filter.CinemaId.HasValue && filter.CinemaId != 0)
                query = query.Where(c => c.CinemaId == filter.CinemaId);

            // 3. الترقيم (Pagination)
            int totalCount = query.Count();
            double totalPages = Math.Ceiling(totalCount / 3.0);
            var moviesResult = query.Skip((page - 1) * 3).Take(3).ToList();

            // 4. جلب الممثلين لكل فيلم عبر Dictionary لعدم وجود علاقة مباشرة في الموديل
            var movieActorsDict = new Dictionary<int, List<ActorMovie>>();
            foreach (var movie in moviesResult)
            {
                var actors = _context.ActorMovies
                    .Include(am => am.Actor)
                    .Where(am => am.MovieId == movie.Id)
                    .ToList();
                movieActorsDict.Add(movie.Id, actors);
            }
            // إرسال الـ Dictionary للـ View عبر ViewBag
            ViewBag.MovieActors = movieActorsDict;

            // 5. إرسال الموديل للـ View
            return View(new MoviesVM()
            {
                Movies = moviesResult,
                TotalPages = totalPages,
                CurrentPage = page,
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList(),
                // إرسال بيانات الفلترة للـ View لعرضها في الحقول المناسبة
                FilterName = filter.Name,
                FilterMaxPrice = filter.MaxPrice,
                FilterMinPrice = filter.MainPrice,
                FilterCategoryId = filter.CategoryId,
                FilterCinemaId = filter.CinemaId
            });
        }




        // صفحة عرض إنشاء قسم جديد
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.AsQueryable();
            var cinemas = _context.Cinemas.AsQueryable();
            var allActors = _context.Actors.AsQueryable(); // جلب كل الممثلين من جدول الممثلين

            return View(new MovieCreateModelVM()
            {
                Categories = categories,
                Cinemas = cinemas,
                Actors = allActors // إرسال القائمة للـ View
            });
        }
        // صفحة إنشاء قسم جديد
        [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Create(Movie Movie, IFormFile Img, List<IFormFile>? SubImgs, List<int>? selectedActorIds)
            {
                // 1. التعامل مع الصورة الأساسية
                if (Img is { Length: > 0 })
                {
                    Movie.MainImg = movieService.CreateFile(Img);
                }

                // 2. إضافة الفيلم أولاً للحصول على الـ ID (Primary Key)
                _context.Movies.Add(Movie);
                _context.SaveChanges();

                // 3. إضافة الصور الفرعية
                if (SubImgs is { Count: > 0 })
                {
                    var subImgEntities = SubImgs
                        .Where(f => f.Length > 0)
                        .Select(f => new SupImg
                        {
                            MovieId = Movie.Id,
                            SubImg = movieService.CreateFile(f, MovieImgType.SubImg)
                        });

                    _context.SupImgs.AddRange(subImgEntities);
                }

                // 4. إضافة الممثلين (باستخدام الـ IDs المحددة من الـ View)
                if (selectedActorIds is { Count: > 0 })
                {
                    var actorMovies = selectedActorIds.Select(id => new ActorMovie
                    {
                        MovieId = Movie.Id,
                        ActorId = id
                    });

                    _context.ActorMovies.AddRange(actorMovies);
                }

                // حفظ الصور والممثلين دفعة واحدة
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

        // ميثود عرض صفحة التعديل (HttpGet) - أضف هذا الجزء
        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null) return NotFound();

            var viewModel = new MovieWebSubImgVM
            {
                Movie = movie,
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList(),
                SubImgs = _context.SupImgs.Where(s => s.MovieId == id).ToList(),

                // القائمة التي تعرض في شريط التمرير (تحتاج Include لبيانات الممثل)
                ActorMovie = _context.ActorMovies
                                     .Include(am => am.Actor) // هذا السطر هو المسؤول عن إظهار الاسم والصورة
                                     .Where(am => am.MovieId == id)
                                     .ToList(),

                // القائمة التي تعرض في الـ Select للاختيار
                AllActors = _context.Actors.ToList()
            };

            return View(viewModel);
        }




        // صفحة تعديل قسم
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Movie Movie, IFormFile? Img, List<IFormFile>? SubImgs, List<int>? selectedActorIds)
        {
            // 1. جلب البيانات القديمة بدون تتبع (AsNoTracking) للمقارنة
            var movieInDb = _context.Movies.AsNoTracking().FirstOrDefault(m => m.Id == Movie.Id);
            if (movieInDb == null) return NotFound();

            // 2. معالجة الصورة الأساسية
            if (Img != null && Img.Length > 0)
            {
                Movie.MainImg = movieService.CreateFile(Img);
                // كود حذف الصورة القديمة من الـ wwwroot هنا
            }
            else
            {
                // مهم جداً: إذا لم يرفع صورة جديدة، احتفظ بالقديمة لكي لا تصبح Null
                Movie.MainImg = movieInDb.MainImg;
            }

            // 3. تحديث بيانات الفيلم الأساسية (الاسم، السعر، الوصف، التصنيف)
            _context.Movies.Update(Movie);

            // 4. تحديث الممثلين (حذف القديم وإضافة المختارين)
            if (selectedActorIds != null)
            {
                var oldActors = _context.ActorMovies.Where(am => am.MovieId == Movie.Id);
                _context.ActorMovies.RemoveRange(oldActors);

                foreach (var actorId in selectedActorIds)
                {
                    _context.ActorMovies.Add(new ActorMovie { MovieId = Movie.Id, ActorId = actorId });
                }
            }

            // 5. حفظ التغييرات النهائية (السطر الأهم)
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
        
    }
}

