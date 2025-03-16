
using SV21T1020206.DataLayers;
using SV21T1020206.DataLayers.SQLServer;
using SV21T1020206.DomainModels;

namespace SV21T1020206.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu chung
    /// (tỉnh/thành, khách hàng, nhà cung cấp, loại hàng, người giao hàng, nhân viên)
    /// </summary>
    public static class CommonDataService
    {
        private static readonly ISimpleQueryDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;
        /// <summary>
        /// Ctor ( Câu hỏi: static constructor hoạt động như nào?)
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;

            provinceDB = new ProvinceDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang 
        /// </summary>
        /// <param name="rowCount"> Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSige">Số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">Tên khách hàng hoặc tên giao dịch cần tìm</param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSige = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSige, searchValue).ToList();

        }
        public static List<Customer> ListOfCustomers()
        {
            
            return customerDB.List();

        }
        /// <summary>
        /// Bổ sung 1 khách hàng mới. Hàm trả về mã của khách hàng được bổ sung
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }

        /// <summary>
        /// Lấy thông tin của khách hàng dựa vào mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }

        /// <summary>
        /// Cập nhật thông tin của khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }

        /// <summary>
        /// Xoá 1 khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if (customerDB.InUsed(id))
            {
                return false;
            }
            return customerDB.Delete(id);
        }

        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang 
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSige">Số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">Tên Shipper cần tìm</param>
        /// <returns></returns>
        public static List<Shipper> ListOfShipper(out int rowCount, int page = 1, int pageSige = 0, String searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSige, searchValue);
        }
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }

        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }


        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }

        public static bool DeleteShipper(int id)
        {
            if (shipperDB.InUsed(id))
            {
                return false;
            }
            return shipperDB.Delete(id);
        }

        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang 
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSige">Số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">Tên Khach Hang cần tìm</param>
        /// <returns></returns>
        public static List<Supplier> ListOfSupplier(out int rowCount, int page = 1, int pageSige = 0, String searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSige, searchValue);
        }
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }

        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }


        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }

        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.InUsed(id))
            {
                return false;
            }
            return supplierDB.Delete(id);
        }

        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }
        public static List<Employee> ListOfEmployee(out int rowCount, int page = 1, int pageSige = 0, String searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSige, searchValue);

        }
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }

        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }


        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }

        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.InUsed(id))
            {
                return false;
            }
            return employeeDB.Delete(id);
        }

        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }
        public static List<Category> ListOfCategory(out int rowCount, int page = 1, int pageSige = 0, String searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSige, searchValue);
        }
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }

        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }


        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }

        public static bool DeleteCategory(int id)
        {
            if (categoryDB.InUsed(id))
            {
                return false;
            }
            return categoryDB.Delete(id);
        }

        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
        /// <summary>
        /// trả về danh sách toàn bộ tất cả các tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List();
        }
        public static List<Category> ListAllCategory()
        {
            return categoryDB.List();
        }

        public static List<Supplier> ListAllSupplier()
        {
            return supplierDB.List();
        }

    }
}
