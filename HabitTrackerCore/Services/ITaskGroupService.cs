using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerCore.Services
{
    public interface ITaskGroupService
    {
        Task<string> InsertGroupAsync(TaskGroup group);
        Task<List<TaskGroup>> GetGroupsAsync(string userId,
                                             bool includeVoid = false);
        Task<bool> UpdateGroupAsync(TaskGroup group);
        Task<TaskGroup> GetGroupAsync(string groupId);
        Task<bool> DeleteGroupAsync(string groupId);
    }
}
