using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class CarpoolGroupsData : ICarpoolGroupsData
    {
        private readonly ISQLDataAccess _db;

        public CarpoolGroupsData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<CarpoolGroupsModel>> GetAllCarpoolGroups()
        {
            string sql = "SELECT * FROM CarpoolGroups";
            return await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { });
        }

        public async Task<List<CarpoolGroupsModel>> GetCarpoolGroup(int groupId)
        {
            string sql = @"SELECT * FROM CarpoolGroups WHERE GroupID = @GroupId";
            return await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { GroupId = groupId });
        }

        public async Task UpdateCarpoolGroup(CarpoolGroupsModel group)
        {
            string sql = @"UPDATE CarpoolGroups 
                           SET GroupName = @GroupName, DriverID = @DriverId, Destination = @Destination, MeetingPoint = @MeetingPoint 
                           WHERE GroupID = @GroupId";
            await _db.SaveData(sql, group);
        }

        public async Task DeleteCarpoolGroup(CarpoolGroupsModel group)
        {
            string sql = @"DELETE FROM CarpoolGroups WHERE GroupID = @GroupId";
            await _db.SaveData(sql, new { group.GroupId });
        }
        public async Task<List<CarpoolGroupsModel>> GetDriverGroups(int driverId)
        {
            string sql = @"select * from CarpoolGroups where DriverID = @DriverID";
            return await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { DriverID = driverId });
        }
    }
}
