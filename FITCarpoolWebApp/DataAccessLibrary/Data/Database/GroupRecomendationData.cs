using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class GroupRecomendationData : IGroupRecommendationData
    {
        private readonly ISQLDataAccess _db;
        private readonly IUsersData _dbUsers;
        public GroupRecomendationData(ISQLDataAccess db, IUsersData usersData)
        {
            _db = db;
            _dbUsers = usersData;
        }
        public async Task<List<RecomendedGroup>> GetRecommendedGroupsForTimePeriod(Week week)
        {
            // SQL query to select groups within the specified time period
            string sql = @"
                        SELECT 
                            GR.GroupID,
                            GR.GroupName,
                            GR.Start AS StartWindow,
                            GR.End AS EndWindow,
                            GR.Direction,
                            GR.RecurringPattern,
                            GR.IsRecurring,
                            GR.ActiveTimeSlots AS ActiveTimeSlotsSerialized
                        FROM GroupRecomendation GR
                        WHERE Start <= @EndDate AND End >= @StartDate";

            var parameters = new { StartDate = week.StartDate, EndDate = week.EndDate };

            // Load groups from the database
            var groups = await _db.LoadData<RecomendedGroup, dynamic>(sql, parameters);

            if (!groups.Any())
            {
                return new List<RecomendedGroup>();
            }

            // Deserialize ActiveTimeSlots and collect GroupIDs
            foreach (var group in groups)
            {
                if (!string.IsNullOrEmpty(group.ActiveTimeSlotsSerialized))
                {
                    group.ActiveTimeSlots = group.ActiveTimeSlotsSerialized.Split(',')
                        .Select(s => DateTime.Parse(s))
                        .ToList();
                }
            }
            return await GetGroupMembersForList(groups);

        }
        public async Task<List<RecomendedGroup>> GetGroupMembersForList(List<RecomendedGroup> memberLessGroups)
        {

            for (int i = 0; i < memberLessGroups.Count; i++)
            {
                memberLessGroups[i] = await GetGroupMembers(memberLessGroups[i]);
            }
            return memberLessGroups;
        }
        public async Task<RecomendedGroup> GetGroupMembers(RecomendedGroup memberLessGroup)
        {
            // SQL query to get all group members for the collected GroupIDs
            string sqlMembers = @"
                        SELECT  UserID
                        FROM GroupRecomendationMembership
                        WHERE GroupID = @groupID";
            List<int> memberIDs = await _db.LoadData<int, dynamic>(sqlMembers, new { groupID = memberLessGroup.GroupID });
            memberLessGroup.GroupMembers = await _dbUsers.GetListUserInfoModel(memberIDs);
            return memberLessGroup;
        }
        public async Task InsertRecommendedGroups(RecomendedGroup recomendedGroup)
        {
            string sql = @"
                            INSERT INTO GroupRecomendation 
                            (GroupName, CreateDate, Start, End, Direction, RecurringPattern, IsRecurring, ActiveTimeSlots) 
                            VALUES 
                            (@GroupName, CURRENT_TIMESTAMP, @StartWindow, @EndWindow, @Direction, @RecurringPattern, @IsRecurring, @ActiveTimeSlots)";

            // Serialize ActiveTimeSlots to a string (e.g., JSON) for storage
            string activeTimeSlotsSerialized = string.Join(",", recomendedGroup.ActiveTimeSlots.Select(ts => ts.ToString("o")));
            var parameters = new
            {
                GroupName = recomendedGroup.GroupName,
                StartWindow = recomendedGroup.StartWindow.ToString("o"), // ISO 8601 format for consistency
                EndWindow = recomendedGroup.EndWindow.ToString("o"),
                Direction = recomendedGroup.Direction,
                RecurringPattern = recomendedGroup.RecurringPattern,
                IsRecurring = recomendedGroup.IsRecurring.Equals("Yes") ? "Yes" : "No",
                ActiveTimeSlots = activeTimeSlotsSerialized
            };
            int GroupID = await _db.SaveDataAndGetLastId(sql, parameters);
            foreach (var i in recomendedGroup.GroupMembers)
            {
                string sqlGroupMem = @"
                            INSERT INTO GroupRecomendationMembership 
                            (GroupID, UserID) 
                            VALUES 
                            (@Groupid, @Userid)";
                await _db.SaveData(sqlGroupMem, new { Userid = i.UserID, Groupid = GroupID });
            }

        }
        public async Task InsertRecommendedGroups(List<RecomendedGroup> recomendedGroups)
        {
            foreach (var i in recomendedGroups)
            {
                await InsertRecommendedGroups(i);
            }
        }
        public async Task<List<RecomendedGroup>> GetUsersUpcomingRecommendations(int userID)
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7);
            var endDate = startDate.AddDays(4); // Friday after Monday
            string sql = @"SELECT DISTINCT
                                GR.GroupID,
                                GR.GroupName,
                                GR.Start AS StartWindow,
                                GR.End AS EndWindow,
                                GR.Direction,
                                GR.RecurringPattern,
                                GR.IsRecurring,
                                GR.ActiveTimeSlots AS ActiveTimeSlotsSerialized,
	                            GRm.UserID
                            FROM GroupRecomendation GR
                                LEFT JOIN GroupRecomendationMembership GRM ON GR.GroupID = GRM.GroupID
                                LEFT JOIN MapRecommendToReal MRR ON GR.GroupID = MRR.RecommendGroupID
                                LEFT JOIN CarpoolGroupMemberTable CGMT ON MRR.CreatedGroupID = CGMT.GroupID
                            WHERE GRM.UserID = @Userid AND Start <= @EndDate   AND NOT EXISTS ( SELECT 1
                                                                                                FROM CarpoolGroupMemberTable CGMT_Sub
                                                                                                    JOIN MapRecommendToReal MRR_Sub ON CGMT_Sub.GroupID = MRR_Sub.CreatedGroupID
                                                                                                WHERE MRR_Sub.RecommendGroupID = GR.GroupID AND CGMT_Sub.UserID = GRM.UserID);";
            // add back if needed to show only the next week and not before  AND End >= @StartDate;
            var parameters = new { StartDate = startDate, EndDate = endDate, Userid = userID };
            var groups = await _db.LoadData<RecomendedGroup, dynamic>(sql, parameters);
            // Deserialize ActiveTimeSlots and collect GroupIDs
            foreach (var group in groups)
            {
                if (!string.IsNullOrEmpty(group.ActiveTimeSlotsSerialized))
                {
                    group.ActiveTimeSlots = group.ActiveTimeSlotsSerialized.Split(',')
                        .Select(s => DateTime.Parse(s))
                        .ToList();
                }
            }
            groups = await GetGroupMembersForList(groups);
            return groups;

        }
        public async Task AcceptGroupRec(RecomendedGroup group, int UserID)
        {
            string sql = @"select CreatedGroupID from MapRecommendToReal where RecommendGroupID = @RecGroupID"; 
            var groupIDs = await _db.LoadData<int, dynamic>(sql, new {RecGroupID = group.GroupID});
            if(groupIDs == null || groupIDs.Count() == 0)
            {
                // Group doesn't exist yet 
                await CreateRealGroupFromRec(group, UserID);
            }
            else
            {
                // Group Already Exists just add user to the trips and to the group 
                int GroupID = groupIDs.FirstOrDefault();
                await AddMemberToGroup(UserID, GroupID);
                await AddMemberAsPending(UserID, GroupID);
            }


        }
        public async Task CreateRealGroupFromRec(RecomendedGroup group, int UserID)
        {
            // Insert group into the groups table 
            string sql = @"INSERT INTO CarpoolGroupTable (Name, Direction, CreateDate) Values (@name, @direction, CURRENT_TIMESTAMP)";
            var parameters = new { name = group.GroupName, direction = group.Direction };
            int GroupId = await _db.SaveDataAndGetLastId(sql, parameters);
            // Add Group mem that intiaed to that group 
            await AddMemberToGroup(UserID, GroupId);
            // Create Trips for each time slots  
            foreach (DateTime TripStart in group.ActiveTimeSlots)
            {
                string TripSQL = @"INSERT INTO CarpoolTrips (GroupID, Start, End) Values (@GroupId, @TripStart, @TripEnd)";
                var TripParameters = new { GroupId, TripStart, TripEnd = TripStart.AddHours(1) };
                int TripID = await _db.SaveDataAndGetLastId(TripSQL, TripParameters);
            }
            // Add Group Creator as pending invite 
            await AddMemberAsPending(UserID, GroupId);
            string sqlInsertMapping = @"INSERT INTO MapRecommendToReal VALUES (@RecGroupID, @NewGroupID);";
            await _db.SaveData(sqlInsertMapping, new {RecGroupID = group.GroupID, NewGroupID = GroupId});
        }
        public async Task AddMemberToGroup(int UserID, int GroupID)
        {
            string sql = @"INSERT INTO CarpoolGroupMemberTable (GroupID, UserID) Values (@GroupID, @UserID)";
            var parameters = new { GroupID, UserID};
            await _db.SaveData(sql, parameters);
        }
        // Adds the member as pending for each trip 
        public async Task AddMemberAsPending(int UserID, int GroupID)
        {
            string sqlGetTripIds = @"select ID from CarpoolTrips where GroupID = @GroupID";
            List<int> TripIDs = await _db.LoadData<int, dynamic>(sqlGetTripIds, new { GroupID });
            foreach (int TripID in TripIDs)
            {
                string sqlInsert = "INSERT INTO CarpoolTripMembers (TripID, UserID, Status) Values (@TripID, @UserID, @Status)";
                var para = new { TripID, UserID, Status = "Pending" };
                await _db.SaveData(sqlInsert, para);
            }
        }
        public async Task DeclineGroupRec(RecomendedGroup group, int UserID)
        {

        }
    }
}
