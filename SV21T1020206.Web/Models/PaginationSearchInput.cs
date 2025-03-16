namespace SV21T1020206.Web.Models
{
    /// <summary>
    /// Lưu giữ các thông tin đầu vào sử dụng cho chức năng tìm kiếm và hiển thị dữ liệu dưới dạng phân trang
    /// </summary>
    public class PaginationSearchInput
    {
        /// <summary>
        /// Trang cần hiển thị
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Số dòng hiển thị trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Chuỗi giá trị cần tìm kiếm
        /// </summary>
        public string SearchValue { get; set; } = string.Empty; // Sử dụng string.Empty thay cho "" để tăng tính rõ ràng
    }
        public class ProductSearchInput : PaginationSearchInput
        {
            public int CategoryID { get; set; } = 0;
            public int SupplierID { get; set; } = 0;
            public decimal minPrice { get; set; } = 0;
            public decimal maxPrice { get; set; } = 0;
    }
}
