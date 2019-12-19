using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerTools;
using System;
using System.Collections.Generic;

namespace HabitTrackerServices.Models.Firestore
{
    [FirestoreData]
    public class FireCalendarTask
    {
        public string CalendarTaskId { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public List<DayOfWeek> RequiredDays { get; set; }

        [FirestoreProperty]
        public eTaskFrequency Frequency { get; set; }

        /// <summary>
        /// Workaround because custom type conversion doesn't work
        /// </summary>
        public int absolutePosition { get; set; }

        [FirestoreProperty]
        public int AbsolutePosition { get; set; }

        [FirestoreProperty]
        public eResultType ResultType { get; set; }

        [FirestoreProperty]
        public bool Void { get; set; }

        [FirestoreProperty]
        public DateTime? InsertDate { get; set; }
        [FirestoreProperty]
        public DateTime? UpdateDate { get; set; }
        [FirestoreProperty]
        public DateTime? VoidDate { get; set; }
        [FirestoreProperty]
        public DateTime? AssignedDate { get; set; }

        public FireCalendarTask()
        {

        }


        public FireCalendarTask(ICalendarTask task)
        {
            try
            {
                this.CalendarTaskId = task.CalendarTaskId;
                this.AbsolutePosition = task.AbsolutePosition;
                this.Frequency = task.Frequency;
                this.Name = task.Name;
                this.RequiredDays = task.RequiredDays;
                this.ResultType = task.ResultType;
                this.UserId = task.UserId;
                this.Void = task.Void;
                this.InsertDate = task.InsertDate;
                this.UpdateDate = task.UpdateDate;
                this.VoidDate = task.VoidDate;
                this.AssignedDate = task.AssignedDate;
            }
            catch (Exception ex)
            {
                Logger.Error("Error constructing FireCalendarTask", ex);
                throw;
            }
        }

        public CalendarTask ToCalendarTask()
        {
            CalendarTask task = new CalendarTask();

            task.CalendarTaskId = this.CalendarTaskId;
            task.AbsolutePosition = this.AbsolutePosition;
            task.Frequency = this.Frequency;
            task.Name = this.Name;
            task.RequiredDays = this.RequiredDays;
            task.ResultType = this.ResultType;
            task.UserId = this.UserId;
            task.Void = this.Void;
            task.InsertDate = this.InsertDate;
            task.UpdateDate = this.UpdateDate;
            task.VoidDate = this.VoidDate;
            task.AssignedDate = this.AssignedDate;

            return task;
        }
    }
}
