using Microsoft.AspNetCore.Mvc;

namespace SV21T1020206.Web.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            return View();
        }
        public IActionResult Delete(int id = 0)
        {
            return View();
        }
    }
}
