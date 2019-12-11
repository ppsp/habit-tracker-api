using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerCore.DAL
{
    /// <summary>
    /// This class implements the data access on the database.
    /// In order to satisfy the Single Responsibility Principle,
    /// there are no try/catches, no caching, and no data modification (like insert date)
    /// whatsoever, those are all in the service layer.
    /// </summary>
    public interface IDALTaskHistory
    {
        Task<bool> DeleteHistoryAsync(string taskHistoryId);
        Task<List<ITaskHistory>> GetHistoriesAsync(GetCalendarTaskRequest request);
        Task<ITaskHistory> GetHistoryAsync(string taskHistoryId);
        Task<string> InsertHistoryAsync(ITaskHistory history);
        Task<bool> UpdateHistoryAsync(ITaskHistory history);
    }
}
