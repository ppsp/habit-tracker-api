using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public class CalendarTask : ICalendarTask
    {
        public string CalendarTaskId { get ; set ; }
        public string UserId { get ; set ; }
        public string Name { get ; set ; }
        public string Description { get ; set ; }
        public int MinDuration { get ; set ; }
        public List<DayOfWeek> RequiredDays { get ; set ; }
        public eTaskFrequency Frequency { get ; set ; }
        public int AbsolutePosition { get ; set ; }
        public eResultType ResultType { get ; set ; }
        public bool Positive { get ; set ; }
        public bool Void { get ; set ; }
        public DateTime? InsertDate { get ; set ; }
        public DateTime? UpdateDate { get ; set ; }
        public DateTime? VoidDate { get ; set ; }
    }
}
