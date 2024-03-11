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
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public SchedulesModel() { }

        public SchedulesModel(int scheduleId, int userId, string dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            ScheduleId = scheduleId;
            UserId = userId;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
        }
    }

}
