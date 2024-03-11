using DataAccessLibrary.Model.Database_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class RolesData
    {
        private readonly ISQLDataAccess _db;

        public RolesData(ISQLDataAccess db)
        {
            _db = db;
        }
        public async Task<List<RolesModel>> GetUserRoles()
        {
            string sql = " ";
            return await _db.LoadData<RolesModel, dynamic>(sql, new { });
        }
    }
}
