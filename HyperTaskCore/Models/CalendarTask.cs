﻿using HyperTaskCore.Exceptions;
using HyperTaskCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperTaskCore.Models
{
    public class CalendarTask : ICalendarTask
    {
        public string Id { get; set; }
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
        public DateTime? SkipUntil { get; set; }

        public int? NotificationId { get; set; }
        public string NotificationTime { get; set; }
        public string GroupId { get; set; }

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

        public List<ITaskHistory> Histories { get; set; } = new List<ITaskHistory>();

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
