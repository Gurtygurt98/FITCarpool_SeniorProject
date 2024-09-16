using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Logic_Models
{
    public class UserInfoModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public string DrivingDistance { get; set; }
        public string Gender { get; set; }
        public int BeltCount { get; set; }
        public string AllowEatDrink { get; set; }
        public string AllowSmokeVape { get; set; }
        public string GenderPreference { get; set; }
        public string EatingPreference { get; set; }
        public string SmokingPreference { get; set; }
        public string TemperaturePreference { get; set; }
        public string MusicPreference { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public double DropoffLatitude { get; set; }
        public double DropoffLongitude { get; set; }

        // Updated constructor with latitude and longitude parameters
        public UserInfoModel(int userID, string firstName, string lastName, string userType, string pickupLocation, string dropoffLocation, string drivingDistance, string gender, int beltCount, string allowEatDrink, string allowSmokeVape, string genderPreference, string eatingPreference, string smokingPreference, string temperaturePreference, string musicPreference, double pickupLatitude, double pickupLongitude, double dropoffLatitude, double dropoffLongitude)
        {
            UserID = userID;
            FirstName = firstName;
            LastName = lastName;
            UserType = userType;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            DrivingDistance = drivingDistance;
            Gender = gender;
            BeltCount = beltCount;
            AllowEatDrink = allowEatDrink;
            AllowSmokeVape = allowSmokeVape;
            GenderPreference = genderPreference;
            EatingPreference = eatingPreference;
            SmokingPreference = smokingPreference;
            TemperaturePreference = temperaturePreference;
            MusicPreference = musicPreference;
            PickupLatitude = pickupLatitude;
            PickupLongitude = pickupLongitude;
            DropoffLatitude = dropoffLatitude;
            DropoffLongitude = dropoffLongitude;
        }

        // Optional: Override ToString for easy display of user info
        public override string ToString()
        {
            return $"UserID: {UserID}, Name: {FirstName} {LastName}, UserType: {UserType}, Pickup: {PickupLocation} (Lat: {PickupLatitude}, Lon: {PickupLongitude}), Dropoff: {DropoffLocation} (Lat: {DropoffLatitude}, Lon: {DropoffLongitude}), DrivingDistance: {DrivingDistance}, Gender: {Gender}, BeltCount: {BeltCount}, AllowEatDrink: {AllowEatDrink}, AllowSmokeVape: {AllowSmokeVape}, Preferences: [Gender: {GenderPreference}, Eating: {EatingPreference}, Smoking: {SmokingPreference}, Temperature: {TemperaturePreference}, Music: {MusicPreference}]";
        }
    }
}
