using DataAccessLibrary.Model;

namespace DataAccessLibrary.Data.Database
{
    public interface ICarpoolGroupsData
    {
        Task DeleteCarpoolGroup(CarpoolGroupsModel group);
        Task<List<CarpoolGroupsModel>> GetAllCarpoolGroups();
        Task<List<CarpoolGroupsModel>> GetCarpoolGroup(int groupId);
        Task UpdateCarpoolGroup(CarpoolGroupsModel group);

    }
}