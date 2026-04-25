using Microsoft.AspNetCore.Mvc;

namespace DigitalCinema.Areas.Idintity.Controllers
{
   [Area(SD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
