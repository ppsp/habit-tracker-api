using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerServices.Models.DTO
{
    public class DTOGetCalendarTaskRequest
    {
        public string userId { get; set; }
        public bool IncludeVoid { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
