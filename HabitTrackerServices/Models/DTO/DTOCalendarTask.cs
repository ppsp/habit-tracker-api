using HabitTrackerCore.Models;
using HabitTrackerServices.Models.Firestore;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerServices.Models.DTO
{
    public class DTOCalendarTask : ICalendarTask
    {
        public string CalendarTaskId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// The minimum duration of the task. This isn't an absolute minimum, it's simply
        /// the amount of time a person wishes to invest in this task.
        /// </summary>
        public int MinDuration { get; set; }

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
        /// This separates the "Good" habits from the "Bad" habits like eating junk food
        /// </summary>
        public bool Positive { get; set; }

        /// <summary>
        /// When we delete a task this is set to true. When retrieving the tasks these will be 
        /// ignored and will only be used for historical purposes
        /// </summary>
        public bool Void { get; set; }

        public DateTime? InsertDate { get; set; }

        public DateTime? UpdateDate { get; set; }
        public DateTime? VoidDate { get; set; }

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

        }

        public DTOCalendarTask(FireCalendarTask task)
        {
            try
            {
                this.InitialAbsolutePosition = task.AbsolutePosition;

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
                Logger.Error("Error constructing DTOCalendarTask", ex);
                throw;
            }
        }

        public DTOCalendarTask(ICalendarTask task)
        {
            try
            {
                this.InitialAbsolutePosition = task.AbsolutePosition;

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
                Logger.Error("Error constructing DTOCalendarTask", ex);
                throw;
            }
        }

        public DTOCalendarTask(ICalendarTask task, int initialAbsolutePosition)
        {
            try
            {
                this.InitialAbsolutePosition = initialAbsolutePosition;
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
                Logger.Error("Error constructing DTOCalendarTask", ex);
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
