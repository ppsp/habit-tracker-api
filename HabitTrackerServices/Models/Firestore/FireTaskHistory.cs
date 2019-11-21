using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerTools;
using System;
using System.Collections.Generic;

namespace HabitTrackerServices.Models.Firestore
{
    [FirestoreData]
    public class FireTaskHistory
    {
        [FirestoreProperty]
        public string CalendarTaskId { get; set; }
        [FirestoreProperty]
        public string TaskHistoryId { get; set; }
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public bool TaskDone { get; set; }
        [FirestoreProperty]
        public object TaskResult { get; set; }
        [FirestoreProperty]
        public bool TaskSkipped { get; set; }
        [FirestoreProperty]
        public int TaskDurationSeconds { get; set; }
        [FirestoreProperty]
        public DateTime DoneDate { get; set; }
        [FirestoreProperty]
        public bool Void { get; set; }
        [FirestoreProperty]
        public DateTime? InsertDate { get; set; }
        [FirestoreProperty]
        public DateTime? UpdateDate { get; set; }
        [FirestoreProperty]
        public DateTime? VoidDate { get; set; }

        public FireTaskHistory()
        {

        }

        public FireTaskHistory(ITaskHistory history)
        {
            this.UserId = history.UserId;
            this.CalendarTaskId = history.CalendarTaskId;
            this.DoneDate = history.DoneDate;
            this.InsertDate = history.InsertDate;
            this.TaskDone = history.TaskDone;
            this.TaskDurationSeconds = history.TaskDurationSeconds;
            this.TaskHistoryId = history.TaskHistoryId;
            this.TaskResult = history.TaskResult;
            this.TaskSkipped = history.TaskSkipped;
            this.UpdateDate = history.UpdateDate;
            this.UserId = history.UserId;
            this.Void = history.Void;
            this.VoidDate = history.VoidDate;
        }

        public TaskHistory ToTaskHistory()
        {
            var taskHistory = new TaskHistory();

            taskHistory.UserId = this.UserId;
            taskHistory.CalendarTaskId = this.CalendarTaskId;
            taskHistory.DoneDate = this.DoneDate;
            taskHistory.InsertDate = this.InsertDate;
            taskHistory.TaskDone = this.TaskDone;
            taskHistory.TaskDurationSeconds = this.TaskDurationSeconds;
            taskHistory.TaskHistoryId = this.TaskHistoryId;
            
            if (this.TaskResult is Timestamp)
            {
                taskHistory.TaskResult = ((Timestamp)this.TaskResult).ToDateTime();
            }
            else
            {
                taskHistory.TaskResult = this.TaskResult;
            }
                
            
            taskHistory.TaskSkipped = this.TaskSkipped;
            taskHistory.UpdateDate = this.UpdateDate;
            taskHistory.UserId = this.UserId;
            taskHistory.Void = this.Void;
            taskHistory.VoidDate = this.VoidDate;

            return taskHistory;
        }
    }
}
