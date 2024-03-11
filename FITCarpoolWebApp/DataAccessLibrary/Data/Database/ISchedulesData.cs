using DataAccessLibrary.Model;

namespace DataAccessLibrary.Data.Database
{
    public interface ISchedulesData
    {
        Task DeleteSchedule(int scheduleId);
        Task<List<SchedulesModel>> GetAllSchedules();
        Task<SchedulesModel> GetSchedule(int scheduleId);
        Task UpdateSchedule(SchedulesModel schedule);
    }
}