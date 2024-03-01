using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class PreferencesModel
    {
        public string? ID {  get; set; } 
        public string? PreferenceType { get; set; }
        public string? PreferenceValue { get; set; }
        public PreferencesModel() { }
    }
}
