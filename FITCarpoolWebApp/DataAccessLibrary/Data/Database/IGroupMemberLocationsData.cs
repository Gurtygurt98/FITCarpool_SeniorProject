using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IGroupMemberLocationsData
    {
        Task DeleteGroupMemberLocation(int locationId);
        Task<List<GroupMemberLocationsModel>> GetAllGroupMemberLocations();
        Task<GroupMemberLocationsModel> GetGroupMemberLocation(int locationId);
        Task UpdateGroupMemberLocation(GroupMemberLocationsModel location);
    }
}