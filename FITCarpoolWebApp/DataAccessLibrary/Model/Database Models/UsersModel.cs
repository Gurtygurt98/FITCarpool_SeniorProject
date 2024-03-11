using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class UsersModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; }
        public string UserLocation { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public int DrivingDistance { get; set; }


        public UsersModel() { }

        public UsersModel(int userId, string email, string firstName, string lastName, string phone, string userType, string userLocation, string pickupLocation, string dropoffLocation, int drivingDistance)
        {
            UserId = userId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            UserType = userType;
            UserLocation = userLocation;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            DrivingDistance = drivingDistance;
        }

        public UsersModel(string email, string firstName, string lastName, string phone, string userType, string userLocation, string pickupLocation, string dropoffLocation, int drivingDistance)
        {

            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            UserType = userType;
            UserLocation = userLocation;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            DrivingDistance = drivingDistance;
        }
        public override string ToString()
        {
            return $"Email: {Email}, FirstName: {FirstName}, LastName: {LastName}, Phone: {Phone}, UserType: {UserType}, UserLocation: {UserLocation}, PickupLocation: {PickupLocation}, DropoffLocation: {DropoffLocation}, DrivingDistance: {DrivingDistance} miles";
        }
    }

}
