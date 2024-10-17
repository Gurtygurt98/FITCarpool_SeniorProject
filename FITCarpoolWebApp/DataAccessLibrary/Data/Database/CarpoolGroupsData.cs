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
        /// <summary>
        /// This function uses k clustering and Hierarchical agglomerative clustering (HAC) to form groups of users 
        /// </summary>
        /// <param name="GoalUserID"></param>
        /// This is the user who is looking for groups to join
        /// <param name="Days"></param>
        /// days of the week Monday, Tuesday, Wednesday ETC 
        /// <param name="direction"></param>
        /// Value is either arrival or departure 
        /// <returns></returns>
        /// A List of generated groups that best match the user 
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
            // Cluster these users by their longitude and latitude using K clustering 
            List<List<UserInfoModel>> ClusterUserList = ClusterUsersbyLocation(MatchingUserList, GoalUserModel, TravelDirection);
            List<RecomendedGroup> GroupList = new List<RecomendedGroup>();
            foreach(List < UserInfoModel > UserCluster in ClusterUserList)
            {
                // Build groups from the cluster using HAC
                List<RecomendedGroup> GroupsFromCluster = HACGroupBuilder(UserCluster, GoalUserModel, TravelDirection, 4);
                GroupList.AddRange(GroupsFromCluster);
            }

            Console.WriteLine($"The Recommendation Algorithm Found {GroupList.Count()} group(s) for you!");
            return GroupList;
        }

        /// <summary>
        /// Clusters users by their location using K-means clustering algorithm.
        /// </summary>
        /// <param name="UserList">List of users to cluster.</param>
        /// <param name="GoalUserModel">The user requesting recommendations.</param>
        /// <param name="Direction">Travel direction ("arrival" or "departure").</param>
        /// <returns>List of clusters, each cluster is a list of UserInfoModel.</returns>
        public List<List<UserInfoModel>> ClusterUsersbyLocation(List<UserInfoModel> UserList, UserInfoModel GoalUserModel, string Direction)
        {
            // Number of clusters
            int K = 4; // You can adjust K or make it a parameter

            // Maximum number of iterations for K-means algorithm
            int maxIterations = 100;

            // List to hold the clusters
            List<List<UserInfoModel>> clusters = new List<List<UserInfoModel>>();

            // List to hold centroids (latitude, longitude)
            List<(double lat, double lon)> centroids = new List<(double lat, double lon)>();

            // Extract locations of users based on the travel direction
            // For arrival, use Dropoff location; for departure, use Pickup location
            List<(UserInfoModel user, double lat, double lon)> userLocations = new List<(UserInfoModel user, double lat, double lon)>();
            foreach (var user in UserList)
            {
                double lat, lon;
                if (Direction.Equals("arrival", StringComparison.OrdinalIgnoreCase))
                {
                    lat = user.DropoffLatitude;
                    lon = user.DropoffLongitude;
                }
                else // departure
                {
                    lat = user.PickupLatitude;
                    lon = user.PickupLongitude;
                }
                userLocations.Add((user, lat, lon));
            }

            // Initialize centroids randomly from the users' locations
            Random rand = new Random();
            HashSet<int> chosenIndices = new HashSet<int>();
            for (int i = 0; i < K; i++)
            {
                int index;
                do
                {
                    index = rand.Next(userLocations.Count);
                } while (chosenIndices.Contains(index));
                chosenIndices.Add(index);
                centroids.Add((userLocations[index].lat, userLocations[index].lon));
            }

            // Initialize cluster assignments for each user
            // assignments[i] = cluster index assigned to user i
            List<int> assignments = new List<int>(new int[userLocations.Count]);

            // K-means algorithm main loop
            for (int iter = 0; iter < maxIterations; iter++)
            {
                bool changed = false;

                // Assignment step: Assign each user to the nearest centroid
                for (int i = 0; i < userLocations.Count; i++)
                {
                    double minDistance = double.MaxValue;
                    int minIndex = -1;

                    for (int c = 0; c < K; c++)
                    {
                        // Calculate distance between user and centroid
                        double distance = CalculateDistance(userLocations[i].lat, userLocations[i].lon, centroids[c].lat, centroids[c].lon);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minIndex = c;
                        }
                    }

                    // Check if assignment has changed
                    if (assignments[i] != minIndex)
                    {
                        changed = true;
                        assignments[i] = minIndex;
                    }
                }

                // If no assignments have changed, convergence reached
                if (!changed)
                    break;

                // Update step: Recalculate centroids as the mean of assigned users
                for (int c = 0; c < K; c++)
                {
                    var assignedUsers = userLocations.Where((ul, idx) => assignments[idx] == c).ToList();
                    if (assignedUsers.Count > 0)
                    {
                        // Calculate mean latitude and longitude
                        double meanLat = assignedUsers.Average(ul => ul.lat);
                        double meanLon = assignedUsers.Average(ul => ul.lon);
                        centroids[c] = (meanLat, meanLon);
                    }
                    else
                    {
                        // If no users assigned to centroid, reinitialize centroid randomly
                        int index = rand.Next(userLocations.Count);
                        centroids[c] = (userLocations[index].lat, userLocations[index].lon);
                    }
                }
            }

            // Build the clusters based on final assignments
            for (int c = 0; c < K; c++)
            {
                var clusterUsers = userLocations.Where((ul, idx) => assignments[idx] == c).Select(ul => ul.user).ToList();
                clusters.Add(clusterUsers);
            }

            return clusters;
        }

        /// <summary>
        /// Haversine formula to calculate the distance between two lat/lon points in meters.
        /// </summary>
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

        /// <summary>
        /// Builds groups from a cluster of users using Hierarchical Agglomerative Clustering (HAC).
        /// </summary>
        /// <param name="UserCluster">List of users in the cluster.</param>
        /// <param name="GoalUserModel">The user requesting recommendations.</param>
        /// <param name="direction">Travel direction ("arrival" or "departure").</param>
        /// <param name="n">Maximum group size.</param>
        /// <returns>List of recommended groups.</returns>
        public List<RecomendedGroup> HACGroupBuilder(List<UserInfoModel> UserCluster, UserInfoModel GoalUserModel, string direction, int n)
        {
            // Initialize clusters: Each user is its own cluster
            List<List<UserInfoModel>> clusters = new List<List<UserInfoModel>>();
            foreach (var user in UserCluster)
            {
                clusters.Add(new List<UserInfoModel> { user });
            }

            // Flag to check if clusters can be merged
            bool clustersCanMerge = true;

            // Continue merging clusters until no more merges can be done
            while (clustersCanMerge)
            {
                clustersCanMerge = false;
                double minDistance = double.MaxValue;
                int mergeIndexA = -1;
                int mergeIndexB = -1;

                // Find the pair of clusters with the minimum distance
                for (int i = 0; i < clusters.Count; i++)
                {
                    for (int j = i + 1; j < clusters.Count; j++)
                    {
                        // Check if merging these clusters would exceed the maximum group size
                        if (clusters[i].Count + clusters[j].Count > n)
                            continue;

                        // Compute the distance between clusters[i] and clusters[j]
                        double distance = ComputeClusterDistance(clusters[i], clusters[j]);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            mergeIndexA = i;
                            mergeIndexB = j;
                            clustersCanMerge = true;
                        }
                    }
                }

                // If a mergeable pair was found, merge them
                if (clustersCanMerge && mergeIndexA != -1 && mergeIndexB != -1)
                {
                    // Merge clusters[mergeIndexB] into clusters[mergeIndexA] and remove clusters[mergeIndexB]
                    clusters[mergeIndexA].AddRange(clusters[mergeIndexB]);
                    clusters.RemoveAt(mergeIndexB);
                }
            }

            // Create RecomendedGroup instances for each cluster
            List<RecomendedGroup> recommendedGroups = new List<RecomendedGroup>();
            int groupId = 1; // Group ID can be assigned sequentially or fetched from database

            foreach (var cluster in clusters)
            {
                // Ensure the cluster size does not exceed N
                if (cluster.Count > n)
                {
                    // Handle splitting the cluster if necessary
                    // For simplicity, skip clusters that exceed size N
                    continue;
                }

                // Include the GoalUserModel in the group if not already present
                if (!cluster.Any(u => u.UserID == GoalUserModel.UserID))
                {
                    cluster.Add(GoalUserModel);
                }

                // Create a RecomendedGroup instance
                RecomendedGroup group = new RecomendedGroup(
                    groupName: $"Group {groupId}",
                    groupID: groupId,
                    groupMembers: cluster,
                    requestUser: GoalUserModel,
                    direction: direction
                );

                recommendedGroups.Add(group);
                groupId++;
            }

            return recommendedGroups;
        }

        /// <summary>
        /// Computes the distance between two clusters using average linkage.
        /// </summary>
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

        /// <summary>
        /// Computes the distance between two users based on their preference match.
        /// </summary>
        private double ComputeUserDistance(UserInfoModel userA, UserInfoModel userB)
        {
            // Calculate preference match score
            double matchScore = CalculatePreferenceMatch(userA, userB);
            // Convert match score to distance (0 = perfect match, 1 = no match)
            double distance = 1.0 - (matchScore / 100.0);
            return distance;
        }

        /// <summary>
        /// Calculates the preference match score between two users.
        /// </summary>
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
