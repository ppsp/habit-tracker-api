using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public class CalendarTaskHistory
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public bool TaskDone { get; set; }
        public object TaskResult { get; set; }
        public bool TaskSkipped { get; set; }
        public DateTime DoneDate { get; set; }
    }
}
