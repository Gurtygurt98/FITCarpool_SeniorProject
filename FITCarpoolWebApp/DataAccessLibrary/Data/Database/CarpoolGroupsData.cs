using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        // This function will use k clustering and Hierarchical agglomerative clustering (HAC) to form groups of users 
        public async Task<List<RecomendedGroup>> GetRecommendGroups(int GoalUserID, List<string> Days, string TravelDirection)
        {
            if(!TravelDirection.Equals("arrival") && !TravelDirection.Equals("departure"))
            {
                Console.WriteLine("Error - Invalid Direction " + TravelDirection); 
                return null;
            }
            // Build the user info model for the goal user
            UserInfoModel GoalUserModel = await _dbUsers.GetUserInfoModel(GoalUserID);
            // Get a list of User Info Model, this repersents all the user that the Goal User can be in a group with 
            List<UserInfoModel> MatchingUserList = await _dbSchedule.GetMatchingSchedules(GoalUserID, Days, TravelDirection);
            // Cluster these users by their longitude and latitude using grid quadrants 
            List<List<UserInfoModel>> ClusterUserList = ClusterUsersbyLocation(MatchingUserList, GoalUserModel);
            List<RecomendedGroup> GroupList = new List<RecomendedGroup>();
            foreach(List < UserInfoModel > UserCluster in ClusterUserList)
            {
                Console.WriteLine("Processing Cluster of size " + UserCluster.Count);
                // Build groups from the cluster using HAC
                List<RecomendedGroup> GroupsFromCluster = HACGroupBuilder(UserCluster, GoalUserModel, TravelDirection, 4);
                Console.WriteLine(GroupsFromCluster.Count + " groups created from HAC");
                GroupList.AddRange(GroupsFromCluster);
            }

            Console.WriteLine($"The Recommendation Algorithm Found {GroupList.Count()} group(s) for you!");
            return GroupList;
        }

        // Clusters users by their location using a grid, need 
        public List<List<UserInfoModel>> ClusterUsersbyLocation(List<UserInfoModel> UserList, UserInfoModel GoalUserModel)
        {
            // Initialize the clusters (one for each quadrant)
            List<List<UserInfoModel>> clusters = new List<List<UserInfoModel>>()
            {
                new List<UserInfoModel>(),
                new List<UserInfoModel>(), 
                new List<UserInfoModel>(),
                new List<UserInfoModel>()  
            };

            // Center of the grid 
            double centerLat = GoalUserModel.PickupLatitude;
            double centerLon = GoalUserModel.PickupLongitude;

            // Iterate over all users and place them into quadrants
            foreach (var user in UserList)
            {
                double userLat = user.PickupLatitude;
                double userLon = user.PickupLongitude;

                // Determine the quadrant based on the relative position to the center
                if (userLat >= centerLat && userLon >= centerLon)
                {
                    clusters[0].Add(user);
                }
                else if (userLat >= centerLat && userLon < centerLon)
                {
                    clusters[1].Add(user);
                }
                else if (userLat < centerLat && userLon < centerLon)
                {
                    clusters[2].Add(user);
                }
                else if (userLat < centerLat && userLon >= centerLon)
                {
                    clusters[3].Add(user);
                }
            }

            return clusters;
        }

        // Haversine formula to calculate the distance between two lat/lon points in meters.
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Earth's radius in meters
            double R = 6371e3; 
            double lat1Rad = lat1 * Math.PI / 180;
            double lat2Rad = lat2 * Math.PI / 180;
            double deltaLat = (lat2 - lat1) * Math.PI / 180;
            double deltaLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = R * c;
            return distance;
        }

        public List<RecomendedGroup> HACGroupBuilder(List<UserInfoModel> UserCluster, UserInfoModel GoalUserModel, string direction, int n)
        {
            // Step 1: Remove duplicate users based on UserId
            List<UserInfoModel> distinctUsers = UserCluster.Distinct().ToList();

            // Step 2: Shuffle the users to ensure random distribution into groups
            Random rand = new Random();
            distinctUsers = distinctUsers.OrderBy(user => rand.Next()).ToList();

            // Step 3: Initialize the list to hold the recommended groups
            List<RecomendedGroup> recommendedGroups = new List<RecomendedGroup>();

            // Step 4: Assign users to groups of size 'n'
            for (int i = 0; i < distinctUsers.Count; i += n)
            {
                // Get the next 'n' users to form a group
                List<UserInfoModel> currentGroup = distinctUsers.Skip(i).Take(n).ToList();

                // Create a RecomendedGroup instance for the current group
                RecomendedGroup group = new RecomendedGroup(
                    groupName: string.Join(" ", currentGroup.Select(user => user.FirstName)),
                    groupID: -1,
                    groupMembers: currentGroup,
                    requestUser: GoalUserModel,
                    direction: direction
                );

                recommendedGroups.Add(group);
            }

            return recommendedGroups;
        }



        // Computes the distance between two clusters using average linkage.
        private double ComputeClusterDistance(List<UserInfoModel> clusterA, List<UserInfoModel> clusterB)
        {
            // Use average linkage: average distance between all pairs of users in the two clusters
            double totalDistance = 0;
            int count = 0;

            foreach (var userA in clusterA)
            {
                foreach (var userB in clusterB)
                {
                    double distance = ComputeUserDistance(userA, userB);
                    totalDistance += distance;
                    count++;
                }
            }

            if (count == 0)
                return double.MaxValue;

            return totalDistance / count;
        }

        //Computes the distance between two users based on their preference match.
        private double ComputeUserDistance(UserInfoModel userA, UserInfoModel userB)
        {
            // Calculate preference match score
            double matchScore = CalculatePreferenceMatch(userA, userB);
            // Convert match score to distance (0 = perfect match, 1 = no match)
            double distance = 1.0 - (matchScore / 100.0);
            return distance;
        }

        //Calculates the preference match score between two users.
        private double CalculatePreferenceMatch(UserInfoModel user1, UserInfoModel user2)
        {
            int matchScore = 0;
            int totalPreferences = 5;

            if (user1.GenderPreference == user2.Gender || user1.GenderPreference == "No Preference")
                matchScore++;
            if (user1.SmokingPreference == user2.AllowSmokeVape || user1.SmokingPreference == "No Preference")
                matchScore++;
            if (user1.EatingPreference == user2.AllowEatDrink || user1.EatingPreference == "No Preference")
                matchScore++;
            if (user1.TemperaturePreference == user2.TemperaturePreference || user1.TemperaturePreference == "No Preference")
                matchScore++;
            if (user1.MusicPreference == user2.MusicPreference || user1.MusicPreference == "No Preference")
                matchScore++;

            return (double)matchScore / totalPreferences * 100;
        }
    }
}
