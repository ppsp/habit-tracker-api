﻿using HabitTrackerCore.Exceptions;
using HabitTrackerCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public class CalendarTask : ICalendarTask
    {
        public string CalendarTaskId { get ; set ; }
        public string UserId { get ; set ; }
        public string Name { get ; set ; }
        public List<DayOfWeek> RequiredDays { get ; set ; }
        public eTaskFrequency Frequency { get ; set ; }
        public int AbsolutePosition { get ; set ; }
        public int InitialAbsolutePosition { get; set; }
        public eResultType ResultType { get ; set ; }
        public bool Void { get ; set ; }
        public DateTime? InsertDate { get ; set ; }
        public DateTime? UpdateDate { get ; set ; }
        public DateTime? VoidDate { get ; set ; }
        public DateTime? AssignedDate { get; set; }
        public eStatType StatType { get; set; }

        public IEnumerable<ITaskHistory> Histories { get; set; }

        public bool HasBeenVoided()
        {
            return this.Void && this.VoidDate == null;
        }

        public bool PositionHasBeenModified()
        {
            return this.AbsolutePosition != this.InitialAbsolutePosition;
        }

        public CalendarTask()
        {
            this.InitialAbsolutePosition = TaskPosition.MaxValue;
        }
    }
}
