using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020206.BusinessLayers;
using SV21T1020206.DataLayers.SQLServer;
using SV21T1020206.DomainModels;
using SV21T1020206.Shop.Models;
using System.Security.Claims;

namespace SV21T1020206.Shop.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Error", "Nhap tên và mật khẩu");
                return View();
            }
            //TODO: Kiểm tra username và pass hợp lệ
            var userAccount = UserAccountService.AuthorizeCustomer(UserTypes.Customer, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");

            }
            //đăng nhập thành công
            var userData = new WebUserData()
            {
                UserID = userAccount?.UserId,
                UserName = userAccount?.UserName,
                DisplayName = userAccount?.DisplayName,
                Photo = userAccount?.Photo,
                Role = userAccount.RoleNames.Split(',').ToList()

            };
            //Ghi nhận
            await HttpContext.SignInAsync(userData.CreatePrincipal());
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var username = User.FindFirstValue(nameof(WebUserData.UserName));

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu có khớp không
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return View();
            }
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "Không tìm thấy tên người dùng.");
                return View();
            }
            // Xác thực người dùng
            var userType = UserTypes.Customer;
            var userAccount = UserAccountService.AuthorizeCustomer(userType, username, oldPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng.");
                return View();
            }

            // Thay đổi mật khẩu
            bool isPasswordChanged = UserAccountService.ChangePassword(userType, username, newPassword);
            if (isPasswordChanged)
            {
                ViewBag.Message = "Đổi mật khẩu thành công!";
                return View();
            }
            else
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đổi mật khẩu. Vui lòng thử lại.");
                return View();
            }
        }
        public IActionResult EditProfile()
        {
            ViewBag.Title = "Cập nhật thông tin cá nhân";

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userData = CommonDataService.GetCustomer(int.Parse(userId));
            if (userData == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(userData);
        }

        [HttpPost]
        public IActionResult EditProfile(Customer model)
        {
            ViewBag.Title = "Cập nhật thông tin cá nhân";

            if (string.IsNullOrWhiteSpace(model.CustomerName))
                ModelState.AddModelError(nameof(model.CustomerName), "Tên khách hàng không được để trống");

            if (string.IsNullOrWhiteSpace(model.ContactName))
                ModelState.AddModelError(nameof(model.ContactName), "Tên giao dịch không được để trống");

            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không được để trống");

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            try
            {
                bool result = CommonDataService.UpdateCustomer(model);
                if (result == false)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email bị trùng");
                    return View("EditProfile", model);
                }

                TempData["Message"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index", "Product");
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn.");
                return View(model);
            }
        }

        public IActionResult Profile()
        {
            ViewBag.Title = "Thông tin cá nhân";

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userData = CommonDataService.GetCustomer(int.Parse(userId));
            if (userData == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(userData);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new Customer();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer customer, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu và xác nhận lại mật khẩu không khớp.";
                return View(customer);
            }

            var existingCustomer = CommonDataService.ListOfCustomers().FirstOrDefault(c => c.Email == customer.Email);
            if (existingCustomer != null)
            {
                ModelState.AddModelError("Error", "Email đã tồn tại");

                ViewBag.Error = "Email đã tồn tại.";
                return View(customer);
            }

            customer.IsLocked = false;
            int newCustomerID = CommonDataService.AddCustomer(customer); ;

            if (newCustomerID > 0)
            {
                ViewBag.Success = "Đăng ký thành công!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Error = "Đăng ký không thành công. Vui lòng thử lại.";
                return View(customer);
            }
        }
    }
}
