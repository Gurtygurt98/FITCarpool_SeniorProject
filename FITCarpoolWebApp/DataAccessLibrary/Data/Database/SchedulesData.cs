using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class SchedulesData : ISchedulesData
    {
        private readonly ISQLDataAccess _db;

        public SchedulesData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<SchedulesModel>> GetAllSchedules()
        {
            string sql = "SELECT * FROM Schedules";
            return await _db.LoadData<SchedulesModel, dynamic>(sql, new { });
        }

        public async Task<SchedulesModel> GetSchedule(int scheduleId)
        {
            string sql = "SELECT * FROM Schedules WHERE ScheduleID = @ScheduleId";
            var result = await _db.LoadData<SchedulesModel, dynamic>(sql, new { ScheduleId = scheduleId });
            return result.FirstOrDefault();
        }

        public async Task UpdateSchedule(SchedulesModel schedule)
        {
<<<<<<< HEAD
            string sql = @"UPDATE Schedules SET UserID = @UserId, DayOfWeek = @DayOfWeek, StartTime = @StartTime, EndTime = @EndTime WHERE ScheduleID = @ScheduleId";
=======
            string sql = @"UPDATE Schedules SET UserID = @UserId, DayOfWeek = @DayOfWeek, StartTime = @StartTime, EndTime = @EndTime, Text = @Text WHERE ScheduleID = @ScheduleId";
>>>>>>> a0659ffbb39356fbf87dd3cb6550cd4e447510d7
            await _db.SaveData(sql, schedule);
        }

        public async Task DeleteSchedule(int scheduleId)
        {
            string sql = "DELETE FROM Schedules WHERE ScheduleID = @ScheduleId";
            await _db.SaveData(sql, new { ScheduleId = scheduleId });
        }
    }
}
