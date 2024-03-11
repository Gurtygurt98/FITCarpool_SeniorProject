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
            string sql = "SELECT * FROM Users";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { });
        }

        public async Task<List<UsersModel>> GetUser(string email)
        {
            string sql = @"SELECT * FROM Users WHERE Users.Email = @Email OR Users.Phone = @Email";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { Email = email });
        }

        public async Task<List<UsersModel>> GetUser(int id)
        {
            string sql = @"SELECT * FROM Users WHERE Users.UserId = @ID";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { ID = id });
        }

        public async Task UpdateUser(UsersModel user)
        {
            string sql = @"UPDATE Users 
                           SET Email = @Email, FirstName = @FirstName, LastName = @LastName, Phone = @Phone, 
                               UserType = @UserType, UserLocation = @UserLocation, PickupLocation = @PickupLocation, 
                               DropoffLocation = @DropoffLocation, DrivingDistance = @DrivingDistance
                           WHERE UserId = @UserId";
            await _db.SaveData(sql, user);
        }

        public async Task DeleteUser(int id)
        {
            string sql = @"DELETE FROM Users WHERE UserId = @ID";
            await _db.SaveData(sql, new { ID = id });
        }

        // Add a new user
        public async Task AddUser(UsersModel user)
        {
            Console.WriteLine(user);
            string sql = @"INSERT INTO Users (Email, FirstName, Phone, UserType, UserLocation, 
                                               PickupLocation, DropoffLocation, DrivingDistance, LastName) 
                           VALUES (@Email, @FirstName,@Phone, @UserType, @UserLocation, 
                                   @PickupLocation, @DropoffLocation, @DrivingDistance, @LastName)";
            await _db.SaveData(sql, user);
        }
    }
}
