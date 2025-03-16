using SV21T1020206.DomainModels;

namespace SV21T1020206.Shop.Models
{
    public class CartModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
