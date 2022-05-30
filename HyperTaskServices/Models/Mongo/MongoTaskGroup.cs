using HyperTaskCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace HyperTaskServices.Models.Mongo
{
    [BsonIgnoreExtraElements]
    public class MongoTaskGroup
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string UserId { get; set; }
        [BsonElement]
        public string GroupId { get; set; }
        [BsonElement]
        public string ColorHex { get; set; }
        [BsonElement]
        public string GroupName { get; set; }
        [BsonElement]
        public int Position { get; set; }
        [BsonElement]
        public int InitialPosition { get; set; }
        [BsonElement]
        public DateTime? InsertDate { get; set; }
        [BsonElement]
        public DateTime? UpdateDate { get; set; }
        [BsonElement]
        public bool Void { get; set; }
        [BsonElement]
        public DateTime? VoidDate { get; set; }
        [BsonElement]
        public bool RecurringDefault { get; set; }

        public TaskGroup ToTaskGroup()
        {
            var newGroup = new TaskGroup();
            newGroup.ColorHex = this.ColorHex;
            newGroup.GroupId = this.GroupId;
            newGroup.Name = this.GroupName;
            newGroup.Position = this.Position;
            newGroup.InitialPosition = this.InitialPosition;
            newGroup.InsertDate = this.InsertDate;
            newGroup.UpdateDate = this.UpdateDate;
            newGroup.UserId = this.UserId;
            newGroup.Void = this.Void;
            newGroup.VoidDate = this.VoidDate;
            newGroup.Id = this.Id;
            newGroup.RecurringDefault = this.RecurringDefault;

            return newGroup;
        }

        public static MongoTaskGroup FromTaskGroup(TaskGroup group)
        {
            var newGroup = new MongoTaskGroup();
            newGroup.ColorHex = group.ColorHex;
            newGroup.GroupId = group.GroupId;
            newGroup.GroupName = group.Name;
            newGroup.Position = group.Position;
            newGroup.InitialPosition = group.InitialPosition;
            newGroup.InsertDate = group.InsertDate;
            newGroup.UpdateDate = group.UpdateDate;
            newGroup.UserId = group.UserId;
            newGroup.Void = group.Void;
            newGroup.VoidDate = group.VoidDate;
            newGroup.Id = group.Id;
            newGroup.RecurringDefault = group.RecurringDefault;

            return newGroup;
        }
    }
}
