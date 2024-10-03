using Microsoft.AspNetCore.Mvc;

namespace SV21T1020206.Web.Controllers
{
    public class ProductController : Controller
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

        public ActionResult Photo(int id = 0, string method = "", int photold = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng"; return View();
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    return View();
                case "delete":
                    // TODO: Xoa ảnh (xóa trực tiếp, không cần confirm)
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public ActionResult Attribute(int id = 0, string method = "", int attributeld = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    return View();
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    return View();
                case "delete":
                    //TODO: Xóa thuộc tính (xóa trực tiếp, không cần confirm)
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");

            }
        }
    } }