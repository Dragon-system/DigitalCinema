using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace DigitalCinema.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<PostComment> _postCommentRepository;
        private readonly IRepository<PostLike> _postLikeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImgesService _imgieService;

        public HomeController(IRepository<PostLike> postLikeRepository, 
            UserManager<ApplicationUser> userManager, IImgesService imgieService , 
            IRepository<Post> postRepository, IRepository<PostComment> postCommentRepository)
        {
            _postLikeRepository = postLikeRepository;
            _userManager = userManager;
            _imgieService = imgieService;
            _postRepository = postRepository;
            _postCommentRepository = postCommentRepository;
        }

     

        public IActionResult Index()
        {
            return View();
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
