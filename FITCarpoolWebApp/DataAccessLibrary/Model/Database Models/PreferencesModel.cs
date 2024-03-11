using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class PreferencesModel
    {
        public int UserId { get; set; }
        public string PreferenceValue { get; set; }

        public PreferencesModel() { }

        public PreferencesModel(int userId, string preferenceValue)
        {
            UserId = userId;
            PreferenceValue = preferenceValue;
        }
    }

}
