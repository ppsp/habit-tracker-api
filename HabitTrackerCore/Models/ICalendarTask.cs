using System;
using System.Collections.Generic;

namespace HabitTrackerCore.Models
{
    public interface ICalendarTask
    {
        public string CalendarTaskId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public List<DayOfWeek> RequiredDays { get; set; }

        /// <summary>
        /// This is the frequency at which we want the task to be done
        /// </summary>
        public eTaskFrequency Frequency { get; set; }

        /// <summary>
        /// Position (order) in the task list
        /// </summary>
        public int AbsolutePosition { get; set; }

        /// <summary>
        /// Used to track if AbsolutePosition has changed
        /// </summary>
        public int InitialAbsolutePosition { get; set; }

        /// <summary>
        /// The result type of the task. If It's simply Done or Not done, use Binary. 
        /// Use decimal to store a number and use Time to store a time of day
        /// </summary>
        public eResultType ResultType { get; set; }

        /// <summary>
        /// When we delete a task this is set to true. When retrieving the tasks these will be 
        /// ignored and will only be used for historical purposes
        /// </summary>
        public bool Void { get; set; }

        public DateTime? InsertDate { get; set; }

        public DateTime? UpdateDate { get; set; }
        public DateTime? VoidDate { get; set; }

        public DateTime? AssignedDate { get; set; }

        public List<ITaskHistory> Histories { get; set; }

        public eStatType StatType { get; set; }

        public DateTime? SkipUntil { get; set; }

        public bool HasBeenVoided();
        public bool PositionHasBeenModified();
    }
}
