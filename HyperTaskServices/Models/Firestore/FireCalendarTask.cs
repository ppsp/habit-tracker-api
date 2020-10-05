using Google.Cloud.Firestore;
using HyperTaskCore.Models;
using HyperTaskCore.Utils;
using HyperTaskTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperTaskServices.Models.Firestore
{
    [FirestoreData]
    public class FireCalendarTask
    {
        public string Id { get; set; }
        [FirestoreProperty]
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
        [FirestoreProperty]
        public eStatType StatType { get; set; }
        [FirestoreProperty]
        public DateTime? SkipUntil { get; set; }
        [FirestoreProperty]
        public string GroupId { get; set; }
        [FirestoreProperty]
        public int? NotificationId { get; set; }
        [FirestoreProperty]
        public string NotificationTime { get; set; }
        [FirestoreProperty]
        public DateTime? DoneDate
        {
            get
            {
                if (this.Histories != null)
                {
                    var history = this.Histories?.FirstOrDefault(p => p.TaskDone &&
                                                                      this.Frequency.In(eTaskFrequency.Once, eTaskFrequency.UntilDone));

                    if (history != null && history.InsertDate != null)
                        return history.InsertDate.Value.Date;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            set
            {

            }
        }

        [FirestoreProperty]
        public FireTaskHistory[] Histories { get; set; } = new FireTaskHistory[0];

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
                this.NotificationId = task.NotificationId;
                this.NotificationTime = task.NotificationTime;
                this.AssignedDate = task.AssignedDate;
                this.StatType = task.StatType;
                this.Histories = task.Histories.Select(p => new FireTaskHistory(p)).ToArray();
                this.SkipUntil = task.SkipUntil;
                this.DoneDate = task.DoneDate;
                this.GroupId = task.GroupId;
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
            task.NotificationTime = this.NotificationTime;
            task.NotificationId = this.NotificationId;
            task.StatType = this.StatType;
            task.InitialAbsolutePosition = this.AbsolutePosition;
            task.Histories = this.Histories.Select(p => p.ToTaskHistory() as ITaskHistory).ToList();
            task.SkipUntil = this.SkipUntil;
            task.DoneDate = this.DoneDate;
            task.GroupId = this.GroupId;

            return task;
        }
    }
}
