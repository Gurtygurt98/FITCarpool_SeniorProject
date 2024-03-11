using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class FriendsData : IFriendsData
    {
        private readonly ISQLDataAccess _db;

        public FriendsData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<FriendsModel>> GetAllFriends()
        {
            string sql = "SELECT * FROM Friends";
            return await _db.LoadData<FriendsModel, dynamic>(sql, new { });
        }

        public async Task<FriendsModel> GetFriend(int friendshipId)
        {
            string sql = "SELECT * FROM Friends WHERE FriendshipID = @FriendshipId";
            var result = await _db.LoadData<FriendsModel, dynamic>(sql, new { FriendshipId = friendshipId });
            return result.FirstOrDefault();
        }

        public async Task UpdateFriend(FriendsModel friend)
        {
            string sql = @"UPDATE Friends SET UserID1 = @UserId1, UserID2 = @UserId2, Status = @Status, CreatedDate = @CreatedDate WHERE FriendshipID = @FriendshipId";
            await _db.SaveData(sql, friend);
        }

        public async Task DeleteFriend(int friendshipId)
        {
            string sql = "DELETE FROM Friends WHERE FriendshipID = @FriendshipId";
            await _db.SaveData(sql, new { FriendshipId = friendshipId });
        }
    }
}
