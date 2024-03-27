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
        public string DayOfWeek { get; set; } 
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Text { get; set; }
        public int EarliestTime { get; set; }
        public int LatestTime { get; set; }

        public SchedulesModel() { }

        public SchedulesModel(int scheduleId, int userId, string dayOfWeek, DateTime startTime, DateTime endTime, int earliestTime, int latestTime)
        {
            ScheduleId = scheduleId;
            UserId = userId;
            DayOfWeek = dayOfWeek;
            Start = startTime;
            End = endTime;
            EarliestTime = earliestTime;
            LatestTime = latestTime;
        }
    }

}
