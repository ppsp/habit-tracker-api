using HabitTrackerCore.Models;
using HabitTrackerServices.Models.Firestore;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerServices.Models.DTO
{
    public class DTOCalendarTask : CalendarTask
    {
        /// <summary>
        /// This is needed in order to know if the position has to be updated
        /// because we can't update this property alone, we need to update every 
        /// element between the initial position and the target position
        /// </summary>
        public int InitialAbsolutePosition { get; set; }

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
            }
            catch (Exception ex)
            {
                Logger.Error("Error constructing DTOCalendarTask", ex);
                throw;
            }
        }

        public DTOCalendarTask(CalendarTask task)
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
