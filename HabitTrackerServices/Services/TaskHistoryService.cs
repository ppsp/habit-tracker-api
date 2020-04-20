using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerTools;
using System;
using System.Threading.Tasks;

namespace HabitTrackerServices.Services
{
    public class TaskHistoryService : ITaskHistoryService
    {
        private ICalendarTaskService CalendarTaskService { get; set; }

        public TaskHistoryService(ICalendarTaskService calendarTaskService)
        {
            this.CalendarTaskService = calendarTaskService;
        }

        private static void SetDefaultDateValues(GetCalendarTaskRequest request)
        {
            if (request.DateStart == null)
                request.DateStart = DateTime.Today.ToUniversalTime();
            if (request.DateEnd == null)
                request.DateEnd = DateTime.Today.AddDays(1).ToUniversalTime();
        }

        public async Task<string> InsertHistoryAsync(ITaskHistory history)
        {
            try
            {
                return await insertHistoryAsync(history);
            }
            catch (Exception ex)
            {
                // TODO: Throw exception instead of returning null and add an exceptoin handler on the controller

                Logger.Error("Error in InsertHistoryAsync", ex);
                return null;
            }
        }

        private async Task<string> insertHistoryAsync(ITaskHistory history)
        {
            history.InsertDate = DateTime.UtcNow;
            history.DoneDate = history.DoneDate != null ?
                                   history.DoneDate.Value.ToUniversalTime() :
                                   (DateTime?)null;
            history.DoneWorkDate = history.DoneWorkDate != null ?
                       history.DoneWorkDate.Value.ToUniversalTime() :
                       (DateTime?)null;
            if (history.TaskResult is DateTime)
                history.TaskResult = ((DateTime)history.TaskResult).ToUniversalTime();

            if (String.IsNullOrEmpty(history.TaskHistoryId)) // legacy, probably not necessary
                history.TaskHistoryId = Guid.NewGuid().ToString();

            var calendarTask = await this.CalendarTaskService.GetTaskAsync(history.CalendarTaskId);
            calendarTask.Histories.Add(history);
            await this.CalendarTaskService.UpdateTaskAsync(calendarTask);

            return history.TaskHistoryId;
        }
       
        public async Task<bool> UpdateHistoryAsync(ITaskHistory history)
        {
            try
            {
                var calendarTask = await this.CalendarTaskService.GetTaskAsync(history.CalendarTaskId);
                var historyIndex = calendarTask.Histories.FindIndex(p => p.TaskHistoryId == history.TaskHistoryId);
                calendarTask.Histories[historyIndex] = history;
                return await this.CalendarTaskService.UpdateTaskAsync(calendarTask);
            }
            catch (Exception ex)
            {
                // TODO: Throw exception instead of returning null and add an exceptoin handler on the controller
                Logger.Error("Error in UpdateHistoryAsync", ex);
                return false;
            }
        }
    }
}
