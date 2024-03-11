using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IFriendsData
    {
        Task DeleteFriend(int friendshipId);
        Task<List<FriendsModel>> GetAllFriends();
        Task<FriendsModel> GetFriend(int friendshipId);
        Task UpdateFriend(FriendsModel friend);
    }
}