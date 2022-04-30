using Google.Cloud.Firestore;
using HyperTaskCore.Models;
using HyperTaskCore.Services;
using HyperTaskCore.Utils;
using HyperTaskServices.Models.Firestore;
using HyperTaskTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperTaskServices.Services
{
    public class FireTaskGroupService : ITaskGroupService
    {
        private const string table_name = "task_group";
        private FirebaseConnector Connector { get; set; }

        public FireTaskGroupService(FirebaseConnector connector)
        {
            this.Connector = connector;
        }

        public async Task<TaskGroup> GetGroupAsync(string groupId)
        {
            try
            {
                Query query = this.Connector.fireStoreDb
                                            .Collection(table_name)
                                            .WhereEqualTo("GroupId", groupId);

                QuerySnapshot tasksQuerySnapshot = await query.GetSnapshotAsync();

                foreach (var document in tasksQuerySnapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var newTaskGroup = document.ConvertTo<FireTaskGroup>();
                        var taskGroup = newTaskGroup.ToTaskGroup();
                        taskGroup.Id = document.Id;

                        return newTaskGroup.ToTaskGroup();
                    }
                }

                return new TaskGroup();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetGroupAsync", ex);
                return new TaskGroup();
            }
        }

        public async Task<List<TaskGroup>> GetGroupsAsync(string userId, bool includeVoid = false)
        {
            try
            {
                Query query = this.Connector.fireStoreDb
                                            .Collection(table_name)
                                            .WhereEqualTo("UserId", userId);

                if (!includeVoid)
                    query = query.WhereEqualTo("Void", false);

                QuerySnapshot tasksQuerySnapshot = await query.GetSnapshotAsync();
                List<TaskGroup> groups = new List<TaskGroup>();

                foreach (var document in tasksQuerySnapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var newFireTaskGroup = document.ConvertTo<FireTaskGroup>();
                        var newTask = newFireTaskGroup.ToTaskGroup();
                        newTask.Id = document.Id;
                        
                        groups.Add(newTask);
                    }
                }

                return groups;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetGroupsAsync", ex);
                return new List<TaskGroup>();
            }
        }

        public async Task<string> InsertGroupAsync(TaskGroup group)
        {
            try
            {
                // Check if task already exists
                bool alreadyExists = await CheckIfExistsAsync(group.GroupId);
                if (alreadyExists)
                {
                    Logger.Error("Group already exists : " + group.GroupId);
                    return null;
                }

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
                Logger.Error($"Error in InsertGroupAsync", ex);
                return null;
            }
        }

        private async Task<bool> CheckIfExistsAsync(string groupId)
        {
            try
            {
                Query query = this.Connector.fireStoreDb
                                            .Collection(table_name)
                                            .WhereEqualTo("GroupId", groupId);

                try
                {
                    QuerySnapshot tasksQuerySnapshot = await query.GetSnapshotAsync();

                    foreach (var document in tasksQuerySnapshot.Documents)
                    {
                        if (document.Exists)
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
                Logger.Error($"Error in CheckIfExistsAsync", ex);
                throw ex;
            }
        }

        private async Task<string> insertGroupAsync(TaskGroup group)
        {
            if (group.InsertDate == null)
                group.InsertDate = DateTime.UtcNow;

            CollectionReference colRef = this.Connector.fireStoreDb.Collection(table_name);
            var reference = await colRef.AddAsync(FireTaskGroup.FromTaskGroup(group));

            return reference.Id;
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

                return await updateGroupNoPositionCheckAsync(group);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in UpdateGroupAsync", ex);
                return false;
            }
        }

        private async Task<bool> updateGroupNoPositionCheckAsync(TaskGroup group)
        {
            Query query = this.Connector.fireStoreDb
                                        .Collection(table_name)
                                        .WhereEqualTo("GroupId", group.GroupId);

            var allDocuments = (await query.GetSnapshotAsync()).Documents;

            // Should not occur but just in case, we delete duplicate ids
            if (allDocuments.Count > 1)
            {
                await deleteDuplicates(allDocuments, group);
                allDocuments = new List<DocumentSnapshot>() { allDocuments[0] };
            }

            var firstDocument = allDocuments.SingleOrDefault();

            if (firstDocument != null && firstDocument.Exists)
            {
                var dictionnary = FireTaskGroup.FromTaskGroup(group).ToDictionary();

                await firstDocument.Reference.UpdateAsync(dictionnary);

                return true;
            }

            return false;
        }

        public async Task<bool> DeleteGroupAsync(string groupId)
        {
            try
            {
                Query query = this.Connector.fireStoreDb
                                            .Collection(table_name)
                                            .WhereEqualTo("GroupId", groupId);

                var firstDocument = (await query.GetSnapshotAsync()).Documents.FirstOrDefault();

                if (firstDocument != null && firstDocument.Exists)
                {
                    await firstDocument.Reference.DeleteAsync();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in DeleteGroupAsync", ex);
                return false;
            }
        }

        public async Task<bool> DeleteGroupWithFireBaseIdAsync(string firebaseGroupId)
        {
            try
            {
                var firstDocument = this.Connector.fireStoreDb
                                        .Collection(table_name)
                                        .Document(firebaseGroupId);

                if (firstDocument != null)
                {
                    await firstDocument.DeleteAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in DeleteGroupWithFireBaseIdAsync", ex);
                return false;
            }
        }

        private async Task deleteDuplicates(IReadOnlyList<DocumentSnapshot> allDocuments, TaskGroup group)
        {
            Logger.Warn("DUPLICATE DOCUMENT WHEN UPDATING, DELETING EXTRA, GroupId" + group.GroupId);

            var toDeletes = allDocuments.OrderBy(p => p.CreateTime).Skip(1);

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
                Logger.Error($"Error in ReorderGroups", ex);
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
