using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using System;

namespace HabitTrackerServices.Models.Firestore
{
    [FirestoreData]
    public class FireTaskGroup
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public string GroupId { get; set; }
        [FirestoreProperty]
        public string ColorHex { get; set; }
        [FirestoreProperty]
        public string GroupName { get; set; }
        [FirestoreProperty]
        public int GroupPosition { get; set; }
        [FirestoreProperty]
        public DateTime? InsertDate { get; set; }
        [FirestoreProperty]
        public DateTime? UpdateDate { get; set; }
        [FirestoreProperty]
        public bool Void { get; set; }
        [FirestoreProperty]
        public DateTime? VoidDate { get; set; }

        public TaskGroup ToTaskGroup()
        {
            var newGroup = new TaskGroup();
            newGroup.ColorHex = this.ColorHex;
            newGroup.GroupId = this.GroupId;
            newGroup.GroupName = this.GroupName;
            newGroup.GroupPosition = this.GroupPosition;
            newGroup.Id = this.Id;
            newGroup.InsertDate = this.InsertDate;
            newGroup.UpdateDate = this.UpdateDate;
            newGroup.UserId = this.UserId;
            newGroup.Void = this.Void;
            newGroup.VoidDate = this.VoidDate;

            return newGroup;
        }


        public static FireTaskGroup FromTaskGroup(TaskGroup group)
        {
            var newGroup = new FireTaskGroup();
            newGroup.ColorHex = group.ColorHex;
            newGroup.GroupId = group.GroupId;
            newGroup.GroupName = group.GroupName;
            newGroup.GroupPosition = group.GroupPosition;
            newGroup.Id = group.Id;
            newGroup.InsertDate = group.InsertDate;
            newGroup.UpdateDate = group.UpdateDate;
            newGroup.UserId = group.UserId;
            newGroup.Void = group.Void;
            newGroup.VoidDate = group.VoidDate;

            return newGroup;
        }
    }
}
