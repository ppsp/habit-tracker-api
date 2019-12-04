using HabitTrackerCore.DAL;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerServices.DAL;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTrackerServices.Services
{
    /*public class CachedTaskHistories
    {
        public List<TaskHistory> Histories { get; set; }
        public GetCalendarTaskRequest Request { get; set; }
    }*/

    public class TaskHistoryService : ITaskHistoryService
    {
        private IDALTaskHistory DALTaskHistory { get; set; }
        private CachingManager Caching { get; set; }

        public TaskHistoryService(IDALTaskHistory dalTaskHistory, 
                                  CachingManager cachingManager)
        {
            this.DALTaskHistory = dalTaskHistory;
            this.Caching = cachingManager;
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
            if (request.DateStart == null)
                request.DateStart = DateTime.Today.ToUniversalTime();
            if (request.DateEnd == null)
                request.DateEnd = DateTime.Today.AddDays(1).ToUniversalTime();

            // TODO: Check if parameters are the same as when cached
            /*string key = getCachingKey(userId);
            List<TaskHistory> cachedHistories = (List<TaskHistory>)Caching.TryGet(key);

            if (cachedHistories != null)
            {
                List<ITaskHistory> histories = new List<ITaskHistory>();
                cachedHistories.Clone().ForEach(p => histories.Add(p));

                return histories;
            }*/

            var histories = await this.DALTaskHistory.GetHistoriesAsync(request);

            //addToCache(histories.Clone());

            return histories;
        }

        /*private void addToCache(List<TaskHistory> newTask)
        {
            if (newTask == null || newTask.Count == 0 || newTask[0].UserId == null || newTask[0].UserId.Length == 0)
                return;

            string cachingKey = getCachingKey(newTask);
            this.Caching.TrySet(cachingKey,
                                newTask,
                                null,
                                newTask.Count);
        }

        private string getCachingKey(List<TaskHistory> newTask)
        {
            if (newTask == null || newTask.Count == 0 || newTask[0].UserId == null || newTask[0].UserId.Length == 0)
                throw new ArgumentException("getCachingKey, newTask is invalid");
                
            return $"taskHistory{newTask[0].UserId}";
        }
        private string getCachingKey(string userId)
        {
            if (userId == null || userId.Length == 0)
                throw new ArgumentException("getCachingKey, userId is invalid");

            return $"taskHistory{userId}";
        }*/

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
