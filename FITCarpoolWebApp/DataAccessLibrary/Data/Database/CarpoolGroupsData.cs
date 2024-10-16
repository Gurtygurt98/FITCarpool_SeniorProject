using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DataAccessLibrary.Model.CarpoolGroupsModel;

namespace DataAccessLibrary.Data.Database
{
    public class CarpoolGroupsData : ICarpoolGroupsData
    {
        private readonly ISQLDataAccess _db;
        private readonly IUsersData _dbUsers;
        private readonly ISchedulesData _dbSchedule;


        public CarpoolGroupsData(ISQLDataAccess db, IUsersData usersData, ISchedulesData schedules)
        {
            _db = db;
            _dbUsers = usersData;
            _dbSchedule = schedules;   
        }
        // build a groups from groups that dont exist 
        public async Task<List<RecomendedGroup>> GetRecomendedGroups(int GoalUserID, string Direction)
        {
            List<RecomendedGroup> RecomendedGroups = new();
            // Get all the users 
            List<UserInfoModel> MatchingUsers = new();


            return RecomendedGroups;

        }
        public async Task<List<CarpoolGroupsModel>> GetAllCarpoolGroups()
        {
            string sql = "SELECT * FROM CarpoolGroups";
            var result = await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { });
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
            var result = await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { GroupId = groupId });
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
            var result = await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { DriverID = driverId });
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
            var data = await _db.LoadData<int, dynamic>(sql, new { GroupName, CreatorID });
            return data.FirstOrDefault();
        }
        public async Task<List<RecomendedGroup>> GetAvailableGroups(int GoalUserID, List<string> Days)
        {
            // Query to get all the matching groups 
            string sql = $@"
                    SELECT s2.GroupID, CG.GroupName, s2.Direction
                    FROM Schedules s1
                    JOIN GroupSchedule s2 on
                        s1.Day = s2.Day 
                        AND s1.Text = s2.Direction
                        AND (s1.Start <= s2.End AND s1.End >= s2.Start)
                    JOIN Users u on s1.UserID = u.UserID
                    LEFT JOIN GroupMembers GM on GM.GroupID = s2.GroupID AND GM.UserID = s1.UserID
                    JOIN CarpoolGroups CG on s2.GroupID = CG.GroupID
                    WHERE s1.UserID = @UserId
                    AND s1.Day IN @Days AND GM.UserID IS NULL;";

            List<(int, string, string)> MatchingScheduleGroupIDs = await _db.LoadData<(int, string, string), dynamic>(sql, new { UserId = GoalUserID, Days });

            // Get the user info for the goal model 
            UserInfoModel GoalUser = await _dbUsers.GetUserInfoModel(GoalUserID);
            List<RecomendedGroup> RecommendedGroups = new List<RecomendedGroup>();
            foreach ((int GroupID, string GroupName, string direction) in MatchingScheduleGroupIDs)
            {
                List<int> GroupMemberIDs = await GetUserIds(GroupID);
                List<UserInfoModel> GroupMembers = new List<UserInfoModel>();
                foreach (int MemberID in GroupMemberIDs)
                {
                    GroupMembers.Add(await _dbUsers.GetUserInfoModel(MemberID));
                }
                RecommendedGroups.Add(new RecomendedGroup(GroupName, GroupID, GroupMembers, GoalUser, direction));
            }
            return RecommendedGroups;
        }
        public async Task<List<int>> GetUserIds(int GroupID)
        {
            string sql = @"select UserID from GroupMembers where GroupID = @groupID";
            return await _db.LoadData<int, dynamic>(sql, new { groupID = GroupID });

        }
        // Get the current groups a user is apart of 
        public async Task<List<(int, string, int, int)>> GetCurrentGroups(int GoalUserID)
        {
            // Query to get all the matching groups 
            string sql = $@"
                    SELECT GM.GroupID, CG.GroupName, CG.CreatorID,GM.UserID
                    FROM GroupMembers GM
                    JOIN CarpoolGroups CG on GM.GroupID = CG.GroupID
                    WHERE GM.UserID = @UserId;";

            return await _db.LoadData<(int, string, int, int), dynamic>(sql, new { UserId = GoalUserID });

        }
        public async Task JoinGroup(int GoalUserID, int GoalGroupID)
        {
            string sql = $@"
                INSERT INTO GroupMembers (GroupID, UserID, JoinDate)
                VALUES (@GroupId, @UserId, CURRENT_TIMESTAMP);";

            await _db.SaveData(sql, new { UserId = GoalUserID, GroupId = GoalGroupID });
        }
        public async Task RemoveGroupMember(int GoalUserID, int GoalGroupID)
        {
            string sql = @"
                DELETE FROM GroupMembers
                WHERE UserID = @UserId AND GroupID = @GroupId;";

            await _db.SaveData(sql, new { UserId = GoalUserID, GroupId = GoalGroupID });
        }

    }
}
