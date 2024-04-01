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

        public async Task<List<SchedulesModel>> GetSchedule(int userID)
        {
            string sql = "SELECT * FROM Schedules WHERE UserId = @UserId";
            return await _db.LoadData<SchedulesModel, dynamic>(sql, new { UserId = userID });
        }

        public async Task UpdateSchedule(SchedulesModel schedule)
        {
            string sql = @"UPDATE Schedules SET UserID = @UserId, Day = @Day, Start = @Start, End = @End, Text = @Text WHERE ScheduleID = @ScheduleId";
            await _db.SaveData(sql, schedule);
        }

        public async Task DeleteSchedule(int scheduleId)
        {
            string sql = "DELETE FROM Schedules WHERE ScheduleID = @ScheduleId";
            await _db.SaveData(sql, new { ScheduleId = scheduleId });
        }
        public async Task AddSchedule(SchedulesModel schedule)
        {
            string sql = @"INSERT INTO Schedules (UserID, Day, Start, End, Text) 
                   VALUES (@UserId, @Day, @Start, @End, @Text)";
            await _db.SaveData(sql, schedule);
        }
    }
}