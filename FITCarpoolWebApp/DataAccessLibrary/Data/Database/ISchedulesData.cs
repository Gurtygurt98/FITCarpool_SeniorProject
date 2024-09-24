using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;
namespace DataAccessLibrary.Data.Database
{
    public interface ISchedulesData
    {
        Task DeleteSchedule(int scheduleId);
        Task<List<SchedulesModel>> GetAllSchedules();
        Task<List<SchedulesModel>> GetSchedule(int userID);
        Task UpdateSchedule(SchedulesModel schedule);
        Task AddSchedule(SchedulesModel schedule);
        Task<List<RecomendedGroup>> GetMatchingSchedules(int GoalUserID, List<string> Days);
        Task<UserInfoModel> GetUserInfoModel(int GoalUserID);

    }
}