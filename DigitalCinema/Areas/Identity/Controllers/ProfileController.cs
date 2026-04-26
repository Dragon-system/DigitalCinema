using DigitalCinema.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DigitalCinema.Areas.Identity.Controllers
{
        [Area(SD.IDENTITY_AREA)]
        public class ProfileController : Controller
        {
            private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImgesService _ImegService;
        public ProfileController(UserManager<ApplicationUser> userManager, IImgesService imegService)
            {
                _userManager = userManager;
                _ImegService = imegService;
            }

            public async Task<IActionResult> Index()
            {
                var user = await _userManager.GetUserAsync(User);

                if (user is null) return NotFound();

                // Automapper, mappster

                var result = user.Adapt<ApplicationUserVM>();

                //var result = new ApplicationUserVM()
                //{
                //    Address = user.Address,
                //    Email = user.Email,
                //    FirstName = user.FName,
                //    LastName = user.LName,
                //    PhoneNumber = user.PhoneNumber,
                //    Id = user.Id
                //};

                return View(result);
            }

            [HttpPost]
            public async Task<IActionResult> Update(ApplicationUserVM applicationUserVM , IFormFile? imageProfile)
            {
            if (!ModelState.IsValid)
                return View("Index", applicationUserVM);

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            if (imageProfile is not null && imageProfile.Length > 0)
            {
                // امسح القديمة لو موجودة
                if (!string.IsNullOrEmpty(user.ImageProfile))
                {
                    var oldFilePath = _ImegService.GetOldFilePath(user.ImageProfile, "Profile");
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                var fileName = await _ImegService.CreateFileAsync(imageProfile, "Profile");
                user.ImageProfile = fileName;
            }

            user.Email = applicationUserVM.Email;
            user.FirstName = applicationUserVM.FirstName;
            user.LastName = applicationUserVM.LastName;
            user.PhoneNumber = applicationUserVM.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View("Index", applicationUserVM);
            }

            TempData["success-notification"] = "Update Profile Successfully";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult NewPassword()
        {
            return View();
        }

            [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM model)
        {
            if(!ModelState.IsValid)
                return View("Index", model);

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View("Index", model);
            }

            TempData["success-notification"] = "Password changed successfully";
            return RedirectToAction("Index");
        }
    }
    
}

