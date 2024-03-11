using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class UsersModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; }

        public UsersModel() { }

        public UsersModel(int userId, string email, string name, string phone, string userType)
        {
            UserId = userId;
            Email = email;
            Name = name;
            Phone = phone;
            UserType = userType;
        }
    }

}
