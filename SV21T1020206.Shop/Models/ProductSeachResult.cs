using SV21T1020206.DomainModels;

namespace SV21T1020206.Shop.Models
{
    public class ProductSeachResult : OrderSearchResult
    {
        public List<Product> Data1 { get; set; } = new List<Product>();
    }
}
