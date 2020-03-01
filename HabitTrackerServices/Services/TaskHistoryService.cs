using HabitTrackerCore.DAL;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerServices.Caching;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTrackerServices.Services
{
    public class TaskHistoryService : ITaskHistoryService
    {
        private IDALTaskHistory DALTaskHistory { get; set; }

        public TaskHistoryService(IDALTaskHistory dalTaskHistory)
        {
            this.DALTaskHistory = dalTaskHistory;
        }

        public async Task<List<ITaskHistory>> GetHistoriesAsync(GetCalendarTaskRequest request)
        {
            try
            {
                return await getHistoriesAsync(request);
            }
            catch (Exception ex)
            {
                // TODO: Throw exception instead of returning null and add an exceptoin handler on the controller
                Logger.Error("Error in GetHistoriesAsync", ex);
                return new List<ITaskHistory>();
            }
        }

        private async Task<List<ITaskHistory>> getHistoriesAsync(GetCalendarTaskRequest request)
        {
            SetDefaultDateValues(request);

            var historiesFromDatabase = await this.DALTaskHistory.GetHistoriesAsync(request);

            //this.TaskHistoryCache.AddToCache(new CachedTaskHistories(request, historiesFromDatabase));

            return historiesFromDatabase;
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
            history.DoneDate = history.DoneDate.ToUniversalTime();
            if (history.TaskResult is DateTime)
                history.TaskResult = ((DateTime)history.TaskResult).ToUniversalTime();

            var taskHistoryId = await DALTaskHistory.InsertHistoryAsync(history);

            history.TaskHistoryId = taskHistoryId;

            //this.addToCache(history);

            return taskHistoryId;
        }

        public async Task<ITaskHistory> GetHistoryAsync(string taskHistoryId)
        {
            try
            {
                // TODO: get from cache

                var history = await this.DALTaskHistory.GetHistoryAsync(taskHistoryId);

                return history;
            }
            catch (Exception ex)
            {
                // TODO: Throw exception instead of returning null and add an exceptoin handler on the controller
                Logger.Error("Error in GetAsync", ex);
                return null;
            }
        }

        public async Task<bool> DeleteHistoryAsync(string taskHistoryId)
        {
            try
            {
                // TODO: Delete from cache

                var result = await this.DALTaskHistory.DeleteHistoryAsync(taskHistoryId);

                return result;
            }
            catch (Exception ex)
            {
                // TODO: Throw exception instead of returning null and add an exceptoin handler on the controller
                Logger.Error("Error in DeleteTaskAsync", ex);
                return false;
            }
        }

        public async Task<bool> UpdateHistoryAsync(ITaskHistory history)
        {
            try
            {
                return await updateHistoryAsync(history);
            }
            catch (Exception ex)
            {
                // TODO: Throw exception instead of returning null and add an exceptoin handler on the controller
                Logger.Error("Error in UpdateHistoryAsync", ex);
                return false;
            }
        }

        private async Task<bool> updateHistoryAsync(ITaskHistory history)
        {
            history.UpdateDate = DateTime.UtcNow;

            if (history.HasBeenVoided())
                history.VoidDate = DateTime.UtcNow;

            var response = await this.DALTaskHistory.UpdateHistoryAsync(history);

            //addToCache(history);

            return response;
        }
    }
}
