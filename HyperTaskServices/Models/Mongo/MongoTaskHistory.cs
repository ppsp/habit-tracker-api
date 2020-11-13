using HyperTaskCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace HyperTaskServices.Models.Mongo
{
    [BsonIgnoreExtraElements]
    public class MongoTaskHistory
    {
        [BsonElement]
        public string Id { get; set; }
        [BsonElement]
        public string CalendarTaskId { get; set; }
        [BsonElement]
        public string TaskHistoryId { get; set; }
        [BsonElement]
        public string UserId { get; set; }
        [BsonElement]
        public bool TaskDone { get; set; }
        [BsonElement]
        public object TaskResult { get; set; }
        [BsonElement]
        public bool TaskSkipped { get; set; }
        [BsonElement]
        public int TaskDurationSeconds { get; set; }
        [BsonElement]
        public DateTime? DoneDate { get; set; }
        [BsonElement]
        public DateTime? DoneWorkDate { get; set; }
        [BsonElement]
        public bool Void { get; set; }
        [BsonElement]
        public DateTime? InsertDate { get; set; }
        [BsonElement]
        public DateTime? UpdateDate { get; set; }
        [BsonElement]
        public DateTime? VoidDate { get; set; }
        [BsonElement]
        public string Comment { get; set; }

        public MongoTaskHistory()
        {

        }

        public MongoTaskHistory(ITaskHistory history)
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
            
            /*if (this.TaskResult is Timestamp)
            {
                taskHistory.TaskResult = ((Timestamp)this.TaskResult).ToDateTime();
            }
            else
            {*/
                taskHistory.TaskResult = this.TaskResult;
            //}
                
            
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
