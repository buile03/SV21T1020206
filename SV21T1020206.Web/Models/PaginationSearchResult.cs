using SV21T1020206.DomainModels;

namespace SV21T1020206.Web.Models
{
    public abstract class PaginationSearchResult
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

    }
    public class CustomerSearchResult : PaginationSearchResult
    {
        public List<Customer> Data { get; set; } = new List<Customer>();
    }

    public class ShipperSearchResult : PaginationSearchResult
    {
        public List<Shipper> Data { get; set; } = new List<Shipper>();
    }

    public class CategorySearchResult : PaginationSearchResult
    {
        public List<Category> Data { get; set; } = new List<Category>();
    }

    public class EmployeeSearchResult : PaginationSearchResult
    {
        public List<Employee> Data { get; set; } = new List<Employee>();
    }

    public class SupplierSearchResult : PaginationSearchResult
    {
        public List<Supplier> Data { get; set; } = new List<Supplier>();
    }

    public class ProductSeachResult : PaginationSearchResult
    {
        public List<Product> Data { get; set; } = new List<Product>();
    }


}

    
