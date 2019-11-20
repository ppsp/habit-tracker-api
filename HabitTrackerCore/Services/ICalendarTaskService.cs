using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface ICalendarTaskService
    {
        Task<string> InsertTaskAsync(ICalendarTask task);
        Task<List<ICalendarTask>> GetAsync(string userId);
        Task<bool> UpdateTaskAsync(ICalendarTask task);
    }
}
