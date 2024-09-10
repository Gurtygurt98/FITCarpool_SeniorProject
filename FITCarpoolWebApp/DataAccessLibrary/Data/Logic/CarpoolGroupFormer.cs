using DataAccessLibrary.Data.Database;
using DataAccessLibrary.Model.Logic_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Logic
{
    public class CarpoolGroupFormer
    {
        private readonly ISchedulesData _dbSchedule;

        public CarpoolGroupFormer(ISchedulesData dbSchedule)
        {
            _dbSchedule = dbSchedule;
        }

        // Method to calculate the distance between two users using their latitude and longitude
        private double CalculateDistance(UserInfoModel user1, UserInfoModel user2)
        {
            var lat1 = user1.Latitude;
            var lon1 = user1.Longitude;
            var lat2 = user2.Latitude;
            var lon2 = user2.Longitude;

            var R = 6371; // Radius of the Earth in km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c; // Distance in km
            return distance;
        }

        // Main method to get and form the carpool groups based on the criteria
        public async Task<List<List<UserInfoModel>>> GetGroups(int GoalUserID, bool IsArriving, bool IsDeparting, List<string> Days)
        {
            // Retrieve users matching the goal user's schedule
            List<UserInfoModel> remainingUsers = await _dbSchedule.GetMatchingSchedules(GoalUserID, IsArriving, IsDeparting, Days);
            if (!remainingUsers.Any())
            {
                Console.WriteLine("No groups found - no matching schedules");
                return null;
            }

            // Retrieve the goal user
            UserInfoModel goalUser = await _dbSchedule.GetGoalUser(GoalUserID);

            int MAX_GROUP_SIZE = 4; // need to find a way to calculate this 

            // List to store the final groups of users
            List<List<UserInfoModel>> finalGroups = new List<List<UserInfoModel>>();

            // Cluster the users based on proximity to the goal user
            while (remainingUsers.Any())
            {
                // Start a new group with the goal user as the anchor point
                List<UserInfoModel> currentGroup = new List<UserInfoModel> { goalUser };

                // Find the closest users to the goal user
                while (currentGroup.Count < MAX_GROUP_SIZE && remainingUsers.Any())
                {
                    UserInfoModel closestUser = null;
                    double minDistance = double.MaxValue;

                    foreach (var user in remainingUsers)
                    {
                        // Calculate distance between current group center (goal user) and other users
                        double distance = CalculateDistance(goalUser, user);
                        if (distance < minDistance)
                        {
                            closestUser = user;
                            minDistance = distance;
                        }
                    }

                    if (closestUser != null)
                    {
                        // Add closest user to the group and remove from remaining users
                        currentGroup.Add(closestUser);
                        remainingUsers.Remove(closestUser);
                    }
                }

                // Add the group to the final groups list
                finalGroups.Add(currentGroup);

                // Optionally, update the goal user to be the center of the next group
                if (remainingUsers.Any())
                {
                    goalUser = remainingUsers[0];
                    remainingUsers.RemoveAt(0);
                }
            }

            // Sort the groups based on proximity to the original goal user (optional step)
            finalGroups = finalGroups.OrderBy(group => CalculateGroupDistanceToGoal(group, goalUser)).ToList();

            return finalGroups;
        }

        // Method to calculate the average distance of a group to the goal user
        private double CalculateGroupDistanceToGoal(List<UserInfoModel> group, UserInfoModel goalUser)
        {
            double totalDistance = 0;
            foreach (var user in group)
            {
                totalDistance += CalculateDistance(goalUser, user);
            }
            return totalDistance / group.Count;
        }
    }
}
