using HyperTaskCore.Models;
using HyperTaskCore.Utils;
using HyperTaskTools;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperTaskServices.Models.Mongo
{
    [BsonIgnoreExtraElements]
    public class MongoCalendarTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string CalendarTaskId { get; set; }
        [BsonElement]
        public string UserId { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public List<DayOfWeek> RequiredDays { get; set; }
        [BsonElement]
        public eTaskFrequency Frequency { get; set; }
        [BsonElement]
        public int AbsolutePosition { get; set; }
        [BsonElement]
        public eResultType ResultType { get; set; }
        [BsonElement]
        public bool Void { get; set; }
        [BsonElement]
        public DateTime? InsertDate { get; set; }
        [BsonElement]
        public DateTime? UpdateDate { get; set; }
        [BsonElement]
        public DateTime? VoidDate { get; set; }
        [BsonElement]
        public DateTime? AssignedDate { get; set; }
        [BsonElement]
        public eStatType StatType { get; set; }
        [BsonElement]
        public DateTime? SkipUntil { get; set; }
        [BsonElement]
        public string GroupId { get; set; }
        [BsonElement]
        public int? NotificationId { get; set; }
        [BsonElement]
        public string NotificationTime { get; set; }
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
        [BsonElement]
        public MongoTaskHistory[] Histories { get; set; } = new MongoTaskHistory[0];

        public MongoCalendarTask()
        {

        }

        public MongoCalendarTask(ICalendarTask task)
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
                this.Histories = task.Histories.Select(p => new MongoTaskHistory(p)).ToArray();
                this.SkipUntil = task.SkipUntil;
                this.DoneDate = task.DoneDate;
                this.GroupId = task.GroupId;
                this.Id = task.Id;
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
            task.Id = this.Id;

            return task;
        }
    }
}
