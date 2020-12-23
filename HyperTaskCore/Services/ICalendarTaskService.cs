using HyperTaskCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyperTaskCore.Services
{
    public interface ICalendarTaskService
    {
        Task<string> InsertTaskAsync(ICalendarTask task);
        Task<List<ICalendarTask>> GetTasksAsync(string userId, 
                                                bool includeVoid = false, 
                                                int? firstPosition = null,
                                                int? lastPosition = null,
                                                string groupId = null,
                                                bool includeOnceDone = true);
        Task<bool> UpdateTaskAsync(ICalendarTask task);
        Task<ICalendarTask> GetTaskAsync(string calendarTaskId);
    }
}
