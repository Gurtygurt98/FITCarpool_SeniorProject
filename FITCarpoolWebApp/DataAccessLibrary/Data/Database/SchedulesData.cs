using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        public async Task<List<UserInfoModel>> GetMatchingSchedules(int GoalUserID, List<string> RequestDays, string TravelDirection)
        {
            string sql = @"SELECT 
                   u.UserID, 
                   u.FirstName, 
                   u.LastName, 
                   u.UserType, 
                   u.PickupLocation, 
                   u.DropoffLocation, 
                   u.DrivingDistance, 
                   u.Gender, 
                   u.BeltCount, 
                   u.AllowEatDrink, 
                   u.AllowSmokeVape, 
                   p.GenderPreference, 
                   p.EatingPreference, 
                   p.SmokingPreference, 
                   p.TemperaturePreference, 
                   p.MusicPreference,
                   l.PickupLatitude,
                   l.PickupLongitude, 
                   l.DropoffLongitude, 
                   l.DropoffLatitude, 
                   u.PhonePrivacy, 
                   u.AddressPrivacy, 
                   u.MakeModel, 
                   u.VehicleColor, 
                   u.LicensePlate, 
                   u.LicensePicture, 
                   u.CarPicture,
                   u.ProfilePicture,
                   u.Rating
            FROM Users u
            JOIN Locations l ON u.UserID = l.UserID
            JOIN Preferences p ON u.UserID = p.UserID
            JOIN Schedules s ON u.UserID = s.UserID
            WHERE u.UserID != @GoalUserID
              AND s.Day IN @RequestDays
              AND s.text = @TravelDirection
              AND s.Start >= datetime('now', '-1 day')
              AND EXISTS (
                    SELECT 1 
                    FROM Schedules gs 
                    WHERE gs.UserID = @GoalUserID
                      AND gs.Day = s.Day
                      AND gs.text = s.text
                      AND ABS(strftime('%s', gs.Start) - strftime('%s', s.Start)) <= 1800
                );";

            return await _db.LoadData<UserInfoModel, dynamic>(sql, new
            {
                GoalUserID,
                RequestDays,
                TravelDirection
            });
        }

    }
}