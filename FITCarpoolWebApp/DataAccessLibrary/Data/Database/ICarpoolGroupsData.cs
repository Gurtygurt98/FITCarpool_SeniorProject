using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface ICarpoolGroupsData
    {
        Task<RecomendedGroup> GetSingleGroup(int GroupID, int requestingUser);
        Task CreateNewGroup(CarpoolGroupsModel carpoolGroup);
        Task DeleteCarpoolGroup(CarpoolGroupsModel group);
        Task<List<CarpoolGroupsModel>> GetAllCarpoolGroups();
        Task<List<RecomendedGroup>> GetAvailableGroups(int GoalUserID, List<string> Days);
        Task<List<CarpoolGroupsModel>> GetCarpoolGroup(int groupId);
        Task<List<(int, string, int, int)>> GetCurrentGroups(int GoalUserID);
        Task<List<CarpoolGroupsModel>> GetDriverGroups(int driverId);
        Task<int> GetGroupNumber(string GroupName, int CreatorID);
        Task<List<CarpoolGroupsModel>> GetRiderGroups(int userID);
        Task<List<CarpoolGroupsModel.RiderModel>> GetRiders(int groupId, int driverID);
        Task<List<int>> GetUserIds(int GroupID);
        Task JoinGroup(int GoalUserID, int GoalGroupID);
        Task RemoveGroupMember(int GoalUserID, int GoalGroupID);
        Task UpdateCarpoolGroup(CarpoolGroupsModel group);
        Task<List<RecomendedGroup>> GetRecommendGroups(int GoalUserID, List<string> Days, string TravelDirection, int DistanceWeight, int PreferenceWeight);
    }
}