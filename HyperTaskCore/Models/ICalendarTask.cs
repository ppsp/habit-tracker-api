using HyperTaskCore.Utils;
using System;
using System.Collections.Generic;

namespace HyperTaskCore.Models
{
    public interface ICalendarTask
    {
        public string CalendarTaskId { get; set; }

        public string UserId { get; set; }

        [ReportInclude]
        public string Name { get; set; }

        [ReportInclude]
        public List<DayOfWeek> RequiredDays { get; set; }

        /// <summary>
        /// This is the frequency at which we want the task to be done
        /// </summary>
        [ReportInclude]
        public eTaskFrequency Frequency { get; set; }

        /// <summary>
        /// Position (order) in the task list
        /// </summary>
        [ReportInclude]
        public int AbsolutePosition { get; set; }

        /// <summary>
        /// Used to track if AbsolutePosition has changed
        /// </summary>
        public int InitialAbsolutePosition { get; set; }

        /// <summary>
        /// The result type of the task. If It's simply Done or Not done, use Binary. 
        /// Use decimal to store a number and use Time to store a time of day
        /// </summary>
        [ReportInclude]
        public eResultType ResultType { get; set; }

        /// <summary>
        /// When we delete a task this is set to true. When retrieving the tasks these will be 
        /// ignored and will only be used for historical purposes
        /// </summary>
        [ReportInclude]
        public bool Void { get; set; }
        [ReportInclude]
        public DateTime? InsertDate { get; set; }
        [ReportInclude]
        public DateTime? UpdateDate { get; set; }
        [ReportInclude]
        public DateTime? VoidDate { get; set; }
        [ReportInclude]
        public DateTime? AssignedDate { get; set; }

        public List<ITaskHistory> Histories { get; set; }

        public eStatType StatType { get; set; }
        [ReportInclude]
        public DateTime? SkipUntil { get; set; }

        /// <summary>
        /// Date completed for non-recurring tasks
        /// </summary>
        [ReportInclude]
        public DateTime? DoneDate { get; set; }

        /// <summary>
        /// We can group tasks together in Groups
        /// </summary>
        public string GroupId { get; set; }

        public int? NotificationId { get; set; }
        [ReportInclude]
        public string NotificationTime { get; set; }

        public bool HasBeenVoided();
        public bool PositionHasBeenModified();
    }
}
