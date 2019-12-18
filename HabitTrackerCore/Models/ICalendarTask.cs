using System;
using System.Collections.Generic;

namespace HabitTrackerCore.Models
{
    public interface ICalendarTask
    {
        public string CalendarTaskId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// The minimum duration of the task. This isn't an absolute minimum, it's simply
        /// the amount of time a person wishes to invest in this task.
        /// </summary>
        public int MinDuration { get; set; }

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
        /// This separates the "Good" habits from the "Bad" habits like eating junk food
        /// </summary>
        public bool Positive { get; set; }

        /// <summary>
        /// When we delete a task this is set to true. When retrieving the tasks these will be 
        /// ignored and will only be used for historical purposes
        /// </summary>
        public bool Void { get; set; }

        public DateTime? InsertDate { get; set; }

        public DateTime? UpdateDate { get; set; }
        public DateTime? VoidDate { get; set; }

        public DateTime? TaskAssignedDate { get; set; }

        public IEnumerable<ITaskHistory> Histories { get; set; }

        public bool HasBeenVoided();
        public bool PositionHasBeenModified();
    }
}
