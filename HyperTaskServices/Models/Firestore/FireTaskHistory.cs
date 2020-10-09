using Google.Cloud.Firestore;
using HyperTaskCore.Models;
using HyperTaskTools;
using System;
using System.Collections.Generic;

namespace HyperTaskServices.Models.Firestore
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
        public DateTime? DoneDate { get; set; }
        [FirestoreProperty]
        public DateTime? DoneWorkDate { get; set; }
        [FirestoreProperty]
        public bool Void { get; set; }
        [FirestoreProperty]
        public DateTime? InsertDate { get; set; }
        [FirestoreProperty]
        public DateTime? UpdateDate { get; set; }
        [FirestoreProperty]
        public DateTime? VoidDate { get; set; }
        [FirestoreProperty]
        public string Comment { get; set; }

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
            this.Comment = history.Comment;
            this.DoneWorkDate = history.DoneWorkDate;
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
            taskHistory.DoneWorkDate = this.DoneWorkDate;
            taskHistory.UserId = this.UserId;
            taskHistory.Void = this.Void;
            taskHistory.VoidDate = this.VoidDate;
            taskHistory.Comment = this.Comment;

            return taskHistory;
        }
    }
}
