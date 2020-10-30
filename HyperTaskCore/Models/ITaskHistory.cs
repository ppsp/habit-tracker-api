using HyperTaskCore.Utils;
using System;

namespace HyperTaskCore.Models
{
    public interface ITaskHistory
    {
        public string CalendarTaskId { get; set; }
        public string TaskHistoryId { get; set; }
        public string UserId { get; set; }
        [ReportInclude]
        public bool TaskDone { get; set; }
        [ReportInclude]
        public object TaskResult { get; set; }
        [ReportInclude]
        public bool TaskSkipped { get; set; }
        public int TaskDurationSeconds { get; set; }
        [ReportInclude]
        public DateTime? DoneDate { get; set; }
        [ReportInclude]
        public DateTime? DoneWorkDate { get; set; }
        [ReportInclude]
        public bool Void { get; set; }
        [ReportInclude]
        public DateTime? InsertDate { get; set; }
        [ReportInclude]
        public DateTime? UpdateDate { get; set; }
        [ReportInclude]
        public DateTime? VoidDate { get; set; }
        [ReportInclude]
        public string Comment { get; set; }
        public bool HasBeenVoided();
    }
}
