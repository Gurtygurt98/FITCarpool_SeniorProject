using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IGroupMemberLocationsData
    {
        Task DeleteGroupMemberLocation(int locationId);
        Task<List<GroupMemberLocationsModel>> GetAllGroupMemberLocations();
        Task<List<GroupMemberLocationsModel>> GetGroupMemberLocation(int userId);
        Task UpdateGroupMemberLocation(GroupMemberLocationsModel location);
    }
}