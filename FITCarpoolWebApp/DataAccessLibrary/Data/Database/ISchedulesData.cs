using DataAccessLibrary.Model;

namespace DataAccessLibrary.Data.Database
{
    public interface ISchedulesData
    {
        Task DeleteSchedule(int scheduleId);
        Task<List<SchedulesModel>> GetAllSchedules();
        Task<List<SchedulesModel>> GetSchedule(int userID);
        Task UpdateSchedule(SchedulesModel schedule);
        Task AddSchedule(SchedulesModel schedule);
    }
}