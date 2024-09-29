﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLibrary.Model.Logic_Models
{
    public class RecomendedGroup
    {
        public string GroupName { get; set; } = string.Empty;
        public int GroupID { get; set; }
        public List<UserInfoModel> GroupMembers { get; set; }
        public UserInfoModel RequestingUser { get; set; }
        public string Direction { get; set; } = "";
        public double DistanceScore { get; private set; }
        public double PreferenceScore { get; private set; }

        public RecomendedGroup() { }

        public RecomendedGroup(string groupName, int groupID, List<UserInfoModel> groupMembers, UserInfoModel requestUser, string direction)
        {
            GroupName = groupName;
            GroupID = groupID;
            GroupMembers = new List<UserInfoModel>(groupMembers);
            RequestingUser = requestUser;
            Direction = direction;

            DistanceScore = CalculateDistanceScore();
            Console.WriteLine("DistanceScore " + GroupName);

            PreferenceScore = CalculateTotalMatchScore();
            Console.WriteLine("PreferenceScore " + PreferenceScore);

        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
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

        private double CalculateDistanceScore()
        {
            double totalDistance = 0;
            int memberCount = 0;

            foreach (var member in GroupMembers)
            {
                if (member.UserID == RequestingUser.UserID)
                    continue;

                double distance = 0;
                if (Direction == "Arriving")
                {
                    distance = CalculateDistance(RequestingUser.DropoffLatitude, RequestingUser.DropoffLongitude, member.DropoffLatitude, member.DropoffLongitude);
                }
                else if (Direction == "Departing")
                {
                    distance = CalculateDistance(RequestingUser.PickupLatitude, RequestingUser.PickupLongitude, member.PickupLatitude, member.PickupLongitude);
                }
                totalDistance += distance;
                memberCount++;
            }

            if (memberCount == 0)
                return 0;

            return totalDistance / memberCount;
        }

        public double CalculatePreferenceMatch(UserInfoModel user1, UserInfoModel user2)
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

        public double CalculateTotalMatchScore()
        {
            double totalScore = 0;
            int memberCount = 0;

            foreach (var member in GroupMembers)
            {
                if (member.UserID == RequestingUser.UserID)
                    continue;

                totalScore += CalculatePreferenceMatch(RequestingUser, member);
                memberCount++;
            }

            if (memberCount == 0)
                return 0;

            return totalScore / memberCount; 
        }

        public override string ToString()
        {
            return $"Group: {GroupName}, ID: {GroupID}, Members: {GroupMembers.Count}, Distance Score: {DistanceScore}, Preference Score: {PreferenceScore}";
        }
    }
}
