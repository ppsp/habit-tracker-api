using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    [FirestoreData]
    public class CalendarTask
    {
        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
        public string CalendarTaskId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [FirestoreProperty]
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
        /// 
        /// </summary>
        [FirestoreProperty]
        public int ColumnPosition { get; set; }

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
                //this.ID = task.ID != null ? new ObjectId(task.ID) : new ObjectId();
                this.CalendarTaskId = task.CalendarTaskId;
                this.ColumnPosition = task.ColumnPosition;
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

            }
        }
    }
}
