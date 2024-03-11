using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class UsersData : IUsersData
    {
        private readonly ISQLDataAccess _db;

        public UsersData(ISQLDataAccess db)
        {
            _db = db;
        }
        public async Task<List<UsersModel>> GetAllUser()
        {
            string sql = "select * from Users";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { });
        }
        //  Get user using either an email or phone number
        public async Task<List<UsersModel>> GetUser(string email)
        {
            string sql = @"select * from Users where Users.email = @Email or Users.PhoneNumber = @Email";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { Email = email });
        }
        // Get user
        public async Task<List<UsersModel>> GetUser(int id)
        {
            string sql = @"select * from Users where Users.UserID = @ID";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { ID = id });
        }
        // Modify user
        public async Task UpdateUser(UsersModel user)
        {
            string sql = @"UPDATE Users 
                   SET Email = @Email, PasswordHash = @PasswordHash, Name = @Name, Phone = @Phone, UserType = @UserType 
                   WHERE UserID = @UserId";
            await _db.SaveData(sql, user);
        }
        // Delete User
        public async Task DeleteUser(int id)
        {
            string sql = @"DELETE FROM Users WHERE UserID = @ID";
            await _db.SaveData(sql, new { ID = id });
        }
    }
}
