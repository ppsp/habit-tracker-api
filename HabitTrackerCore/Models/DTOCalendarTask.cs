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
        public int AbsolutePosition { get; set; }

        /// <summary>
        /// This is needed in order to know if the position has to be updated
        /// because we can't update this property alone, we need to update every 
        /// element between the initial position and the target position
        /// </summary>
        public int InitialAbsolutePosition { get; set; }

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

        public DTOCalendarTask()
        {

        }

        public DTOCalendarTask(CalendarTask task)
        {
            this.CalendarTaskId = task.CalendarTaskId;
            this.AbsolutePosition = task.AbsolutePosition;
            this.InitialAbsolutePosition = task.AbsolutePosition;
            this.Description = task.Description;
            this.FontAwesomeIcon = task.FontAwesomeIcon;
            this.Frequency = task.Frequency;
            this.MinDuration = task.MinDuration;
            this.Name = task.Name;
            this.Positive = task.Positive;
            this.RequiredDays = task.RequiredDays;
            this.ResultType = task.ResultType;
            this.UserId = task.UserId;
            this.Void = task.Void;
        }
    }
}
