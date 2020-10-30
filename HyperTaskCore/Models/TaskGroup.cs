using HyperTaskCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyperTaskCore.Models
{
    public class TaskGroup
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string ColorHex { get; set; }
        [ReportInclude]
        public string Name { get; set; }
        [ReportInclude]
        public int Position { get; set; }
        public int InitialPosition { get; set; }
        [ReportInclude]
        public DateTime? InsertDate { get; set; }
        [ReportInclude]
        public DateTime? UpdateDate { get; set; }
        [ReportInclude]
        public bool Void { get; set; }
        [ReportInclude]
        public DateTime? VoidDate { get; set; }

        public TaskGroup()
        {
            this.InitialPosition = TaskPosition.MaxValue;
        }

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
