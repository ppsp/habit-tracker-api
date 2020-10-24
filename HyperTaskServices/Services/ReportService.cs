using HyperTaskCore.Models;
using HyperTaskCore.Services;
using HyperTaskCore.Utils;
using System;
using System.Collections;
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

            List<Type> collections = new List<Type>() { typeof(IEnumerable<>), typeof(IEnumerable) };

            var taskProperties = typeof(ICalendarTask).GetProperties()
                                                      .Where(p => p.CustomAttributes.Any(p => p.AttributeType == typeof(ReportInclude)))
                                                      .ToList();
            var historyProperties = typeof(ITaskHistory).GetProperties()
                                                        .Where(p => p.CustomAttributes.Any(p => p.AttributeType == typeof(ReportInclude)))
                                                        .ToList();
            var groupProperties = typeof(TaskGroup).GetProperties()
                                                   .Where(p => p.CustomAttributes.Any(p => p.AttributeType == typeof(ReportInclude)))
                                                   .ToList();

            // Header
            // var headerLine = String.Join(",", "Group Name", "Task Name", "History Datetime", "History Result");
            var headerLine = String.Join(",", String.Join(",", groupProperties.Select(p => "Group " + p.Name)),
                                              String.Join(",", taskProperties.Select(p => "Task " + p.Name)),
                                              String.Join(",", historyProperties.Select(p => "Result " + p.Name)));

            lines.Add(headerLine);

            // Loop on groups
            foreach (var group in groups.OrderBy(p => p.Position))
            {
                // lines.Add(group.Name);
                lines.Add(String.Join(",", groupProperties.Select(p => group.GetPropertyValue(p.Name) == null ?
                                                                       "" :
                                                                       group.GetPropertyValue(p.Name).ToString().Replace(",", " ")))); // group values

                // Loop on tasks
                foreach (var task in tasks.Where(p => p.GroupId == group.GroupId).OrderBy(p => p.AbsolutePosition))
                {
                    lines.Add(String.Concat(new String(',', groupProperties.Count), // group columns
                                            String.Join(",", taskProperties.Select(p => task.GetPropertyValue(p.Name) == null ?
                                                                                        "" :
                                                                                        typeof(IEnumerable<DayOfWeek>).IsAssignableFrom(p.PropertyType) ?
                                                                                            String.Join(";", ((List<DayOfWeek>)(task.GetPropertyValue(p.Name))).Select(q => (int)q)) : // Day of weeks
                                                                                            task.GetPropertyValue(p.Name).ToString().Replace(",", " "))))); // task values

                    // Loop on histories
                    foreach (var history in task.Histories.OrderBy(p => p.DoneWorkDate))
                    {
                        lines.Add(String.Concat(new String(',', groupProperties.Count), // group columns
                                                new String(',', taskProperties.Count), // task columns
                                                String.Join(",", historyProperties.Select(p => history.GetPropertyValue(p.Name) == null ? 
                                                                                                  "" :
                                                                                                  history.GetPropertyValue(p.Name).ToString().Replace(",", " "))))); // history values
                    }
                }
            }

            var csvContent = String.Join(Environment.NewLine, lines);

            return csvContent;
        }
    }
}
