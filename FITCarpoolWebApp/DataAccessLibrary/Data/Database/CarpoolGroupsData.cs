using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DataAccessLibrary.Model.CarpoolGroupsModel;

namespace DataAccessLibrary.Data.Database
{
    public class CarpoolGroupsData : ICarpoolGroupsData
    {
        private readonly ISQLDataAccess _db;

        public CarpoolGroupsData(ISQLDataAccess db)
            {
            _db = db;
        }

        public async Task<List<CarpoolGroupsModel>> GetAllCarpoolGroups()
        {
            string sql = "SELECT * FROM CarpoolGroups";
            var result =  await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { });
            foreach (var item in result)
            {
                item.Riders = await GetRiders(item.GroupId, item.DriverId);
            }
            return result;
        }

        public async Task<List<CarpoolGroupsModel>> GetCarpoolGroup(int groupId)
        {
            string sql = @"SELECT 
                               GroupID, GroupName,DriverID,Destination,
                                Users.FirstName || "" "" || Users.LastName as DriverName
                            FROM CarpoolGroups
                            JOIN Users on CarpoolGroups.DriverID = Users.UserId
                            WHERE GroupID = @GroupId";
            var result =  await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { GroupId = groupId });
            foreach (var item in result)
            {
                item.Riders = await GetRiders(item.GroupId, item.DriverId);
            }
            return result;
        }

        public async Task UpdateCarpoolGroup(CarpoolGroupsModel group)
        {
            string sql = @"UPDATE CarpoolGroups 
                           SET GroupName = @GroupName, DriverID = @DriverId, Destination = @Destination, MeetingPoint = @MeetingPoint 
                           WHERE GroupID = @GroupId";
            await _db.SaveData(sql, group);
        }

        public async Task DeleteCarpoolGroup(CarpoolGroupsModel group)
        {
            string sql = @"DELETE FROM CarpoolGroups WHERE GroupID = @GroupId";
            await _db.SaveData(sql, new { group.GroupId });
        }
        public async Task<List<CarpoolGroupsModel>> GetDriverGroups(int driverId)
        {
            string sql = @"SELECT 
                               GroupID, GroupName,DriverID,Destination,
                                Users.FirstName || '""' || Users.LastName as DriverName
                            FROM CarpoolGroups
                            JOIN Users on CarpoolGroups.DriverID = Users.UserId
                            WHERE DriverID = @DriverID";
            var result =  await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { DriverID = driverId });
            foreach (var item in result)
            {
                item.Riders = await GetRiders(item.GroupId, item.DriverId);
            }
            return result;
        }
        public async Task<List<RiderModel>> GetRiders(int groupId, int driverID)
        {
            string sql = @"SELECT GM.UserId as Id, U.PickupLocation as Location, U.FirstName || "" "" || U.LastName as Name
                            FROM CarpoolGroups G
                            JOIN GroupMembers GM on G.GroupID = GM.GroupID and GM.UserId != @driverID
                            JOIN Users U on GM.UserID = U.UserId
                            WHERE G.GroupID = @groupId";
            return await _db.LoadData<RiderModel, dynamic>(sql, new { groupId, driverID });
        }
        public async Task<List<CarpoolGroupsModel>> GetRiderGroups(int userID)
        {
            string sql = @"select 
                                  CG.GroupID, CG.GroupName,CG.DriverID,CG.Destination,
                                   Users.FirstName || ' ' ||  Users.LastName as DriverName
                            from GroupMembers GM 
                            JOIN CarpoolGroups CG on GM.GroupID = CG.GroupID
                            JOIN Users on CG.DriverID = Users.UserId
                            where GM.UserId = @userID";
            var result = await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { userID });
            foreach (var item in result)
            {
                item.Riders = await GetRiders(item.GroupId, item.DriverId);
            }
            return result;

        }
        public async Task CreateNewGroup(CarpoolGroupsModel carpoolGroup)
        {

            string sql = @"INSERT INTO CarpoolGroups (GroupName, DriverID, Destination, CreatorID) 
                   VALUES (@GroupName, @DriverId, @Destination, @CreatorID)";
            await _db.SaveData(sql, carpoolGroup);
            sql = @"select GroupID from CarpoolGroups where GroupName = @groupName";
            var result = await _db.LoadData<string, dynamic>(sql, new { groupName = carpoolGroup.GroupName });
            string GoalGroupID = result.First();
            sql = $@"
                INSERT INTO GroupMembers (GroupID, UserID, JoinDate)
                VALUES (@GroupId, @UserId, CURRENT_TIMESTAMP);";

            await _db.SaveData(sql, new { UserId = carpoolGroup.DriverId, GroupId = GoalGroupID });
        }
        public async Task<int> GetGroupNumber(string GroupName, int CreatorID)
        {
            string sql = @"select GroupID from CarpoolGroups where GroupName = @GroupName and CreatorID = @CreatorID ";
            var data =  await _db.LoadData<int, dynamic>(sql, new { GroupName, CreatorID });
            return data.FirstOrDefault();
        }
    }
}
