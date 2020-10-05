using HyperTaskCore.Models;
using HyperTaskCore.Utils;
using HyperTaskServices.Models.Firestore;
using HyperTaskTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperTaskServices.Models.DTO
{
    public class DTOTaskGroup
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string ColorHex { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public int InitialPosition { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool Void { get; set; }
        public DateTime? VoidDate { get; set; }

        public DTOTaskGroup()
        {
            this.InitialPosition = TaskPosition.MaxValue;
        }

        public TaskGroup ToTaskGroup()
        {
            var newGroup = new TaskGroup();
            newGroup.ColorHex = this.ColorHex;
            newGroup.GroupId = this.GroupId;
            newGroup.Name = this.Name;
            newGroup.Position = this.Position;
            newGroup.InitialPosition = this.InitialPosition;
            newGroup.Id = this.Id;
            newGroup.InsertDate = this.InsertDate;
            newGroup.UpdateDate = this.UpdateDate;
            newGroup.UserId = this.UserId;
            newGroup.Void = this.Void;
            newGroup.VoidDate = this.VoidDate;

            return newGroup;
        }


        public static DTOTaskGroup FromTaskGroup(TaskGroup group)
        {
            var newGroup = new DTOTaskGroup();
            newGroup.ColorHex = group.ColorHex;
            newGroup.GroupId = group.GroupId;
            newGroup.Name = group.Name;
            newGroup.Position = group.Position;
            newGroup.InitialPosition = group.InitialPosition;
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
