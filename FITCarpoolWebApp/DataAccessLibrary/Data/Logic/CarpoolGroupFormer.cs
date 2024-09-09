using DataAccessLibrary.Model.Database_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Logic
{
    public class CarpoolGroupFormer
    {

        public void GetGroups(int GoalUserID, bool IsArriving, bool IsDeparting, string DayofWeek)
        {
            /*  
                                                    ***** Find users with a similar schedule ***** 

                Using the current userID, write an sql query to find all users that match the users schedule for their direction of travel
                or both if both IsArriving and IsDeparting. Also match day of the week if specificed. Goal is to 
                Input: 
                        GoalUserID - unique identifier - will be used to retrive the other users that match the Goal user schedule 
                        

            */
        }

    }
}
