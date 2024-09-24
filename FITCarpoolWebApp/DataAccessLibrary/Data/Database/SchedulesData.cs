using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccessLibrary.Data.Database
{
    public class SchedulesData : ISchedulesData
    {
        private readonly ISQLDataAccess _db;

        public SchedulesData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<SchedulesModel>> GetAllSchedules()
        {
            string sql = "SELECT * FROM Schedules";
            return await _db.LoadData<SchedulesModel, dynamic>(sql, new { });
        }

        public async Task<List<SchedulesModel>> GetSchedule(int userID)
        {
            string sql = "SELECT * FROM Schedules WHERE UserId = @UserId";
            return await _db.LoadData<SchedulesModel, dynamic>(sql, new { UserId = userID });
        }

        public async Task UpdateSchedule(SchedulesModel schedule)
        {
            string sql = @"UPDATE Schedules SET UserID = @UserId, Day = @Day, Start = @Start, End = @End, Text = @Text WHERE ScheduleID = @ScheduleId";
            await _db.SaveData(sql, schedule);
        }
        public async Task DeleteSchedule(int scheduleId)
        {
            string sql = "DELETE FROM Schedules WHERE ScheduleID = @ScheduleId";
            await _db.SaveData(sql, new { ScheduleId = scheduleId });
        }
        public async Task AddSchedule(SchedulesModel schedule)
        {
            string sql = @"INSERT INTO Schedules (UserID, Day, Start, End, Text) 
                   VALUES (@UserId, @Day, @Start, @End, @Text)";
            await _db.SaveData(sql, schedule);
        }
        public async Task<List<RecomendedGroup>> GetMatchingSchedules(int GoalUserID,List<string> Days)
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
                    JOIN CarpoolGroups CG on s2.GroupID = CG.GroupID
                    WHERE s1.UserID = @UserId
                    AND s1.Day IN @Days;";

            List<(int,string, string)> MatchingScheduleGroupIDs = await _db.LoadData<(int,string,string), dynamic>(sql, new { UserId = GoalUserID, Days });

            // Get the user info for the goal model 
            UserInfoModel GoalUser = await GetUserInfoModel(GoalUserID);
            List<RecomendedGroup> RecommendedGroups = new List<RecomendedGroup>();
            foreach ((int GroupID,string GroupName, string direction) in MatchingScheduleGroupIDs)
            {
                List<int> GroupMemberIDs = await GetUserIds(GroupID);
                List<UserInfoModel> GroupMembers = new List<UserInfoModel>();
                foreach (int MemberID in GroupMemberIDs) 
                {
                    GroupMembers.Add(await GetUserInfoModel(MemberID));
                }
                RecommendedGroups.Add(new RecomendedGroup(GroupName, GroupID, GroupMembers, GoalUser, direction));
            }
            return RecommendedGroups;
        }
        public async Task<UserInfoModel> GetUserInfoModel(int GoalUserID)
        {
            string sql = $@"SELECT 
                u.UserID, 
                           u.FirstName, 
                           u.LastName, 
                           u.UserType, 
                           u.PickupLocation, 
                           u.DropoffLocation, 
                           u.DrivingDistance, 
                           u.Gender, 
                           u.BeltCount, 
                           u.AllowEatDrink, 
                           u.AllowSmokeVape, 
                           p.GenderPreference, 
                           p.EatingPreference, 
                           p.SmokingPreference, 
                           p.TemperaturePreference, 
                           p.MusicPreference,
                           l.PickupLatitude,
                           l.PickupLongitude, 
                           l.DropoffLongitude, 
                           l.DropoffLatitude, 
                           u.PhonePrivacy, 
                           u.AddressPrivacy, 
                           u.MakeModel, 
                           u.VehicleColor, 
                           u.LicensePlate, 
                           u.LicensePicture, 
                           u.CarPicture,
                           u.ProfilePicture,
                           u.Rating
                        FROM Users u
                        JOIN Locations l ON u.UserID = l.UserID
                        JOIN Preferences p ON u.UserID = p.UserID
                        WHERE u.UserID = @UserId;";
            List<UserInfoModel> FoundUsers = await _db.LoadData<UserInfoModel, dynamic>(sql, new { UserId = GoalUserID });
            if (!FoundUsers.Any())
            {
                Console.WriteLine("Goal user not found");
                return new UserInfoModel();

            }
            return FoundUsers.FirstOrDefault();

        }
        public async Task<List<int>> GetUserIds(int GroupID)
        {
            string sql = @"select UserID from GroupMembers where GroupID = @groupID";
            return await _db.LoadData<int, dynamic>(sql, new { groupID = GroupID });

        }
        // Get the current groups a user is apart of 
        public async Task<List<(int,string)>> GetCurrentGroups(int GoalUserID)
        {
            // Query to get all the matching groups 
            string sql = $@"
                    SELECT GM.GroupID, CG.GroupName
                    FROM GroupMembers GM
                    JOIN CarpoolGroups CG on GM.GroupID = CG.GroupID
                    WHERE GM.UserID = @UserId;";

            return await _db.LoadData<(int, string), dynamic>(sql, new { UserId = GoalUserID });
            
        }
        public async Task JoinGroup(int GoalUserID, int GoalGroupID)
        {
            string sql = $@"
                INSERT INTO GroupMembers (GroupID, UserID, JoinDate)
                VALUES (@GroupId, @UserId, CURRENT_TIMESTAMP);";

            await _db.SaveData(sql, new { UserId = GoalUserID, GroupId = GoalGroupID });
        }

    }
}