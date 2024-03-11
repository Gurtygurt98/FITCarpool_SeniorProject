using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class CarpoolGroupsModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int DriverId { get; set; }
        public string Destination { get; set; }
        public string MeetingPoint { get; set; }

        public CarpoolGroupsModel() { }

        public CarpoolGroupsModel(int groupId, string groupName, int driverId, string destination, string meetingPoint)
        {
            GroupId = groupId;
            GroupName = groupName;
            DriverId = driverId;
            Destination = destination;
            MeetingPoint = meetingPoint;
        }
    }

}
