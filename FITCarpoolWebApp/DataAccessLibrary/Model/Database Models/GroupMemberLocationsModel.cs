 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class GroupMemberLocationsModel
    {
        public int LocationId { get; set; }
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }

        public GroupMemberLocationsModel() { }

        public GroupMemberLocationsModel(int locationId, int userId, int groupId, double latitude, double longitude, DateTime timestamp)
        {
            LocationId = locationId;
            UserID = userId;
            GroupID = groupId;
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = timestamp;
        }
        public GroupMemberLocationsModel(int userId, int groupId, double latitude, double longitude, DateTime timestamp)
        {
            UserID = userId;
            GroupID = groupId;
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = timestamp;
        }
        public override string ToString()
        {
            return $"LocationId: {LocationId}, UserId: {UserID}, GroupId: {GroupID}, Latitude: {Latitude}, Longitude: {Longitude}, Timestamp: {Timestamp}";
        }
    }

}
