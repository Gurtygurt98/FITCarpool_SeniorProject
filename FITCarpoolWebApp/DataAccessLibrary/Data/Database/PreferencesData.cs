using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class PreferencesData : IPreferencesData
    {
        private readonly ISQLDataAccess _db;

        public PreferencesData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<PreferencesModel>> GetAllPreferences()
        {
            string sql = "SELECT * FROM Preferences";
            return await _db.LoadData<PreferencesModel, dynamic>(sql, new { });
        }

        public async Task<PreferencesModel> GetPreference(int userId)
        {
            string sql = "SELECT * FROM Preferences WHERE UserID = @UserId";
            var result = await _db.LoadData<PreferencesModel, dynamic>(sql, new { UserId = userId });
            return result.FirstOrDefault();
        }

        public async Task UpdatePreference(PreferencesModel preference)
        {
            string sql = @"UPDATE Preferences SET PreferenceValue = @PreferenceValue WHERE UserID = @UserId";
            await _db.SaveData(sql, preference);
        }

        public async Task DeletePreference(int userId)
        {
            string sql = "DELETE FROM Preferences WHERE UserID = @UserId";
            await _db.SaveData(sql, new { UserId = userId });
        }
    }
}
