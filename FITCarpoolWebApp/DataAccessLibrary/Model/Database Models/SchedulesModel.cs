using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class SchedulesModel
    {
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public string Day { get; set; } 
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Text { get; set; }

        public SchedulesModel() { }

        public SchedulesModel(int scheduleId, int userId, string day, DateTime startTime, DateTime endTime)
        {
            ScheduleId = scheduleId;
            UserId = userId;
            Day = day;
            Start = startTime;
            End = endTime;
        }
    }

}
