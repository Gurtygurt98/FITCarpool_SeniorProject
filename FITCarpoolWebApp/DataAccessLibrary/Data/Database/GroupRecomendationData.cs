using DataAccessLibrary.Model.Logic_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public  class GroupRecomendationData
    {
        private readonly ISQLDataAccess _db;
        private readonly IUsersData _dbUsers;
        public GroupRecomendationData(ISQLDataAccess db, IUsersData usersData)
        {
            _db = db;
            _dbUsers = usersData;
        }
        public async Task<List<RecomendedGroup>> GetRecomendedGroupsForTimePeriod()
        {
            return null;
        }
        public async Task InsertRecommendedGroups()
        {

        }
    }
}
