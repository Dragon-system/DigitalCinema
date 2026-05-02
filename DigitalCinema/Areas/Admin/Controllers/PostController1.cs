using DigitalCinema.DataAccess;
using DigitalCinema.Models;
using DigitalCinema.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Area(SD.ADMIN_AREA)]
[Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
public class PostController : Controller
{
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<PostComment> _postCommentRepository;
    private readonly IRepository<PostLike> _postLikeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IImgesService _imgieService;

    public PostController(
        IRepository<Post> postRepository,
        IRepository<PostComment> postCommentRepository,
        IRepository<PostLike> postLikeRepository,
        UserManager<ApplicationUser> userManager,
        IImgesService imgieService)
    {
        _postRepository = postRepository;
        _postCommentRepository = postCommentRepository;
        _postLikeRepository = postLikeRepository;
        _userManager = userManager;
        _imgieService = imgieService;
    }

    // ══════════════════════════════════════════
    // 1. عرض كل البوستات
    // ══════════════════════════════════════════
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var posts = await _postRepository.GetAsync(
            includes: [p => p.User, p => p.PostLikes, p => p.PostComments]);

        var postVMs = posts
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
                    AuthorName = $"{c.User.FirstName} {c.User.LastName}",
                    AuthorImage = c.User.ImageProfile,
                    CreatedAt = c.CreatedAt,
                }).ToList()
            }).ToList();

        return View(postVMs);
    }

    // ══════════════════════════════════════════
    // 2. صفحة إنشاء بوست (GET)
    // ══════════════════════════════════════════
    [HttpGet]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    public IActionResult Create()
    {
        return View();
    }

    // ══════════════════════════════════════════
    // 3. حفظ البوست (POST)
    // ══════════════════════════════════════════
    [HttpPost]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    public async Task<IActionResult> Create(CreaetPostVM postVM, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(postVM);

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        string? imageName = null;
        string? videoName = null;

        if (postVM.Image is not null && postVM.Image.Length > 0)
            imageName = await _imgieService.CreateFileAsync(postVM.Image, "PostImages");

        if (postVM.Video is not null && postVM.Video.Length > 0)
            videoName = await _imgieService.CreateFileAsync(postVM.Video, "PostVideos");

        var post = new Post()
        {
            Content = postVM.Content,
            ImagePath = imageName,
            VideoPath = videoName,
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id
        };

        await _postRepository.CreateAsync(post, cancellationToken);
        await _postRepository.CommitAsync(cancellationToken);
        TempData["success-notification"] = "تم نشر الإعلان بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════
    // 4. حذف البوست
    // ══════════════════════════════════════════
    [HttpPost]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var post = (await _postRepository.GetAsync(p => p.Id == id)).FirstOrDefault();
        if (post is null) return NotFound();

        if (!string.IsNullOrEmpty(post.ImagePath))
        {
            var imgPath = _imgieService.GetOldFilePath(post.ImagePath, "PostImages");
            if (System.IO.File.Exists(imgPath))
                System.IO.File.Delete(imgPath);
        }

        if (!string.IsNullOrEmpty(post.VideoPath))
        {
            var videoPath = _imgieService.GetOldFilePath(post.VideoPath, "PostVideos");
            if (System.IO.File.Exists(videoPath))
                System.IO.File.Delete(videoPath);
        }

        _postRepository.Delete(post);
        await _postRepository.CommitAsync(cancellationToken);
        TempData["success-notification"] = "Deleted Successfully";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════
    // 5. لايك / إلغاء لايك
    // ══════════════════════════════════════════
    [HttpPost]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    public async Task<IActionResult> Like(int postId, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);
        //if (userId is null)
        //{
        //    TempData["error-notification"] = "يجب تسجيل الدخول لإضافة تعليق";
        //    return RedirectToAction(nameof(Index));
        //}
        var existingLike = (await _postLikeRepository
            .GetAsync(l => l.PostId == postId && l.UserId == userId))
            .FirstOrDefault();
       
        if (existingLike is not null)
        {
             _postLikeRepository.Delete(existingLike);
        }
        else
        {
            await _postLikeRepository.CreateAsync(new PostLike()
            {
                PostId = postId,
                UserId = userId
            }, cancellationToken);
        }

        await _postLikeRepository.CommitAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════
    // 6. إضافة تعليق
    // ══════════════════════════════════════════
    [HttpPost]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    public async Task<IActionResult> Comment(int postId, string content, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["error-notification"] = "التعليق لا يمكن أن يكون فارغاً";
            return RedirectToAction(nameof(Index));
        }

        var userId = _userManager.GetUserId(User);
        //if (userId is null)
        //{
        //    TempData["error-notification"] = "يجب تسجيل الدخول لإضافة تعليق";
        //    return RedirectToAction(nameof(Index));
        //}
        await _postCommentRepository.CreateAsync(new PostComment()
        {
            PostId = postId,
            Text = content,
            UserId = userId
        }, cancellationToken);

        await _postCommentRepository.CommitAsync(cancellationToken);
        TempData["success-notification"] = "تم إضافة التعليق";
        return RedirectToAction(nameof(Index));
    }
}