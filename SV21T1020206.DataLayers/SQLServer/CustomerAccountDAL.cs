using Dapper;
using SV21T1020206.DomainModels;
using System.Data;
using System.Data.Common;

namespace SV21T1020206.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                // Truy vấn SQL để xác thực tài khoản người dùng
                var sql = @"select  CustomerID as UserId,
                                    Email as UserName,
                                    CustomerName as DisplayName
                                    
                                    
                            from Customers where Email = @Email and Password = @Password";

                var parameters = new
                {
                    Email = username,
                    Password = password
                };

                // Thực hiện truy vấn và lấy dữ liệu đầu tiên hoặc null nếu không tìm thấy
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public bool ChangePassword(string username, string newpassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // Truy vấn SQL để thay đổi mật khẩu người dùng
                var sql = @"UPDATE Customers
                            SET Password = @NewPassword
                            WHERE Email = @UserName";

                var parameters = new
                {
                    UserName = username,
                    NewPassword = newpassword,
                };

                // Thực hiện cập nhật mật khẩu
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }
        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Customers where Email = @Email)
                    select -1;
                    else
                    begin 
                        insert into Customers(CustomerName, ContactName, Province, Address, 
                                           Phone, Email, IsLocked, Password)
                        values (@CustomerName, @ContactName, @Province, @Address, 
                               @Phone, @Email, @IsLocked, @Password);
                        select scope_identity(); 
                    end";
                var parameter = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    IsLocked = data.IsLocked,
                    Password = data.Password
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: CommandType.Text);
            }
            return id;
        }


    }
}
