using HyperTaskCore.Exceptions;
using HyperTaskCore.Models;
using HyperTaskCore.Services;
using HyperTaskCore.Utils;
using HyperTaskServices.Models.Mongo;
using HyperTaskTools;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperTaskServices.Services
{
    public class MongoCalendarTaskService : ICalendarTaskService
    {
        private readonly string DBHyperTask = "hypertask";
        private readonly string CollectionTasks = "task_todo";
        private MongoConnector Connector { get; set; }

        public MongoCalendarTaskService(MongoConnector connector)
        {
            this.Connector = connector;
        }

        /// <summary>
        /// Does not return Voided tasks
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<ICalendarTask>> GetTasksAsync(string userId, 
                                                             bool includeVoid = false,
                                                             int? firstPosition = null,
                                                             int? lastPosition = null,
                                                             string groupId = null,
                                                             bool includeOnceDone = true)
        {
            try
            {
                return await getTasksAsync(userId, includeVoid, firstPosition, lastPosition, groupId, includeOnceDone);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetAsync", ex);
                return new List<ICalendarTask>();
            }
        }

        private async Task<List<ICalendarTask>> getTasksAsync(string userId, 
                                                              bool includeVoid,
                                                              int? firstPosition,
                                                              int? lastPosition,
                                                              string groupId,
                                                              bool includeOnceDone = true)
        {
            var query = await getGetTasksQuery(userId, includeVoid, firstPosition, lastPosition, groupId);
            try
            {
                var tasksQuerySnapshot = query.ToList();
                List<ICalendarTask> tasks = new List<ICalendarTask>();

                foreach (var document in tasksQuerySnapshot)
                {
                    if (document != null)
                    {
                        var newTask = document.ToCalendarTask();
                        
                        tasks.Add(newTask);
                    }
                }

                if (!includeOnceDone)
                    return tasks.Where(p => p.DoneDate == null).ToList();
                else
                    return tasks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<IAsyncCursor<MongoCalendarTask>> getGetTasksQuery(string userId, 
                                                                                   bool includeVoid,
                                                                                   int? firstPosition,
                                                                                   int? lastPosition,
                                                                                   string groupId)
        {
            var filter = Builders<MongoCalendarTask>.Filter.Eq(p => p.UserId, userId);

            if (!includeVoid)
                filter = filter & Builders<MongoCalendarTask>.Filter.Eq(p => p.Void, false);

            if (firstPosition != null)
                filter = filter & Builders<MongoCalendarTask>.Filter.Gte(p => p.AbsolutePosition, firstPosition.Value);

            if (lastPosition != null)
                filter = filter & Builders<MongoCalendarTask>.Filter.Lte(p => p.AbsolutePosition, lastPosition.Value);

            if (groupId != null)
                filter = filter & Builders<MongoCalendarTask>.Filter.Eq(p => p.GroupId, groupId);

            var query = await this.Connector.mongoClient
                                            .GetDatabase(DBHyperTask)
                                            .GetCollection<MongoCalendarTask>(CollectionTasks)
                                            .FindAsync(filter);

            Logger.Debug($"Request Units in getGetTasksQuery = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");

            return query;
        }

        public async Task<string> InsertTaskAsync(ICalendarTask task)
        {
            try
            {
                var dateStart = DateTime.Now;
                // Check if AbsolutePosition already exists
                var existingTasks = await getTasksAsync(task.UserId, false, task.AbsolutePosition, task.AbsolutePosition, task.GroupId);
                Logger.Debug("Got task seconds " + (DateTime.Now - dateStart).TotalSeconds);
                if (existingTasks.Count > 0)
                {
                    await reorderTasks(task);
                    Logger.Debug("Reordered seconds " + (DateTime.Now - dateStart).TotalSeconds);
                }

                var result = await insertTaskAsync(task);
                Logger.Debug("inserted task seconds " + (DateTime.Now - dateStart).TotalSeconds);
                return result;

            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return null;
            }
        }

        private async Task<string> insertTaskAsync(ICalendarTask task)
        {
            if (task.InsertDate == null)
                task.InsertDate = DateTime.UtcNow;

            var mongoTask = new MongoCalendarTask(task);
            await this.Connector.mongoClient
                                .GetDatabase(DBHyperTask)
                                .GetCollection<MongoCalendarTask>(CollectionTasks)
                                .InsertOneAsync(mongoTask);

            Logger.Debug($"Request Units in insertTaskAsync = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");

            task.Id = mongoTask.Id;

            return mongoTask.Id;
        }

        private static void GuardAgainstInvalidTask(ICalendarTask task)
        {
            if (task.Name == null || task.Name.Length == 0)
                throw new InvalidCalendarTaskException("Name is invalid");

            if (!task.AbsolutePosition.IsBetween(0, 500))
                throw new InvalidCalendarTaskException("Position must be between 0 and 500");

            if (task.Frequency.In(eTaskFrequency.Once, eTaskFrequency.UntilDone) && task.AssignedDate == null)
                throw new InvalidCalendarTaskException("Assigned date can't be null");
        }

        public async Task<bool> ReorderTasks(ICalendarTask task)
        {
            try
            {
                await reorderTasks(task);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task reorderTasks(ICalendarTask task)
        {
            int difference = task.AbsolutePosition - task.InitialAbsolutePosition;
            int lowest = Math.Min(task.AbsolutePosition, task.InitialAbsolutePosition);
            int highest = Math.Max(task.AbsolutePosition, task.InitialAbsolutePosition);

            var tasks = await GetTasksAsync(task.UserId,
                                            false,
                                            lowest,
                                            highest,
                                            task.GroupId,
                                            false);

            if (tasks.Count > Math.Abs(difference) + 1 || tasks.GroupBy(p => p.AbsolutePosition).Any(p => p.Count() > 1)) // reorder all if 2 are the same
            {
                Logger.Debug("Update all tasks" + task.CalendarTaskId + " " + task.UserId);

                await reorderAllTasks(task);
            }
            else
            {
                Logger.Debug("Update NOT all tasks" + task.CalendarTaskId + " " + task.UserId);

                // reorder only between current and new Id
                foreach (var currentTask in tasks.Where(p => p.GroupId == task.GroupId &&
                                                             p.AbsolutePosition.IsBetween(task.AbsolutePosition,
                                                                                          task.InitialAbsolutePosition) &&
                                                             !p.Void &&
                                                             p.CalendarTaskId != task.CalendarTaskId))
                {
                    currentTask.AbsolutePosition = difference < 0 ?
                                                    currentTask.AbsolutePosition + 1 :
                                                    currentTask.AbsolutePosition - 1;

                    await UpdateTaskAsyncNoPositionCheck(currentTask);
                }
            }
        }

        private async Task reorderAllTasks(ICalendarTask task)
        {
            var tasks = await GetTasksAsync(task.UserId,
                                            false);

            tasks = tasks.Where(p => p.GroupId == task.GroupId &&
                                     !p.Void &&
                                     p.CalendarTaskId != task.CalendarTaskId &&
                                     (IsPresentOrFuture(p)))
                         .OrderBy(p => p.AbsolutePosition)
                         .ToList();

            int positionIterator = 1;
            foreach (var currentTask in tasks)
            {
                if (positionIterator == task.AbsolutePosition)
                    positionIterator++;

                currentTask.AbsolutePosition = positionIterator++;

                await UpdateTaskAsyncNoPositionCheck(currentTask);
            }
        }

        private static bool IsPresentOrFuture(ICalendarTask p)
        {
            return p.Frequency.NotIn(eTaskFrequency.Once, eTaskFrequency.UntilDone) ||
                                     !p.Histories.Any(p => p.TaskDone);
        }

        private async Task<bool> UpdateTaskAsyncNoPositionCheck(ICalendarTask task)
        {
            try
            {
                await updateTaskAsyncNoPositionCheck(task);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task updateTaskAsyncNoPositionCheck(ICalendarTask task)
        {
            // task.UpdateDate = DateTime.UtcNow;

            if (task.HasBeenVoided())
                task.VoidDate = DateTime.UtcNow;

            if (task.AssignedDate != null)
                task.AssignedDate = task.AssignedDate.Value.ToUniversalTime();

            var filter = Builders<MongoCalendarTask>.Filter.Eq(p => p.CalendarTaskId, task.CalendarTaskId);
            var query = await this.Connector.mongoClient
                                            .GetDatabase(DBHyperTask)
                                            .GetCollection<MongoCalendarTask>(CollectionTasks)
                                            .FindAsync<MongoCalendarTask>(filter);

            Logger.Debug($"Request Units in updateTaskAsyncNoPositionCheck = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");

            var allDocuments = query.ToList();

            // Should not occur but just in case, we delete duplicate ids
            if (allDocuments.Count > 1)
            {
                await deleteDuplicates(allDocuments, task.CalendarTaskId);
            }

            var firstDocument = allDocuments.SingleOrDefault();

            if (firstDocument != null)
            {
                var mongoTask = new MongoCalendarTask(task);
                mongoTask.Id = firstDocument.Id;

                // TODO: Find a solution for too many request units
                if (task.Histories != null && task.Histories.Count > 90)
                {
                    Logger.Info("More than 90 histories, deleting :" + task.CalendarTaskId);
                    task.Histories.RemoveAll(p => p.InsertDate < DateTime.Now.AddDays(-90));
                }

                await TryReplace(filter, mongoTask);

                Logger.Debug($"Request Units in updateTaskAsyncNoPositionCheck2 = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");
            }
        }

        private async Task<ReplaceOneResult> TryReplace(FilterDefinition<MongoCalendarTask> filter, MongoCalendarTask mongoTask)
        {
            int retry = 0;
            while (retry < 6)
            {
                try
                {
                    var result = await this.Connector.mongoClient
                                           .GetDatabase(DBHyperTask)
                                           .GetCollection<MongoCalendarTask>(CollectionTasks)
                                           .ReplaceOneAsync(filter, mongoTask);

                    return result;
                }
                catch (MongoCommandException ex)
                {
                    if (ex.Code == 16500)
                    {
                        if (retry > 4)
                            throw ex;

                        Logger.Warn("Too many requests, retrying in 2 seconds", ex);
                        retry++;

                        Thread.Sleep(2000);
                    }
                }
            }

            throw new Exception("Too many requests");
        }

        private async Task deleteDuplicates(List<MongoCalendarTask> allDocuments, string logText)
        {
            Logger.Warn("DUPLICATE DOCUMENT WHEN UPDATING, DELETING EXTRA, CalendarTaskId" + logText);

            var toDeletes = allDocuments.OrderBy(p => p.InsertDate).Skip(1);

            foreach (var toDelete in toDeletes)
            {
                await DeleteTaskWithFireBaseIdAsync(toDelete.Id);
            }
        }

        public async Task<bool> UpdateTaskAsync(ICalendarTask task)
        {
            try
            {
                if (task.Histories == null || task.Histories.Count == 0)
                {
                    var latestTask = await GetTaskAsync(task.CalendarTaskId);
                    task.Histories = latestTask.Histories;
                }    

                return await updateTaskAsync(task);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task<bool> updateTaskAsync(ICalendarTask task)
        {
            if (task.HasBeenVoided())
            {
                task.AbsolutePosition = TaskPosition.MaxValue;
            }
            else if (task.PositionHasBeenModified())
            {
                await this.ReorderTasks(task);
            }

            return await UpdateTaskAsyncNoPositionCheck(task);
        }

        // For retrocompatibility
        private async Task<ICalendarTask> GetTaskAsyncCustomId(string Id)
        {
            try
            {
                var filter = Builders<MongoCalendarTask>.Filter.Eq(p => p.Id, Id);
                var tasks = await this.Connector.mongoClient
                                                .GetDatabase(DBHyperTask)
                                                .GetCollection<MongoCalendarTask>(CollectionTasks)
                                                .FindAsync(filter);

                Logger.Debug($"Request Units in GetTaskAsyncCustomId = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");

                return tasks.ToList().FirstOrDefault().ToCalendarTask();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return null;
            }
        }

        public async Task<ICalendarTask> GetTaskAsync(string calendarTaskId)
        {
            try
            {
                var filter = Builders<MongoCalendarTask>.Filter.Eq(p => p.CalendarTaskId, calendarTaskId);
                var tasksQuery = await this.Connector.mongoClient
                                                     .GetDatabase(DBHyperTask)
                                                     .GetCollection<MongoCalendarTask>(CollectionTasks)
                                                     .FindAsync<MongoCalendarTask>(filter);

                Logger.Debug($"Request Units in GetTaskAsync = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");

                var tasks = tasksQuery.ToList();

                try
                {
                    // Should not occur but just in case, we delete duplicate ids
                    if (tasks.Count > 1)
                    {
                        await deleteDuplicates(tasks, calendarTaskId);
                    }

                    foreach (var document in tasks)
                    {
                        if (document != null)
                        {
                            var newTask = document.ToCalendarTask();

                            // for retrocompatibility 2020-04-19
                            if (String.IsNullOrEmpty(newTask.CalendarTaskId))
                                newTask.CalendarTaskId = document.Id;

                            return newTask;
                        }
                    }

                    // return null;
                    return await this.GetTaskAsyncCustomId(calendarTaskId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return null;
            }
        }

        public async Task<bool> CheckIfExistsAsync(string calendarTaskId)
        {
            try
            {
                var filter = Builders<MongoCalendarTask>.Filter.Eq(p => p.CalendarTaskId, calendarTaskId);
                var tasksQuery = await this.Connector.mongoClient
                                                     .GetDatabase(DBHyperTask)
                                                     .GetCollection<MongoCalendarTask>(CollectionTasks)
                                                     .FindAsync<MongoCalendarTask>(filter);

                Logger.Debug($"Request Units in CheckIfExistsAsync = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");

                var tasks = tasksQuery.ToList();

                try
                {
                    foreach (var document in tasks)
                    {
                        if (document != null)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                throw ex;
            }
        }

        public async Task<bool> DeleteTaskAsync(string calendarTaskId)
        {
            try
            {
                var filter = Builders<MongoCalendarTask>.Filter.Eq(p => p.CalendarTaskId, calendarTaskId);
                var deleteResult = await this.Connector.mongoClient
                                                       .GetDatabase(DBHyperTask)
                                                       .GetCollection<MongoCalendarTask>(CollectionTasks)
                                                       .DeleteOneAsync(filter);

                Logger.Debug($"Request Units in DeleteTaskAsync = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");

                return deleteResult.DeletedCount == 1;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        public async Task<bool> DeleteTaskWithFireBaseIdAsync(string fireBaseId)
        {
            try
            {
                var filter = Builders<MongoCalendarTask>.Filter.Eq(p => p.Id, fireBaseId);
                var deleteResult = await this.Connector.mongoClient
                                                       .GetDatabase(DBHyperTask)
                                                       .GetCollection<MongoCalendarTask>(CollectionTasks)
                                                       .DeleteOneAsync(filter);

                Logger.Debug($"Request Units in DeleteTaskWithFireBaseIdAsync = {this.Connector.GetLatestRequestCharge(DBHyperTask)}");

                return deleteResult.DeletedCount == 1;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }
    }
}
