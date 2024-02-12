using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class ScheduleModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string Text { get; set; }
        public ScheduleModel() { }
        public ScheduleModel(string text, DateTime start, DateTime end) 
        {  
            this.Start = start;
            this.End = end;
            this.Text = text;
        }
    }
}
