using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface ICalendarTaskHistoryService
    {
        Task<bool> InsertHistoryAsync(CalendarTaskHistory history);
        Task<List<CalendarTaskHistory>> GetAsync(string userId);
    }
}
