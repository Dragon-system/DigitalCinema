using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalCinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            var users = _userManager.Users.AsQueryable();

            if (query is not null)
            {
                users = users.Where(e => e.NormalizedUserName!.Contains(query.Trim().ToUpper()));
                ViewBag.Query = query;
            }
            double totalPages = Math.Ceiling(users.Count() / 3.0);
            var usersList = users.Skip((page - 1) * 3).Take(3).ToList();

            Dictionary<ApplicationUser, string> keyValuePairs = new Dictionary<ApplicationUser, string>();

            foreach (var item in usersList)
            {
                keyValuePairs.Add(item, (await _userManager.GetRolesAsync(item)).FirstOrDefault()!);
            }
            return View(new ApplicationUserWthFlturrVM()
            {
                UsersRoles = keyValuePairs.ToDictionary(),
                TotalPages = totalPages,
                CurrentPage = page

            });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, SD.SUPER_ADMIN_ROLE)) return NotFound();

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;



            return View(new UserWithRoleVM()
            {
                User = user,
                RoleName = userRole,
                identityRoles = _roleManager.Roles.AsEnumerable()
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(UserWithRoleVM userWithRoleVM)
        {

            var user = await _userManager.FindByIdAsync(userWithRoleVM.Id);

            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);

            await _userManager.AddToRoleAsync(user, userWithRoleVM.RoleName);
            TempData["Success-notification"] = $"Update Role To user: {user.UserName} Successfully";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> LockUnlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            user.LockoutEnabled = !user.LockoutEnabled;
            if (!user.LockoutEnabled)
            {
                user.LockoutEnd = DateTime.Now.AddDays(14);
                TempData["warning-notification"] = $"Lock user: {user.UserName} Successfully";
            }
            else
            {
                user.LockoutEnd = null;
                TempData["Success-notification"] = $"Unlock user: {user.UserName} Successfully";
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}

