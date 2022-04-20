using Google.Cloud.Firestore;
using HyperTaskCore.Models;
using System;

namespace HyperTaskServices.Models.Firestore
{
    [FirestoreData]
    public class FireTaskGroup
    {
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public string GroupId { get; set; }
        [FirestoreProperty]
        public string ColorHex { get; set; }
        [FirestoreProperty]
        public string GroupName { get; set; }
        [FirestoreProperty]
        public int Position { get; set; }
        [FirestoreProperty]
        public int InitialPosition { get; set; }
        [FirestoreProperty]
        public DateTime? InsertDate { get; set; }
        [FirestoreProperty]
        public DateTime? UpdateDate { get; set; }
        [FirestoreProperty]
        public bool Void { get; set; }
        [FirestoreProperty]
        public DateTime? VoidDate { get; set; }
        [FirestoreProperty]
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
            newGroup.RecurringDefault = this.RecurringDefault;

            return newGroup;
        }


        public static FireTaskGroup FromTaskGroup(TaskGroup group)
        {
            var newGroup = new FireTaskGroup();
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
            newGroup.RecurringDefault = group.RecurringDefault;

            return newGroup;
        }
    }
}
