using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface ITaskHistoryService
    {
        Task<string> InsertHistoryAsync(ITaskHistory history);
        Task<bool> UpdateHistoryAsync(ITaskHistory history);
        Task<List<ITaskHistory>> GetHistoriesAsync(string userId, 
                                                   bool includeVoid = false,
                                                   DateTime? dayStart = null,
                                                   DateTime? dayEnd= null);
    }
}
