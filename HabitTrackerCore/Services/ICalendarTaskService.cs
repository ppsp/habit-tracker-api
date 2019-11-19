using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface ICalendarTaskService
    {
        Task<bool> InsertTaskAsync(CalendarTask task);
        Task<List<CalendarTask>> GetAsync(string userId);
        Task<bool> UpdateTaskAsync(CalendarTask task, int? initialAbsolutePosition = null);
        Task<bool> DeleteTaskAsync(string taskId);
    }
}
