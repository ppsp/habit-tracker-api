using HabitTrackerCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface ICalendarTaskService
    {
        Task<string> InsertTaskAsync(ICalendarTask task);
        Task<List<ICalendarTask>> GetTasksAsync(string userId, bool includeVoid = false);
        Task<bool> UpdateTaskAsync(ICalendarTask task);
        Task<ICalendarTask> GetTaskAsync(string calendarTaskId);
    }
}
