using SV21T1020206.DomainModels;

namespace SV21T1020206.Shop.Models
{
    public class ProductViewModel
    {
        public int ProductID { get; set; } = 0;
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal minPrice { get; set; } = 0;
        public decimal maxPrice { get; set; } = 0;
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = string.Empty;
        public int RowCount { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0) return 1;

                int count = RowCount / PageSize;
                if (RowCount % PageSize > 0) ++count;

                return count;
            }
        }
        public List<Product> Data { get; set; } = new List<Product>();
    }
}
