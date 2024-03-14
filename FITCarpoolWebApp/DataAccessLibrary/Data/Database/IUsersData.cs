using DataAccessLibrary.Model;

namespace DataAccessLibrary.Data.Database
{
    public interface IUsersData
    {
        Task AddUser(UsersModel user);
        Task DeleteUser(int id);
        Task<List<UsersModel>> GetAllUser();
        Task<List<UsersModel>> GetUser(int id);
        Task<List<UsersModel>> GetUser(string email);
        Task UpdateUser(UsersModel user);
        Task DeleteAccount(string email);
        Task UpdateUserProfilePicture(int userId, byte[] profilePicture);
        Task UpdateUserLicensePicture(int userId, byte[] licensePicture);
    }
}