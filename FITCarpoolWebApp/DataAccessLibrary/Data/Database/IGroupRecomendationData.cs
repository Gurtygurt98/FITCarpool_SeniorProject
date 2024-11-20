using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IGroupRecommendationData
    {
        Task<List<RecomendedGroup>> GetRecommendedGroupsForTimePeriod(Week week);
        Task InsertRecommendedGroups(List<RecomendedGroup> recommendedGroups);
        Task InsertRecommendedGroups(RecomendedGroup recommendedGroup);
        Task<List<RecomendedGroup>> GetUsersUpcomingRecommendations(int userID);
        Task AcceptGroupRec(RecomendedGroup group, int UserID);
        Task DeclineGroupRec(RecomendedGroup group, int UserID);
    }
}