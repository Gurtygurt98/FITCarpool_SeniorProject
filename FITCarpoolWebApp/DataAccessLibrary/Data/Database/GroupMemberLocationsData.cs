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

        public async Task<List<GroupMemberLocationsModel>> GetGroupMemberLocation(int userId)
        {
            string sql = "SELECT * FROM GroupMemberLocations WHERE  UserID = @UserId";
            return await _db.LoadData<GroupMemberLocationsModel, dynamic>(sql, new { UserId = userId });
        }

        public async Task UpdateGroupMemberLocation(GroupMemberLocationsModel location)
        {
            string sql = @"UPDATE GroupMemberLocations SET UserID = @UserId, GroupID = @GroupId, Latitude = @Latitude, Longitude = @Longitude, Timestamp = @Timestamp WHERE LocationID = @LocationId";
            await _db.SaveData(sql,location );
            //new { UserId = location.UserId, GroupId = location.GroupId, Latitude = location.Latitude, Longitude = location.Longitude, TimeStap = location.Timestamp }
        }

        public async Task DeleteGroupMemberLocation(int locationId)
        {
            string sql = "DELETE FROM GroupMemberLocations WHERE LocationID = @LocationId";
            await _db.SaveData(sql, new { LocationId = locationId });
        }
        public async Task AddGroupMemberLocation(GroupMemberLocationsModel location)
        {
            string sql = @"INSERT INTO GroupMemberLocations (UserID, TripID, Latitude, Longitude, Timestamp) 
                   VALUES (@UserId, @TripID, @Latitude, @Longitude, @Timestamp);";
            await _db.SaveData(sql, location);
        }

        public async Task<List<GroupMemberLocationsModel>> GetDriverLocations(int trip)
        {
            string sql = @"SELECT GML.UserID, GM.TripID, GML.Latitude, GML.Longitude, GML.Timestamp, GML.LocationID
                            FROM GroupMembers GM
                            JOIN GroupMemberLocations GML ON GM.GroupID = GML.GroupID
                            WHERE GM.UserId = @UserId;";
            return await _db.LoadData<GroupMemberLocationsModel, dynamic>(sql, new { UserId = trip });

        }
    }
}
