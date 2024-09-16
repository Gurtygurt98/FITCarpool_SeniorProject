using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLibrary.Model.Logic_Models
{
    public class RecomendedGroup
    {
        public string GroupName { get; set; } = String.Empty;
        public int GroupID { get; set; }
        public List<UserInfoModel> GroupMembers { get; set; }
        public UserInfoModel RequestingUser { get; set; }
        public string Direction { get; set; }
        public double DistanceScore { get; private set; }
        public int PreferenceScore { get; private set; }

        public RecomendedGroup() { }

        public RecomendedGroup(string groupName, int groupID, List<UserInfoModel> groupMembers, UserInfoModel requestUser, string direction)
        {
            GroupName = groupName;
            GroupID = groupID;
            GroupMembers = new List<UserInfoModel>(groupMembers);
            RequestingUser = requestUser;
            Direction = direction;

            // Calculate and assign DistanceScore and PreferenceScore
            DistanceScore = CalculateDistanceScore();
            PreferenceScore = CalculateTotalMatchScore();
        }

        // Haversine formula to calculate distance between two latitude/longitude points
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371e3; // Radius of the Earth in meters
            double lat1Rad = lat1 * Math.PI / 180;
            double lat2Rad = lat2 * Math.PI / 180;
            double deltaLat = (lat2 - lat1) * Math.PI / 180;
            double deltaLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                        Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                        Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = R * c; // Distance in meters
            return distance;
        }

        // Calculate the total distance between the requesting user and all group members
        private double CalculateDistanceScore()
        {
            double totalDistance = 0;

            foreach (var member in GroupMembers)
            {
                if (Direction == "Arriving")
                {
                    // Calculate distance based on dropoff location
                    totalDistance += CalculateDistance(RequestingUser.DropoffLatitude, RequestingUser.DropoffLongitude, member.DropoffLatitude, member.DropoffLongitude);
                }
                else if (Direction == "Departing")
                {
                    // Calculate distance based on pickup location
                    totalDistance += CalculateDistance(RequestingUser.PickupLatitude, RequestingUser.PickupLongitude, member.PickupLatitude, member.PickupLongitude);
                }
            }

            // You can normalize the distance score if needed, or return it as total distance
            return totalDistance / GroupMembers.Count;
        }

        // Preference Matching Algorithm: Returns a score based on preference similarity
        public int CalculatePreferenceMatch(UserInfoModel user1, UserInfoModel user2)
        {
            int matchScore = 0;

            // Compare gender preferences
            if (user1.GenderPreference == user2.Gender || user1.GenderPreference == "No Preference")
                matchScore++;

            // Compare smoking preferences
            if (user1.SmokingPreference == user2.AllowSmokeVape || user1.SmokingPreference == "No Preference")
                matchScore++;

            // Compare eating preferences
            if (user1.EatingPreference == user2.AllowEatDrink || user1.EatingPreference == "No Preference")
                matchScore++;

            // Compare temperature preferences
            if (user1.TemperaturePreference == user2.TemperaturePreference || user1.TemperaturePreference == "No Preference")
                matchScore++;

            // Compare music preferences
            if (user1.MusicPreference == user2.MusicPreference || user1.MusicPreference == "No Preference")
                matchScore++;

            return matchScore;
        }

        // Calculate the total preference match score for the entire group
        public int CalculateTotalMatchScore()
        {
            int totalScore = 0;

            foreach (var member in GroupMembers)
            {
                totalScore += CalculatePreferenceMatch(RequestingUser, member);
            }

            return totalScore;
        }

        public override string ToString()
        {
            return $"Group: {GroupName}, ID: {GroupID}, Members: {GroupMembers.Count}, Distance Score: {DistanceScore}, Preference Score: {PreferenceScore}";
        }
    }
}
