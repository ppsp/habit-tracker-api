using System;
using System.Collections.Generic;

namespace HabitTrackerCore.Models
{
    public class CalendarTask
    {
        public virtual string CalendarTaskId { get; set; }

        public virtual string UserId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        /// <summary>
        /// The minimum duration of the task. This isn't an absolute minimum, it's simply
        /// the amount of time a person wishes to invest in this task.
        /// </summary>
        public virtual int MinDuration { get; set; }

        public virtual List<DayOfWeek> RequiredDays { get; set; }

        /// <summary>
        /// This is the frequency at which we want the task to be done
        /// </summary>
        public virtual eTaskFrequency Frequency { get; set; }

        /// <summary>
        /// Position (order) in the task list
        /// </summary>
        public virtual int AbsolutePosition { get; set; }

        /// <summary>
        /// The result type of the task. If It's simply Done or Not done, use Binary. 
        /// Use decimal to store a number and use Time to store a time of day
        /// </summary>
        public virtual eResultType ResultType { get; set; }

        /// <summary>
        /// This separates the "Good" habits from the "Bad" habits like eating junk food
        /// </summary>
        public virtual bool Positive { get; set; }

        /// <summary>
        /// When we delete a task this is set to true. When retrieving the tasks these will be 
        /// ignored and will only be used for historical purposes
        /// </summary>
        public virtual bool Void { get; set; }

        public virtual DateTime InsertDate { get; set; }

        public virtual DateTime? UpdateDate { get; set; }
        public virtual DateTime? VoidDate { get; set; }
    }
}
