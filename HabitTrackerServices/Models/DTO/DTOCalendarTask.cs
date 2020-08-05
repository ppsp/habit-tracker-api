using HabitTrackerCore.Models;
using HabitTrackerCore.Utils;
using HabitTrackerServices.Models.Firestore;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace HabitTrackerServices.Models.DTO
{
    public class DTOCalendarTask : ICalendarTask
    {
        public string CalendarTaskId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

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
        /// The result type of the task. If It's simply Done or Not done, use Binary. 
        /// Use decimal to store a number and use Time to store a time of day
        /// </summary>
        public eResultType ResultType { get; set; }

        /// <summary>
        /// When we delete a task this is set to true. When retrieving the tasks these will be 
        /// ignored and will only be used for historical purposes
        /// </summary>
        public bool Void { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? VoidDate { get; set; }
        public List<ITaskHistory> Histories { get; set; } = new List<ITaskHistory>();
        public eStatType StatType { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? SkipUntil { get; set; }
        public int? NotificationId { get; set; }
        public string NotificationTime { get; set; }
        public string GroupId { get; set; }
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

        /// <summary>
        /// This is needed in order to know if the position has to be updated
        /// because we can't update this property alone, we need to update every 
        /// element between the initial position and the target position
        /// </summary>
        public int InitialAbsolutePosition { get; set; }

        public bool PositionHasBeenModified()
        {
            return this.AbsolutePosition != this.InitialAbsolutePosition;
        }

        public bool HasBeenVoided()
        {
            return this.Void && this.VoidDate == null;
        }

        public DTOCalendarTask()
        {
            this.InitialAbsolutePosition = TaskPosition.MaxValue;
        }

        public DTOCalendarTask(FireCalendarTask task)
        {
            try
            {
                this.InitialAbsolutePosition = task.AbsolutePosition;

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
                this.StatType = task.StatType;
                this.SkipUntil = task.SkipUntil;
                this.NotificationId = task.NotificationId;
                this.NotificationTime = task.NotificationTime;
                this.DoneDate = task.DoneDate;
                this.GroupId = task.GroupId;
            }
            catch (Exception ex)
            {
                Logger.Error("Error constructing DTOCalendarTask", ex);
                throw;
            }
        }

        public DTOCalendarTask(ICalendarTask task)
        {
            try
            {
                this.InitialAbsolutePosition = task.InitialAbsolutePosition;
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
                this.Histories = task.Histories;
                this.AssignedDate = task.AssignedDate;
                this.NotificationId = task.NotificationId;
                this.NotificationTime = task.NotificationTime;
                this.StatType = task.StatType;
                this.SkipUntil = task.SkipUntil;
                this.CalendarTaskId = task.CalendarTaskId;
                this.DoneDate = task.DoneDate;
                this.GroupId = task.GroupId;
            }
            catch (Exception ex)
            {
                Logger.Error("Error constructing DTOCalendarTask", ex);
                throw;
            }
        }

        public CalendarTask ToCalendarTask()
        {
            CalendarTask task = new CalendarTask();

            task.CalendarTaskId = this.CalendarTaskId;
            task.AbsolutePosition = this.AbsolutePosition;
            task.InitialAbsolutePosition = this.InitialAbsolutePosition;
            task.Frequency = this.Frequency;
            task.Name = this.Name;
            task.RequiredDays = this.RequiredDays;
            task.ResultType = this.ResultType;
            task.UserId = this.UserId;
            task.Void = this.Void;
            task.InsertDate = this.InsertDate;
            task.UpdateDate = this.UpdateDate;
            task.VoidDate = this.VoidDate;
            task.Histories = this.Histories;
            task.AssignedDate = this.AssignedDate;
            task.StatType = this.StatType;
            task.SkipUntil = this.SkipUntil;
            task.NotificationTime = this.NotificationTime;
            task.NotificationId = this.NotificationId;
            task.DoneDate = this.DoneDate;
            task.GroupId = this.GroupId;

            return task;
        }

        public void Validate()
        {
            if (this.Name.Length > 200)
                throw new InvalidDataException("Task Name should not be over 200 characters");

            foreach (var history in this.Histories)
                if (history.Comment != null && history.Comment.Length > 2000)
                    throw new InvalidDataException("History Comment should not be over 2000 characters");

            return;
        }
    }
}
