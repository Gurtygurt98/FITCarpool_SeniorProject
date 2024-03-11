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
<<<<<<< HEAD
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public SchedulesModel() { }

        public SchedulesModel(int scheduleId, int userId, string dayOfWeek, TimeSpan startTime, TimeSpan endTime)
=======
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Text { get; set; }

        public SchedulesModel() { }

        public SchedulesModel(int scheduleId, int userId, string dayOfWeek, DateTime startTime, DateTime endTime)
>>>>>>> a0659ffbb39356fbf87dd3cb6550cd4e447510d7
        {
            ScheduleId = scheduleId;
            UserId = userId;
            DayOfWeek = dayOfWeek;
<<<<<<< HEAD
            StartTime = startTime;
            EndTime = endTime;
=======
            Start = startTime;
            End = endTime;
>>>>>>> a0659ffbb39356fbf87dd3cb6550cd4e447510d7
        }
    }

}
