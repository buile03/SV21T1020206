using SV21T1020206.DomainModels;

namespace SV21T1020206.Shop.Models
{
    public class OrderSearchResult 
    {
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
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }
    
}
