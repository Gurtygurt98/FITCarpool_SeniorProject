using DataAccessLibrary.Model;

namespace DataAccessLibrary.Data.Database
{
    public interface IPreferencesData
    {
        Task DeletePreference(int userId);
        Task<List<PreferencesModel>> GetAllPreferences();
        Task<PreferencesModel> GetPreference(int userId);
        Task UpdatePreference(PreferencesModel preference);
    }
}