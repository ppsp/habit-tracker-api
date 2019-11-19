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
    public class FireCalendarTask : CalendarTask
    {
        [FirestoreProperty]
        public override string UserId { get; set; }

        [FirestoreProperty]
        public override string Name { get; set; }

        [FirestoreProperty]
        public override string Description { get; set; }

        [FirestoreProperty]
        public override int MinDuration { get; set; }

        [FirestoreProperty]
        public override List<DayOfWeek> RequiredDays { get; set; }

        [FirestoreProperty]
        public override eTaskFrequency Frequency { get; set; }

        [FirestoreProperty]
        public override int AbsolutePosition { get; set; }

        [FirestoreProperty]
        public override eResultType ResultType { get; set; }

        [FirestoreProperty]
        public override bool Positive { get; set; }

        [FirestoreProperty]
        public override bool Void { get; set; }

        public FireCalendarTask()
        {

        }


        public FireCalendarTask(DTOCalendarTask task)
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

            task.CalendarTaskId = task.CalendarTaskId;
            task.AbsolutePosition = task.AbsolutePosition;
            task.Description = task.Description;
            task.Frequency = task.Frequency;
            task.MinDuration = task.MinDuration;
            task.Name = task.Name;
            task.Positive = task.Positive;
            task.RequiredDays = task.RequiredDays;
            task.ResultType = task.ResultType;
            task.UserId = task.UserId;
            task.Void = task.Void;

            return task;
        }
    }
}
