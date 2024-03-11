using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class GroupMemberLocationsData : IGroupMemberLocationsData
    {
        private readonly ISQLDataAccess _db;

        public GroupMemberLocationsData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<GroupMemberLocationsModel>> GetAllGroupMemberLocations()
        {
            string sql = "SELECT * FROM GroupMemberLocations";
            return await _db.LoadData<GroupMemberLocationsModel, dynamic>(sql, new { });
        }

        public async Task<GroupMemberLocationsModel> GetGroupMemberLocation(int locationId)
        {
            string sql = "SELECT * FROM GroupMemberLocations WHERE LocationID = @LocationId";
            var result = await _db.LoadData<GroupMemberLocationsModel, dynamic>(sql, new { LocationId = locationId });
            return result.FirstOrDefault();
        }

        public async Task UpdateGroupMemberLocation(GroupMemberLocationsModel location)
        {
            string sql = @"UPDATE GroupMemberLocations SET UserID = @UserId, GroupID = @GroupId, Latitude = @Latitude, Longitude = @Longitude, Timestamp = @Timestamp WHERE LocationID = @LocationId";
            await _db.SaveData(sql, location);
        }

        public async Task DeleteGroupMemberLocation(int locationId)
        {
            string sql = "DELETE FROM GroupMemberLocations WHERE LocationID = @LocationId";
            await _db.SaveData(sql, new { LocationId = locationId });
        }
    }
}
