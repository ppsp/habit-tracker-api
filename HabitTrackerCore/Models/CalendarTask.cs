using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    [FirestoreData]
    public class CalendarTask
    {
        public string CalendarTaskId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty] //TODO: Core should not depend on Firebase (should not have any dependencies)
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public int MinDuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public List<DayOfWeek> RequiredDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public eTaskFrequency Frequency { get; set; }

        /// <summary>
        /// Position (order) in the task list
        /// </summary>
        [FirestoreProperty]
        public int AbsolutePosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public eResultType ResultType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public bool Positive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public string FontAwesomeIcon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public bool Void { get; set; }

        public CalendarTask()
        {

        }


        public CalendarTask(DTOCalendarTask task)
        {
            try
            {
                this.CalendarTaskId = task.CalendarTaskId;
                this.AbsolutePosition = task.AbsolutePosition;
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
            catch (Exception ex)
            {
                // TODO: Application Insight/log4net
            }
        }
    }
}
