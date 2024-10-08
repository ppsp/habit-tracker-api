﻿using System;

namespace HyperTaskCore.Models
{
    public class TaskHistory : ITaskHistory
    {
        public string CalendarTaskId { get; set; }
        public string TaskHistoryId { get; set; }
        public string UserId { get; set; }
        public bool TaskDone { get; set; }
        public object TaskResult { get; set; }
        public bool TaskSkipped { get; set; }
        public int TaskDurationSeconds { get; set; }
        public DateTime? DoneDate { get; set; }
        public DateTime? DoneWorkDate { get; set; }
        public bool Void { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? VoidDate { get; set; }
        public string Comment { get; set; }

        public TaskHistory()
        {

        }

        public TaskHistory(ICalendarTask task)
        {
            this.UserId = task.UserId;
            this.CalendarTaskId = task.CalendarTaskId;
        }
        public bool HasBeenVoided()
        {
            return this.Void && this.VoidDate == null;
        }
    }
}
