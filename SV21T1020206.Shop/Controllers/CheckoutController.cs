using Microsoft.AspNetCore.Mvc;

namespace SV21T1020206.Shop.Controllers
{
    public class CheckoutController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PlaceOrder(string fullName, string address, string phone)
        {
            return RedirectToAction("Confirmation");
        }
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
