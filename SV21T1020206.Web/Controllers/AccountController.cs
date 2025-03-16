using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020206.BusinessLayers;
using SV21T1020206.DomainModels;
using System.Security.Claims;

namespace SV21T1020206.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            ViewBag.UserName = userName;

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập Tài khoản và Mật Khẩu");
                return View();
            }
            
            var userAccount = UserAccountService.AuthorizeEmployee(UserTypes.Customer, userName, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //Login success: login status
            var userData = new WebUserData()
            {
                UserID = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Role = userAccount.RoleNames.Split(',').ToList()

            };
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        /// <summary>
        /// Đổi mật khẩu
        /// Nếu đổi mật khẩu thành công thì hệ thống sẽ chuyển người dùng về trang chủ
        /// </summary>
        /// <param name="oldPassword">Mật khẩu cũ</param>
        /// <param name="newPassword">Mật khẩu mới</param>
        /// <param name="confirmPassword">Xác nhận mật khẩu mới</param>
        /// <returns></returns>
        public IActionResult ChangePassword(string username = "", string oldPassword = "", string newPassword = "",
            string confirmPassword = "")
        {
            if (Request.Method != "POST") return View();


            // yêu cầu nhập đầy đủ các giá trị mật khẩu cũ, mật khẩu mới, xác nhận mật khẩu mới
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ các trường");
                return View();
            }
            var currentUser = UserAccountService.AuthorizeEmployee(UserTypes.Customer, username, oldPassword);

            // nếu currentUser == null => oldPassword sai
            if (currentUser == null)
            {
                ModelState.AddModelError(nameof(oldPassword), "Mật khẩu cũ không đúng, vui lòng nhập chính xác");
                return View();
            }

            if (newPassword == oldPassword)
            {
                ModelState.AddModelError(nameof(newPassword), "Vui lòng nhập mật khẩu mới khác mật khẩu cũ");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(nameof(confirmPassword), "Xác nhận mật khẩu mới không khớp, vui lòng nhập lại");
                return View();
            }

            bool result = UserAccountService.ChangePassword(UserTypes.Customer, oldPassword ,newPassword);

            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        public IActionResult EditProfile()
        {
            // Lấy thông tin người dùng hiện tại từ Claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Lấy thông tin chi tiết khách hàng
            var customer = CommonDataService.GetCustomer(userId);

            if (customer == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Title = "Chỉnh sửa thông tin cá nhân";
            return View(customer);
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditProfile(Customer data)
        {
            ViewBag.Title = "Chỉnh sửa thông tin cá nhân";

            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Bạn chưa chọn tỉnh/thành khách hàng");

            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                return View(data);
            }

            // Lấy UserId từ Claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            data.CustomerID = userId; // Đảm bảo ID khách hàng được giữ nguyên

            // Cập nhật thông tin
            bool result = CommonDataService.UpdateCustomer(data);

            if (!result)
            {
                ModelState.AddModelError(nameof(data.Email), "Cập nhật thông tin thất bại. Có thể email đã tồn tại.");
                return View(data);
            }

            // Thông báo cập nhật thành công (có thể sử dụng TempData)
            TempData["Message"] = "Cập nhật thông tin thành công";

            // Chuyển hướng về trang chủ hoặc trang profile
            return RedirectToAction("Index", "Home");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
