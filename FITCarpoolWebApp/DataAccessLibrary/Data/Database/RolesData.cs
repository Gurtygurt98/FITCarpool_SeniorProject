using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class RolesData
    {
        private readonly ISQLDataAccess _db;

        public RolesData(ISQLDataAccess db)
        {
            _db = db;
        }
        public async Task<List<RolesModel>> GetUserRoles()
        {
            string sql = @"SELECT AspNetUsers.Id AS ID, AspNetUsers.UserName, AspNetRoles.Name as RoleName , " +
                "AspNetRoles.Id as RoleID" +
                "FROM AspNetUserRoles" +
                "JOIN AspNetUsers ON AspNetUserRoles.UserId = AspNetUsers.Id" +
                "JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id;";
            return await _db.LoadData<RolesModel, dynamic>(sql, new { });
        }
        public async Task<RolesModel> GetUserRole(string username)
        {
            string sql = @"SELECT AspNetUsers.Id AS ID, AspNetUsers.UserName, AspNetRoles.Name as RoleName , " +
                "AspNetRoles.Id as RoleID" +
                "FROM AspNetUserRoles" +
                "JOIN AspNetUsers ON AspNetUserRoles.UserId = AspNetUsers.Id" +
                "JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id" +
                "where AspNetUsers.email = @Username;";
            var result = await _db.LoadData<RolesModel, dynamic>(sql, new {Username = username });
            return result.FirstOrDefault();
        }
        public async Task AddAdmin(RolesModel user)
        {
            await DeleteUserRole(user);

            string adminRoleId = await GetAdminID();

            string sql = @"INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            await _db.SaveData(sql, new { UserId = user.ID, RoleId = adminRoleId });
        }
        public async Task<string> GetAdminID()
        {
            string sql = @"select Id from AspNetRoles Name = @Name";
            var result = await _db.LoadData<string, dynamic>(sql, new { Name = "Admin" });
            return result.FirstOrDefault();
        }
        public async Task DeleteUserRole(RolesModel user)
        {
            string sql = "DELETE FROM AspNetRoles WHERE UserID = @UserId";
            await _db.SaveData(sql, new { user });
        }
    }
}
