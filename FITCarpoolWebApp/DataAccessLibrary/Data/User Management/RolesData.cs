using DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.User_Management
{
    public class RolesData
    {
        private readonly ISQLDataAccess _db;

        public RolesData(ISQLDataAccess db)
        {
            _db = db;
        }
        public Task<List<RolesModel>> GetUserRoles()
        {
            string sql = " ";
            return _db.LoadData<RolesModel, dynamic>(sql, new { });
        }
    }
}
