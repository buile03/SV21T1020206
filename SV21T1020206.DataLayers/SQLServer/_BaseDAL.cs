using Microsoft.Data.SqlClient;

namespace SV21T1020206.DataLayers.SQLServer
{
    public abstract class BaseDAL
    {
        protected string _connectionString = "";
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="connectionString">Chuỗi lưu trữ các tham số kết nối đến CSDL</param>
        public BaseDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected SqlConnection OpenConnection() 
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

    } 
}
