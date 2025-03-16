
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020206.BusinessLayers;
using SV21T1020206.DomainModels;
using SV21T1020206.Shop.Models;
using System.Globalization;

namespace SV21T1020206.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 20;

        private const int PRODUCT_PAGE_SIZE = 3;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchSale";

        public const string SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            OrderSearchInput? condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");

                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",

                };

            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status,
                condition.FromTime,
                condition.ToTime,
                condition.SearchValue ?? "");
            OrderSearchResult model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,

                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Checkout()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {


                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""

                };

            }
            return View(condition);
        }

        public IActionResult Cart()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {


                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""

                };

            }
            return View(condition);
        }

        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);
        }


        

        // Canceled - Hủy bỏ đơn hàng
        [HttpGet]
        public IActionResult Cancel(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return View("Details", new { id });
            }

            // Cập nhật trạng thái đơn hàng thành "Canceled"
            bool success = OrderDataService.CancelOrder(id);
            if (!success)
            {
                ModelState.AddModelError("", "Không thể hủy đơn hàng.");
                return View("Details", new { id });
            }

            // Nếu thành công, quay lại trang Details
            return RedirectToAction("Details", new { id });
        }

        // Rejected - Từ chối đơn hàng

        private List<Models.CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<Models.CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<Models.CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }

            return shoppingCart;
        }
        public ActionResult GetCartItems()
        {
            var cart = GetShoppingCart();
            return Json(cart);
        }
        [HttpPost]
        public IActionResult AddToCart(Models.CartItem item)
        {

            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }

            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            try
            {
                var shoppingCart = GetShoppingCart();
                if (shoppingCart.Count == 0)
                    return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");
                if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                    return Json("Vui lòng nhập đầy đủ thông tin");

                var userData = User.GetUserData();
                int employeeID = 1; //TODO: Thay bởi ID của nhân viên đang login vào hệ thống
                //if (userData != null)
                //{
                //    employeeID = int.Parse(userData.UserId);
                //}
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                foreach (var item in shoppingCart)
                {
                    orderDetails.Add(new OrderDetail()
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        SalePrice = item.SalePrice

                    });
                }
                int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
                ClearCart();
                return Json(orderID);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình xử lý đơn hàng.");
            }
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0." });

            var shoppingCart = GetShoppingCart();
            var product = shoppingCart.FirstOrDefault(m => m.ProductID == productId);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });

            product.Quantity = quantity;
            product.TotalPrice = product.Quantity * product.SalePrice;
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

            var totalPrice = shoppingCart.Sum(m => m.TotalPrice);
            return Json(new { success = true, totalPrice });
        }

    }
}