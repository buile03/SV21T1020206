using SV21T1020206.DomainModels;

namespace SV21T1020206.Shop.Models
{
    public class ProductDetailsViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Photo { get; set; } = "";
        public List<Product> Data { get; set; } = new List<Product>();
    }
}
