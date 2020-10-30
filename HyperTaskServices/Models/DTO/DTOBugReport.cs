using HyperTaskCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyperTaskServices.Models.DTO
{
    public class DTOBugReport
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime InsertDate { get; set; }
        public eBugReportType BugReportType { get; set; }
    }
}
