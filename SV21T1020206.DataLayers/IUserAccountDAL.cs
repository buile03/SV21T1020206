using SV21T1020206.DomainModels;

namespace SV21T1020206.DataLayers
{
    public interface IUserAccountDAL
    {
        UserAccount? Authorize(string username, string password);
        bool ChangePassword(string username, string newpassword);

    }
}
