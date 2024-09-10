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
        public async Task<List<UserInfoModel>> GetMatchingSchedules(int GoalUserID, bool IsArriving, bool IsDeparting, List<string> Days)
        {
            string TravelDirectionFilter = "";

            // Construct the filter for travel direction if arriving and/or departing
            if (IsArriving && IsDeparting)
            {
                TravelDirectionFilter = "s1.Text IN ('Arriving', 'Departing')";
            }
            else if (IsArriving)
            {
                TravelDirectionFilter = "s1.Text = 'Arriving'";
            }
            else if (IsDeparting)
            {
                TravelDirectionFilter = "s1.Text = 'Departing'";
            }

            string sql = $@"
                    SELECT s1.UserID, u.FirstName, u.LastName, u.UserType, u.PickupLocation, u.DropoffLocation, u.DrivingDistance, u.Gender, u.BeltCount, u.AllowEatDrink, u.AllowSmokeVape, p.GenderPreference, p.EatingPreference, p.SmokingPreference p.TemperaturePreference, p.MusicPreference
                    FROM Schedules s1
                    JOIN Schedules s2 
                        ON s1.UserID <> s2.UserID 
                        AND s1.Day = s2.Day 
                        AND s1.Text = s2.Text
                        AND (s1.Start < s2.End AND s1.End > s2.Start)
                    JOIN Users u on s1.UserID = u.UserID
                    JOIN Preferences p on u.UserID = p.UserID 
                    WHERE s2.UserID = @UserId
                    AND s1.Day IN @Days
                    AND {TravelDirectionFilter}
                    AND DATE(s1.Day) >= DATE('now', '-1 day');";

            List<UserInfoModel> MatchingScheduleUsers = await _db.LoadData<UserInfoModel, dynamic>(sql, new { UserId = GoalUserID, Days });

            return MatchingScheduleUsers;
        }
        public async Task<UserInfoModel> GetGoalUser(int GoalUserID)
        {

            string sql = $@"SELECT s1.UserID, u.FirstName, u.LastName, u.UserType, u.PickupLocation, u.DropoffLocation, u.DrivingDistance, u.Gender, u.BeltCount, u.AllowEatDrink, u.AllowSmokeVape, p.GenderPreference, p.EatingPreference, p.SmokingPreference p.TemperaturePreference, p.MusicPreference
                        FROM Users u
                        JOIN Preferences p on u.UserID = p.UserID
                        WHERE s2.UserID = @UserId";
            List<UserInfoModel> FoundUsers = await _db.LoadData<UserInfoModel, dynamic>(sql, new { UserId = GoalUserID });
            if (!FoundUsers.Any())
            {
                Console.WriteLine("Goal user not found");
            }
            return FoundUsers.First();

        }

    }
}