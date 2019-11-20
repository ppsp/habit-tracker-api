using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerServices.Models.DTO;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Text;

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
        public string Description { get; set; }

        [FirestoreProperty]
        public int MinDuration { get; set; }

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
        public bool Positive { get; set; }

        [FirestoreProperty]
        public bool Void { get; set; }

        [FirestoreProperty]
        public DateTime? InsertDate { get; set; }
        [FirestoreProperty]
        public DateTime? UpdateDate { get; set; }
        [FirestoreProperty]
        public DateTime? VoidDate { get; set; }

        public FireCalendarTask()
        {

        }


        public FireCalendarTask(ICalendarTask task)
        {
            try
            {
                this.CalendarTaskId = task.CalendarTaskId;
                this.AbsolutePosition = task.AbsolutePosition;
                this.Description = task.Description;
                this.Frequency = task.Frequency;
                this.MinDuration = task.MinDuration;
                this.Name = task.Name;
                this.Positive = task.Positive;
                this.RequiredDays = task.RequiredDays;
                this.ResultType = task.ResultType;
                this.UserId = task.UserId;
                this.Void = task.Void;
                this.InsertDate = task.InsertDate;
                this.UpdateDate = task.UpdateDate;
                this.VoidDate = task.VoidDate;
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
            task.Description = this.Description;
            task.Frequency = this.Frequency;
            task.MinDuration = this.MinDuration;
            task.Name = this.Name;
            task.Positive = this.Positive;
            task.RequiredDays = this.RequiredDays;
            task.ResultType = this.ResultType;
            task.UserId = this.UserId;
            task.Void = this.Void;
            task.InsertDate = this.InsertDate;
            task.UpdateDate = this.UpdateDate;
            task.VoidDate = this.VoidDate;

            return task;
        }
    }
}
