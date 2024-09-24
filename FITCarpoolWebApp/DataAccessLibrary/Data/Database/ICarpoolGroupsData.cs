using DataAccessLibrary.Model;
using static DataAccessLibrary.Model.CarpoolGroupsModel;

namespace DataAccessLibrary.Data.Database
{
    public interface ICarpoolGroupsData
    {
        Task DeleteCarpoolGroup(CarpoolGroupsModel group);
        Task<List<CarpoolGroupsModel>> GetAllCarpoolGroups();
        Task<List<CarpoolGroupsModel>> GetCarpoolGroup(int groupId);
        Task UpdateCarpoolGroup(CarpoolGroupsModel group);
        Task<List<CarpoolGroupsModel>> GetDriverGroups(int driverId);
        Task<List<RiderModel>> GetRiders(int groupId, int driverID);
        Task<List<CarpoolGroupsModel>> GetRiderGroups(int userID);
        Task CreateNewGroup(CarpoolGroupsModel carpoolGroup);
        Task<int> GetGroupNumber(string GroupName, int CreatorID);
    }
}