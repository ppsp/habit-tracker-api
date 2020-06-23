using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public class TaskGroup
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

        public bool HasBeenVoided()
        {
            return this.Void && this.VoidDate == null;
        }

        public bool PositionHasBeenModified()
        {
            return this.Position != this.InitialPosition;
        }
    }
}
