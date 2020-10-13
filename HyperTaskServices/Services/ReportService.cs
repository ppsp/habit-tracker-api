using HyperTaskCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperTaskServices.Services
{
    public class ReportService : IReportService
    {
        ICalendarTaskService TaskService { get; set; }
        ITaskGroupService GroupService { get; set; }

        public ReportService(ICalendarTaskService taskService,
                             ITaskGroupService groupService)
        {
            this.TaskService = taskService;
            this.GroupService = groupService;
        }

        public async Task<string> GetTasksCsv(string userId)
        {
            var tasks = await this.TaskService.GetTasksAsync(userId, true);
            var groups = await this.GroupService.GetGroupsAsync(userId, true);

            var lines = new List<string>();

            // Header
            var headerLine = String.Join(",", "Group Name", "Task Name", "History Datetime", "History Result");
            lines.Add(headerLine);

            // Loop on groups
            foreach (var group in groups)
            {
                lines.Add(group.Name);

                // Loop on tasks
                foreach (var task in tasks.Where(p => p.GroupId == group.GroupId))
                {
                    lines.Add(String.Concat(",", task.Name));

                    // Loop on histories
                    foreach (var history in task.Histories)
                    {
                        var columns = new List<string>();

                        columns.Add("");
                        columns.Add("");
                        columns.Add(history.DoneDate.ToString());
                        columns.Add(history.TaskResult == null ? "" : history.TaskResult.ToString());

                        lines.Add(String.Join(",", columns));
                    }
                }
            }

            var csvContent = String.Join(Environment.NewLine, lines);

            return csvContent;
        }
    }
}
