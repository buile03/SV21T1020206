using Dapper;
using SV21T1020206.DomainModels;
using System.Data;

namespace SV21T1020206.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến Khách hàng (Customer)
    /// </summary>
    public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// Thêm người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Shippers where Phone = @Phone)
                            select -1;
                            else
                            begin 
                                insert into Shippers(ShipperName, Phone)
                                values (@ShipperName, @Phone);
                                select scope_identity();    
                            end
                            ";
                var parameter = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone
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
                            from Shippers
                            where (ShipperName like @searchValue )";
                var parameters = new
                {
                    searchValue

                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

            }
            return count;
        }
        /// <summary>
        /// Xóa người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Shippers where ShipperID = @ShipperID";
                var parameter = new
                {
                    ShipperID = id
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select * from Shippers where ShipperID = @ShipperID   ";
                var parameter = new
                {
                    ShipperID = id
                };
                data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameter, commandType: CommandType.Text);
                connection.Close();

            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from orders where ShipperID = @ShipperID)
                                select 1
                            else
                                select 0";
                var parameter = new
                {
                    ShipperID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> data = new List<Shipper>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from    (
			                            select *, 
			                            row_number() over(order by ShipperName) as RowNumber
			                            from Shippers
			                            where (ShipperName like @searchValue )
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
                data = connection.Query<Shipper>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                return data;
            }
        }
        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists (select * from Shippers where ShipperID <> @ShipperID and Phone = @Phone)
                             begin 
                                update Shippers
                                set ShipperName = @ShipperName,
                                Phone = @Phone
                            
                            where ShipperID = @ShipperID
end
                            ";
                var parameter = new
                {
                    ShipperID = data.ShipperID,
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone
                    
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

    }
}
