using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020206.BusinessLayers;
using SV21T1020206.DomainModels;
using SV21T1020206.Web.Models;

namespace SV21T1020206.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRole.ADMIN}")]
    public class CustomerController : Controller
    {
        public const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";


        public IActionResult Index()
        {
            // Lấy điều kiện tìm kiếm từ session
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);

            // Nếu không có điều kiện tìm kiếm, khởi tạo một đối tượng mới
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1, 
                    PageSize = 20, 
                    SearchValue = ""
                };
            }

            return View(condition);
        }


        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfCustomers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");

            CustomerSearchResult model = new CustomerSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);
            return View(model);
        }


        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            var data = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer data, string confirmPassword)
        {
            ViewBag.Title = "Đăng ký khách hàng";

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email");
            if (string.IsNullOrWhiteSpace(data.Password))
                ModelState.AddModelError(nameof(data.Password), "Vui lòng nhập mật khẩu");
            if (data.Password != confirmPassword)
                ModelState.AddModelError(nameof(confirmPassword), "Mật khẩu không khớp");

            // Nếu có lỗi, trả lại View kèm thông báo
            if (!ModelState.IsValid)
            {
                return View(data);
            }

            try
            {
                // Thêm khách hàng mới
                int result = CommonDataService.AddCustomer(data);

                if (result == -1)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng");
                    return View(data);
                }

                if (result > 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Đăng ký không thành công, vui lòng thử lại");
                return View(data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi, vui lòng thử lại sau");
                return View(data);
            }
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
