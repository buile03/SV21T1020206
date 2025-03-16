using SV21T1020206.DataLayers;
using SV21T1020206.DataLayers.SQLServer;
using SV21T1020206.DomainModels;

namespace SV21T1020206.BusinessLayers
{
    public static class UserAccountService
    {
        public static readonly IUserAccountDAL employeeAcountDB;
        public static readonly IUserAccountDAL customerAcountDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;
            employeeAcountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAcountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);

        }
        public static UserAccount? AuthorizeEmployee(UserTypes userType, string username, string password)
        {
            //if (userType == UserTypes.Employee)
                return employeeAcountDB.Authorize(username, password);
            //else
            //    return customerAcountDB.Authorize(username, password);


        }
        public static UserAccount? AuthorizeCustomer(UserTypes userType, string username, string password)
        {
            //if (userType == UserTypes.Employee)
            //return employeeAcountDB.Authorize(username, password);
            //else
            return customerAcountDB.Authorize(username, password);


        }
        public static bool ChangePassword(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
                return employeeAcountDB.ChangePassword(username, password);
            else
                return customerAcountDB.ChangePassword(username, password);

        }

    }

    public enum UserTypes
    {
        Employee,
        Customer
    }
}