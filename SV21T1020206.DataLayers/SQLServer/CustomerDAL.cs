using Dapper;
using SV21T1020206.DomainModels;
using System.Data;

namespace SV21T1020206.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến Khách hàng (Customer)
    /// </summary>
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// Thêm khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @" if exists (select * from Customers where Email = @Email)
                            select -1;
                            else
                            begin 
                                insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked, Password)
                                values (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked, @Password);
                                select scope_identity(); 
                            end";
                var parameter = new
                {
                    CustomerName= data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    IsLocked = data.IsLocked,
                    Password = data.Password
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: CommandType.Text);
                connection.Close();
                
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Customers
                            where (CustomerName like @searchValue ) or (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue

                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

            }
            return count;
        }
        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from customers where customerid = @customerid";
                var parameter = new
                {
                    customerid = id
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            
            using (var connection = OpenConnection())
            {
                var sql = @"select * from customers where customerid = @customerid   ";
                var parameter = new
                {
                    customerid = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameter, commandType: CommandType.Text);
                connection.Close();

            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from orders where customerid = @customerid)
                                select 1
                            else
                                select 0";
                var parameter = new
                {
                    customerid = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                            from    (
			                            select *, 
			                            row_number() over(order by CustomerName) as RowNumber
			                            from Customers
			                            where (CustomerName like @searchValue ) or (ContactName like @searchValue)
		                            )as t
                            where (@pageSize = 0)
                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber ";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue

                };
                data = connection.Query<Customer>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                return data;
            }
        }
        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @" if not exists (select * from Customers where CustomerID <> @CustomerID and Email = @Email)
                             begin 
                                update Customers
                                set CustomerName = @CustomerName,
                                ContactName = @ContactName,
                                Province = @Province,
                                Address = @Address,
                                Phone = @Phone,
                                Email = @Email,
                                Islocked =  @Islocked
                                where CustomerID = @CustomerID

                            end
                            ";
                var parameter = new
                {   
                    CustomerID = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",   
                    Province = data.Province ,
                    Address = data.Address,
                    Phone = data.Phone ,
                    Email = data.Email,
                    IsLocked = data.IsLocked
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

}
}
