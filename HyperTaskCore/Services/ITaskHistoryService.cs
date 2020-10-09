using HyperTaskCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyperTaskCore.Services
{
    public interface ITaskHistoryService
    {
        Task<string> InsertHistoryAsync(ITaskHistory history);
        Task<bool> UpdateHistoryAsync(ITaskHistory history);
    }
}
