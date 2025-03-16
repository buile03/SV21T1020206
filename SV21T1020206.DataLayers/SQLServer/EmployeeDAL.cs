using Dapper;
using SV21T1020206.DomainModels;
using System.Data;

namespace SV21T1020206.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến nhân viên (Customer)
    /// </summary>
    public class EmployeeDAL : BaseDAL, ICommonDAL<Employee>
    {
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// Thêm nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @" if exists (select * from Employees where Email = @Email)
                            select -1;
                            else
                            begin 
                                insert into Employees(FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking)
                                values (@FullName, @BirthDate, @Address, @Phone, @Email, @Password,@Photo, @IsWorking);
                                select scope_identity();    
                            end
                            ";
                var parameter = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    Password= data.Password,
                    Photo = data.Photo,
                    IsWorking = data.IsWorking
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
                            from Employees
                            where (FullName like @searchValue )";
                var parameters = new
                {
                    searchValue

                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

            }
            return count;
        }
        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Employees where EmployeeID = @EmployeeID";
                var parameter = new
                {
                    EmployeeID = id
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Lấy đối tượng Employee từ cơ sở dữ liệu dựa trên ID nhân viên được cung cấp.
        /// </summary>
        /// <param name="id">ID của nhân viên cần lấy.</param>
        /// <returns>
        /// Trả về đối tượng Employee nếu tìm thấy; ngược lại, trả về null.
        /// </returns>
        public Employee? Get(int id)
        {
            Employee? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select * from Employees where EmployeeID = @EmployeeID   ";
                var parameter = new
                {
                    EmployeeID = id
                };
                data = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameter, commandType: CommandType.Text);
                connection.Close();

            }
            return data;
        }

        /// <summary>
        /// Kiểm tra xem một khách hàng có đơn hàng nào trong cơ sở dữ liệu hay không.
        /// </summary>
        /// <param name="id">ID của khách hàng cần kiểm tra.</param>
        /// <returns>
        /// Trả về true nếu khách hàng có ít nhất một đơn hàng; ngược lại, trả về false.
        /// </returns>
        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from orders where EmployeeID = @EmployeeID)
                                select 1
                            else
                                select 0";
                var parameter = new
                {
                    EmployeeID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách khách hàng từ cơ sở dữ liệu, có thể phân trang và tìm kiếm theo tên hoặc tên liên hệ.
        /// </summary>
        /// <param name="page">Số trang cần lấy (mặc định là 1).</param>
        /// <param name="pageSize">Số lượng khách hàng trên mỗi trang (mặc định là 0, lấy tất cả nếu không chỉ định).</param>
        /// <param name="searchValue">Giá trị tìm kiếm theo tên khách hàng hoặc tên liên hệ (mặc định là chuỗi rỗng).</param>
        /// <returns>
        /// Danh sách các đối tượng Customer.
        /// </returns>
        public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> data = new List<Employee>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from    (
			                            select *, 
			                            row_number() over(order by FullName) as RowNumber
			                            from Employees
			                            where (FullName like @searchValue )
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
                data = connection.Query<Employee>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                return data;
            }
        }


        /// <summary>
        /// sửa nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Employee data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists (select * from Employees where EmployeeID <> @EmployeeID and Email = @Email)
                             begin 
                                update Employees
                                    set FullName = @FullName,
                                    BirthDate = @BirthDate,
                                    Address = @Address,
                                    Phone = @Phone,
                                    Email = @Email,
                                    Password = @Password,
                                    Photo = @Photo,
                                    IsWorking =  @IsWorking
                                where EmployeeID = @EmployeeID
                                end";
                var parameter = new
                {
                    EmployeeID = data.EmployeeID,
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    Password = data.Password,
                    Photo = data.Photo,
                    IsWorking = data.IsWorking
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

    }
}
