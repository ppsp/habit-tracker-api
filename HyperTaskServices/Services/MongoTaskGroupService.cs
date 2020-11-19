using Google.Cloud.Firestore;
using HyperTaskCore.Models;
using HyperTaskCore.Services;
using HyperTaskCore.Utils;
using HyperTaskServices.Models.Firestore;
using HyperTaskServices.Models.Mongo;
using HyperTaskTools;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperTaskServices.Services
{
    public class MongoTaskGroupService : ITaskGroupService
    {
        private readonly string DBHyperTask = "hypertask";
        private const string CollectionGroups = "task_group";
        private MongoConnector Connector { get; set; }

        public MongoTaskGroupService(MongoConnector connector)
        {
            this.Connector = connector;
        }

        public async Task<TaskGroup> GetGroupAsync(string groupId)
        {
            try
            {
                var filter = Builders<MongoTaskGroup>.Filter.Eq(p => p.GroupId, groupId);
                var tasksQuery = await this.Connector.mongoClient
                                                     .GetDatabase(DBHyperTask)
                                                     .GetCollection<MongoTaskGroup>(CollectionGroups)
                                                     .FindAsync<MongoTaskGroup>(filter);
                var tasks = tasksQuery.ToList();

                foreach (var document in tasks)
                {
                    if (document != null)
                    {
                        var taskGroup = document.ToTaskGroup();
                        taskGroup.Id = document.Id;

                        return taskGroup;
                    }
                }

                return new TaskGroup();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return new TaskGroup();
            }
        }

        public async Task<List<TaskGroup>> GetGroupsAsync(string userId, bool includeVoid = false)
        {
            try
            {
                var filter = Builders<MongoTaskGroup>.Filter.Eq(p => p.UserId, userId);

                if (!includeVoid)
                    filter = filter & Builders<MongoTaskGroup>.Filter.Eq(p => p.Void, false);

                var groupsQuery = await this.Connector.mongoClient
                                                     .GetDatabase(DBHyperTask)
                                                     .GetCollection<MongoTaskGroup>(CollectionGroups)
                                                     .FindAsync<MongoTaskGroup>(filter);

                List<TaskGroup> groups = groupsQuery.ToList()
                                                    .Select(p => p.ToTaskGroup())
                                                    .ToList();

                return groups;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return new List<TaskGroup>();
            }
        }

        public async Task<string> InsertGroupAsync(TaskGroup group)
        {
            try
            {
                // Check if Position already exists
                var existingGroups = await GetGroupsAsync(group.UserId,
                                                          false); /*, task.AbsolutePosition, task.AbsolutePosition);*/ // TODO : Add these parameters
                if (existingGroups.Count(p => p.Position == group.Position) > 0)
                {
                    await reorderGroups(group);
                }

                return await insertGroupAsync(group);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return null;
            }
        }

        private async Task<bool> CheckIfExistsAsync(string groupId)
        {
            try
            {
                var filter = Builders<MongoTaskGroup>.Filter.Eq(p => p.GroupId, groupId);
                var tasksQuery = await this.Connector.mongoClient
                                                     .GetDatabase(DBHyperTask)
                                                     .GetCollection<MongoTaskGroup>(CollectionGroups)
                                                     .FindAsync<MongoTaskGroup>(filter);

                try
                {
                    return tasksQuery.ToList().Count > 0;
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

        private async Task<string> insertGroupAsync(TaskGroup group)
        {
            if (group.InsertDate == null)
                group.InsertDate = DateTime.UtcNow;

            var mongoGroup = MongoTaskGroup.FromTaskGroup(group);
            await this.Connector.mongoClient
                                .GetDatabase(DBHyperTask)
                                .GetCollection<MongoTaskGroup>(CollectionGroups)
                                .InsertOneAsync(mongoGroup);
            group.Id = mongoGroup.Id;

            return mongoGroup.Id;
        }

        public async Task<bool> UpdateGroupAsync(TaskGroup group)
        {
            try
            {
                group.UpdateDate = DateTime.UtcNow;

                if (group.HasBeenVoided())
                    group.VoidDate = DateTime.UtcNow;
                else if (group.PositionHasBeenModified())
                    await this.ReorderGroups(group);

                await updateGroupNoPositionCheckAsync(group);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task updateGroupNoPositionCheckAsync(TaskGroup group)
        {
            var filter = Builders<MongoTaskGroup>.Filter.Eq(p => p.GroupId, group.GroupId);
            var groupQuery = await this.Connector.mongoClient
                                            .GetDatabase(DBHyperTask)
                                            .GetCollection<MongoTaskGroup>(CollectionGroups)
                                            .FindAsync<MongoTaskGroup>(filter);

            var groups = groupQuery.ToList();

            // Should not occur but just in case, we delete duplicate ids
            if (groups.Count > 1)
            {
                await deleteDuplicates(groups, group);
            }

            var firstDocument = groups.SingleOrDefault();

            if (firstDocument != null)
            {
                var mongoGroup = MongoTaskGroup.FromTaskGroup(group);

                var result = await this.Connector.mongoClient
                                                 .GetDatabase(DBHyperTask)
                                                 .GetCollection<MongoTaskGroup>(CollectionGroups)
                                                 .ReplaceOneAsync(filter, mongoGroup);
            }
        }

        public async Task<bool> DeleteGroupAsync(string groupId)
        {
            try
            {
                var filter = Builders<MongoTaskGroup>.Filter.Eq(p => p.GroupId, groupId);
                var deleteResult = await this.Connector.mongoClient
                                                       .GetDatabase(DBHyperTask)
                                                       .GetCollection<MongoTaskGroup>(CollectionGroups)
                                                       .DeleteOneAsync(filter);
                return deleteResult.DeletedCount == 1;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        public async Task<bool> DeleteGroupWithFireBaseIdAsync(string firebaseGroupId)
        {
            try
            {
                var filter = Builders<MongoCalendarTask>.Filter.Eq(p => p.Id, firebaseGroupId);
                var deleteResult = await this.Connector.mongoClient
                                                       .GetDatabase(DBHyperTask)
                                                       .GetCollection<MongoCalendarTask>(CollectionGroups)
                                                       .DeleteOneAsync(filter);
                return deleteResult.DeletedCount == 1;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task deleteDuplicates(List<MongoTaskGroup> groups, TaskGroup group)
        {
            Logger.Warn("DUPLICATE DOCUMENT WHEN UPDATING, DELETING EXTRA, GroupId" + group.GroupId);

            var toDeletes = groups.OrderBy(p => p.InsertDate).Skip(1);

            foreach (var toDelete in toDeletes)
            {
                await DeleteGroupWithFireBaseIdAsync(toDelete.Id);
            }
        }

        public async Task<bool> ReorderGroups(TaskGroup group)
        {
            try
            {
                await reorderGroups(group);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
        }

        private async Task reorderGroups(TaskGroup group)
        {
            int difference = group.Position - group.InitialPosition;
            int lowest = Math.Min(group.Position, group.InitialPosition);
            int highest = Math.Max(group.Position, group.InitialPosition);

            var groups = await GetGroupsAsync(group.UserId,
                                              false);
                                              /*lowest,
                                              highest,
                                              false);*/ // TODO : Add these parameters

            if (groups.Count > Math.Abs(difference) + 1 || groups.GroupBy(p => p.Position).Any(p => p.Count() > 1)) // reorder all if 2 are the same
            {
                Logger.Debug("Update all groups" + group.GroupId + " " + group.UserId);

                await reorderAllGroups(group);
            }
            else
            {
                Logger.Debug("Update NOT all tasks" + group.GroupId + " " + group.UserId);

                // reorder only between current and new Id
                foreach (var currentGroup in groups.Where(p => p.Position.IsBetween(group.Position,
                                                                                          group.InitialPosition) &&
                                                         !p.Void &&
                                                         p.GroupId != group.GroupId))
                {
                    currentGroup.Position = difference < 0 ?
                                                currentGroup.Position + 1 :
                                                currentGroup.Position - 1;

                    await updateGroupNoPositionCheckAsync(currentGroup);
                }
            }
        }

        private async Task reorderAllGroups(TaskGroup group)
        {
            var tasks = await GetGroupsAsync(group.UserId,
                                            false);

            tasks = tasks.Where(p => !p.Void &&
                                     p.GroupId != group.GroupId &&
                                     p.Void == false)
                         .OrderBy(p => p.Position)
                         .ToList();

            int positionIterator = 1;
            foreach (var currentTask in tasks)
            {
                if (positionIterator == group.Position)
                    positionIterator++;

                currentTask.Position = positionIterator++;

                await updateGroupNoPositionCheckAsync(currentTask);
            }
        }
    }
}
