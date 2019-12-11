using HabitTrackerCore.Models;
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

        public GetCalendarTaskRequest ToCalendarTaskRequest()
        {
            var request = new GetCalendarTaskRequest();
            request.UserId = this.userId;
            request.IncludeVoid = this.IncludeVoid;
            request.DateStart = this.DateStart;
            request.DateEnd = this.DateEnd;

            if (request.DateStart.HasValue && request.DateStart.Value.Kind != DateTimeKind.Utc)
                request.DateStart = request.DateStart.Value.ToUniversalTime();

            if (request.DateEnd.HasValue && request.DateEnd.Value.Kind != DateTimeKind.Utc)
                request.DateEnd = request.DateEnd.Value.ToUniversalTime();

            return request;
        }
    }
}
