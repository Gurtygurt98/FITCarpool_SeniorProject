using DataAccessLibrary.Model;

namespace DataAccessLibrary.Data.Database
{
    public interface IUsersData
    {
        Task DeleteUser(int id);
        Task<List<UsersModel>> GetAllUser();
        Task<List<UsersModel>> GetUser(int id);
        Task<List<UsersModel>> GetUser(string email);
        Task UpdateUser(UsersModel user);
    }
}