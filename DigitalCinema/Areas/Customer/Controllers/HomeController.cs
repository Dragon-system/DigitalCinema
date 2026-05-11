using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalCinema.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<PostComment> _postCommentRepository;
        private readonly IRepository<PostLike> _postLikeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImgesService _imgieService;
        private readonly IRepository<ActorMovie> _actorMovieRepository;
        private readonly IRepository<SupImg> _supImgRepository;
        private readonly IRepository<Show> _showRepository;
        public HomeController(IRepository<PostLike> postLikeRepository, 
            UserManager<ApplicationUser> userManager, IImgesService imgieService, IRepository<Movie> movieRepository,
        IRepository<Post> postRepository, IRepository<PostComment> postCommentRepository, IRepository<ActorMovie> actorMovieRepository, 
        IRepository<SupImg> supImgRepository, IRepository<Show> showRepository)
        {
            _postLikeRepository = postLikeRepository;
            _userManager = userManager;
            _imgieService = imgieService;
            _postRepository = postRepository;
            _postCommentRepository = postCommentRepository;
                _movieRepository = movieRepository;
            _actorMovieRepository = actorMovieRepository;
            _supImgRepository = supImgRepository;  
            _showRepository = showRepository;
        }



        public async Task<IActionResult> Index(string? CategoryName, CancellationToken cancellationToken)
        {
            // 1. جلب كل الأفلام مع Category
            var movies = await _movieRepository.GetAsync(
                cancellationToken: cancellationToken,
                includes: [m => m.Category]
            );

            // 2. عدد الأفلام في كل فئة
            var categoryWithTotal = movies
                .GroupBy(e => e.Category.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToDictionary(k => k.Name, v => v.Count);

            // 3. الفلترة حسب الفئة المختارة
            var moviesQuery = movies.AsQueryable();

            if (!string.IsNullOrEmpty(CategoryName))
            {
                moviesQuery = moviesQuery.Where(e => e.Category.Name == CategoryName);
            }

            // 4. أحدث 8 أفلام
            var finalMovies = moviesQuery
                .OrderByDescending(m => m.Id)
                .Take(8)
                .ToList();

            return View(new MovieWithCatogryVM
            {
                Movies = finalMovies,
                CategoryWithTotal = categoryWithTotal,
                SelectedCategory = CategoryName
            });
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(
                e => e.Id == id,
                includes: [e => e.Category, e => e.Cinema],
                cancellationToken: cancellationToken);

            if (movie is null) return NotFound();

            // ✅ جيب العروض الخاصة بالفيلم ده
            var shows = await _showRepository.GetAsync(
                s => s.ShowMovieHall.MovieId == id,
                includes: [s => s.ShowMovieHall],
                cancellationToken: cancellationToken);

            var actorMovies = await _actorMovieRepository.GetAsync(
                e => e.MovieId == id,
                includes: [e => e.Actor],
                cancellationToken: cancellationToken);

            var supImgs = await _supImgRepository.GetAsync(
                e => e.MovieId == id,
                cancellationToken: cancellationToken);

            var allMovies = await _movieRepository.GetAsync(
                includes: [p => p.Category],
                cancellationToken: cancellationToken);

            var query = allMovies.AsQueryable();

            var relatedVM = new MovieWithRelatedVM
            {
                Movie = movie,
                Shows = shows.ToList(), // ✅
                ActorMovies = actorMovies.ToList(),
                SupImgs = supImgs.ToList(),
                SamCategory = query
                    .Where(e => e.CategoryId == movie.CategoryId && e.Id != movie.Id)
                    .Take(4).ToList(),
                SamName = query
                    .Where(e => e.Name.Contains(movie.Name) && e.Id != movie.Id)
                    .Take(4).ToList(),
            };

            return View(relatedVM);
        }


        [HttpGet]
        public async Task<IActionResult> Post(CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            var posts = await _postRepository.GetAsync(
                includes: [p => p.User, p => p.PostLikes, p => p.PostComments],
                cancellationToken: cancellationToken);

            var postVMs = posts?
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostVM()
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImagePath = p.ImagePath,
                    VideoPath = p.VideoPath,
                    CreatedAt = p.CreatedAt,
                    AuthorImage = p.User.ImageProfile,
                    AuthorName = $"{p.User.FirstName} {p.User.LastName}",
                    LikesCount = p.PostLikes.Count,
                    IsLikedByCurrentUser = p.PostLikes.Any(l => l.UserId == userId),
                    Comments = p.PostComments.Select(c => new CommintVM()
                    {
                        Id = c.Id,
                        Content = c.Text,
                        AuthorName = c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : "مجهول",
                        AuthorImage = c.User?.ImageProfile,
                        CreatedAt = c.CreatedAt
                    }).ToList()
                }).ToList() ?? new List<PostVM>();

            return View(postVMs);
        }
        [HttpPost]
        public async Task<IActionResult> Like(int postId, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null) return RedirectToAction(nameof(Post));

            var existingLike = (await _postLikeRepository
                .GetAsync(l => l.PostId == postId && l.UserId == userId))
                .FirstOrDefault();

            if (existingLike is not null)
                _postLikeRepository.Delete(existingLike);
            else
                await _postLikeRepository.CreateAsync(new PostLike()
                {
                    PostId = postId,
                    UserId = userId
                }, cancellationToken);

            await _postLikeRepository.CommitAsync(cancellationToken);
            //return RedirectToAction(nameof(Post));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Comment(int postId, string content, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction(nameof(Post));

            var userId = _userManager.GetUserId(User);
            if (userId is null) return RedirectToAction(nameof(Post));

            await _postCommentRepository.CreateAsync(new PostComment()
            {
                PostId = postId,
                Text = content,
                UserId = userId
            }, cancellationToken);

            await _postCommentRepository.CommitAsync(cancellationToken);
            //return RedirectToAction(nameof(Post));
            return Ok();
        }
    }
}
