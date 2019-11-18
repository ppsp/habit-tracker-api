using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public class DTOCalendarTask
    {
        /// <summary>
        /// Represents the DocumentId
        /// </summary>
        public string CalendarTaskId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MinDuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<DayOfWeek> RequiredDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public eTaskFrequency Frequency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnPosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public eResultType ResultType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Positive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FontAwesomeIcon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Void { get; set; }
    }
}
