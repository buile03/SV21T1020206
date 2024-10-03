using Microsoft.AspNetCore.Mvc;

namespace SV21T1020206.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
        public IActionResult ChangePassWord()
        {
            return View();
        }
    }
}
