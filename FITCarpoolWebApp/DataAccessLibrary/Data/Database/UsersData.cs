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
            string sql = @"SELECT * FROM Users";
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
                               DropoffLocation = @DropoffLocation, DrivingDistance = @DrivingDistance, PhonePrivacy = @PhonePrivacy, 
                               Gender = @Gender, ProfilePicture = @ProfilePicture, AddressPrivacy = @AddressPrivacy, 
                               BeltCount = @BeltCount, MakeModel = @MakeModel, VehicleColor = @VehicleColor, 
                               LicensePlate = @LicensePlate, LicensePlatePicture = @LicensePlatePicture, 
                               AllowEatDrink = @AllowEatDrink, AllowSmokeVape = @AllowSmokeVape
                           WHERE UserId = @UserId";
            await _db.SaveData(sql, user);
        }

        public async Task DeleteUser(int id)
        {
            string sql = @"DELETE FROM Users WHERE UserId = @ID";
            await _db.SaveData(sql, new { ID = id });
        }

        public async Task DeleteAccount(string email)
        {
            string sql = @"DELETE FROM AspNetUsers WHERE Email = @Email;";
            await _db.SaveData(sql, new { Email = email });
        }

        // Add a new user
        public async Task AddUser(UsersModel user)
        {
            string sql = @"INSERT INTO Users (Email, FirstName, LastName, Phone, UserType, UserLocation, 
                                               PickupLocation, DropoffLocation, DrivingDistance, PhonePrivacy, Gender, 
                                               ProfilePicture, AddressPrivacy, BeltCount, MakeModel, VehicleColor, 
                                               LicensePlate, LicensePlatePicture, AllowEatDrink, AllowSmokeVape) 
                           VALUES (@Email, @FirstName, @LastName, @Phone, @UserType, @UserLocation, 
                                   @PickupLocation, @DropoffLocation, @DrivingDistance, @PhonePrivacy, @Gender, 
                                   @ProfilePicture, @AddressPrivacy, @BeltCount, @MakeModel, @VehicleColor, 
                                   @LicensePlate, @LicensePlatePicture, @AllowEatDrink, @AllowSmokeVape)";
            await _db.SaveData(sql, user);
        }
        public async Task UpdateUserProfilePicture(int userId, byte[] profilePicture)
        {

            string sql = "UPDATE Users SET ProfilePicture = @ProfilePicture WHERE UserId = @UserId";
            await _db.SaveData(sql, new { UserId = userId, ProfilePicture = profilePicture });
        }
        public async Task UpdateUserLicensePicture(int userId, byte[] licensePicture)
        {

            string sql = "UPDATE Users SET LicensePlatePicture = @LicensePlatePic WHERE UserId = @UserId";
            await _db.SaveData(sql, new { UserId = userId, LicensePlatePic = licensePicture });
        }
    }
}