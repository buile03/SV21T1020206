using Microsoft.AspNetCore.Mvc;
using SV21T1020206.BusinessLayers;
using SV21T1020206.DomainModels;
using SV21T1020206.Shop.Models;
using System.Globalization;

namespace SV21T1020206.Shop.Controllers
{
    //[Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class CartController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 20;

        // Số dòng trên 1 trang khi hiển thị danh sách mặt hàng cần tìm kiếm khi lập đơn hàng
        private const int PRODUCT_PAGE_SIZE = 5;
        // Tên biến session để lưu điều kiện tìm kiếm mặt hàng khi lập đơn hàng
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchSale";
        private const string SHOPPING_CART = "ShoppingCart";
        /// <summary>
        /// Giao diện tìm kiếm và hiển thị kết quả tìm kiếm đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }
            return View(condition);
        }
        /// <summary>
        /// Tìm kiếm đơn hàng 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize,
                condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");

            var model = new OrderSearchResult()
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
        /// <summary>
        /// Hiển thị thông tin chi tiết của một đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Giao diện chọn người giao hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderId = id;
            return View();
        }
        /// <summary>
        /// Ghi nhận người giao hàng và chuyển trạng thái đơn hàng sang đang giao
        /// Trả về chuỗi khác rỗng nếu có lỗi hoặc đầu vào không hợp lệ
        /// Trả về chuỗi rỗng nếu thành công
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="shipperID">Mã người giao hàng</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
                return Json("Vui lòng chọn người giao hàng");

            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
                return Json("Đơn hàng không cho phép chuyển cho người giao hàng");

            return Json("");

        }

        /// <summary>
        /// Giao diện lập đơn hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }
        /// <summary>
        /// Tìm kiếm mặt hàng và đưa vào giỏ hàng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            // Chỉ hiển thị những sản phẩm đang bán
            var data = ProductDataService.ListProducts(out rowCount, condition.Page,
                condition.PageSize, condition.SearchValue ?? "");

            var model = new ProductSeachResult
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data1 = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

            return View(model);
        }

        /// <summary>
        /// Lấy giỏ hàng đang lưu trong session
        /// </summary>
        /// <returns></returns>
        private List<OrderDetail> GetShoppingCart()
        {
            // giỏ hàng là danh sách các mặt hàng (order detail) được chọn để bán trong đơn hàng
            // và được lưu trong session

            var shopppingCart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);
            if (shopppingCart == null)
            {
                shopppingCart = new List<OrderDetail>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shopppingCart);
            }

            return shopppingCart;
        }
        /// <summary>
        /// Bổ sung 1 mặt hàng vào giỏ hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddToCart(Models.CartItem item)
        {
            // Lấy giỏ hàng hiện có từ Session
            List<Models.CartItem> shoppingCart = ApplicationContext.GetSessionData<List<Models.CartItem>>(SHOPPING_CART) ?? new List<Models.CartItem>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = shoppingCart.FirstOrDefault(p => p.ProductID == item.ProductID);

            if (existingItem != null)
            {
                // Nếu đã có, tăng số lượng
                existingItem.Quantity += item.Quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.SalePrice;
            }
            else
            {
                // Nếu chưa có, thêm mới vào giỏ
                item.TotalPrice = item.Quantity * item.SalePrice;
                shoppingCart.Add(item);
            }

            // Lưu giỏ hàng vào Session
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

            return Json("");
        }
        /// <summary>
        /// Xóa 1 mặt hàng ra khỏi giỏ hàng
        /// </summary>
        /// <param name="id">Mã mặt hàng cần xóa khỏi giỏ hàng</param>
        /// <returns></returns>
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

            return Json("");
        }

        /// <summary>
        /// Xóa tất cả mặt hàng trong giỏ hàng
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Khởi tạo đơn hàng
        /// Trả về chuỗi khác rỗng thông báo lỗi nếu đầu vào không hợp lệ hoặc tạo đơn hàng không thành công
        /// Trả về mã đơn hàng được tạo thành công (là 1 giá trị số)
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="deliveryProvince"></param>
        /// <param name="deliveryAddress"></param>
        /// <returns></returns>
        public IActionResult Init(int customerID = 0, string deliveryProvince = "",
            string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống, không thể lập đơn hàng");
            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhap đay đu thông tin khach hang va nơi giao hang");
            int employeeID = 1;
            List<OrderDetail> orderDetail = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetail.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince,
                deliveryAddress, orderDetail);
            ClearCart();

            return Json(orderID);
        }
        /// <summary>
        /// Giao diện cập nhật thông tin đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Update(int id = 0)
        {
            return View();
        }
        /// <summary>
        /// Cập nhật thông tin đơn hàng với dữ liệu mới 
        /// Những thông tin có thể cập nhật sẽ phụ thuộc vào trạng thái đơn hàng 
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Save(int id = 0)
        {
            return View();
        }
        /// <summary>
        /// Chuyển đơn hàng sang trạng thái được duyệt 
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (!result)
                ViewBag.Message = "Không thể duyệt đơn hàng này";

            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Chuyển đơn hàng sang trạng thái kết thúc
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result)
                ViewBag.Message = "Không thể ghi nhận trạng thái kết thúc cho đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Chuyển đơn hàng sang trạng thái từ chối
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
                ViewBag.Message = "Không thể từ chối đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Xóa 1 đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
                ViewBag.Message = "Không thể xóa đơn hàng này!";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Xóa 1 mặt hàng trong đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productID">Mã mặt hàng</param>
        /// <returns></returns>
        public IActionResult DeleteDetail(int id = 0, int productID = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productID);
            if (!result)
                TempData["Message"] = "Không thể xóa mặt hàng ra khỏi đơn hàng";

            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái bị hủy
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
                ViewBag.Message = "Không thể hủy đơn hàng này!";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Giao diện sửa đổi thông tin mặt hàng trong đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productId">Mã sản phẩm</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);
            return View(model);
        }
        /// <summary>
        /// Cập nhật giá bán và số lượng mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="orderID">Mã đơn hàng</param>
        /// <param name="productID">Mã sản phẩm</param>
        /// <param name="Quantity">Số lượng bán</param>
        /// <param name="salePrice">Giá bán</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateDetail(int orderID = 0, int productID = 0, int Quantity = 0, decimal salePrice = 0)
        {
            if (Quantity <= 0)
                return Json("Số lượng phải lớn hơn 0");
            if (salePrice <= 0)
                return Json("Giá sản phẩm phải lớn hơn 0");

            bool result = OrderDataService.SaveOrderDetail(orderID, productID, Quantity, salePrice);
            if (!result)
                return Json("Không thể cập nhật chi tiết đơn hàng này");


            return Json("");
        }

    }
}