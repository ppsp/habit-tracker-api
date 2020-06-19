using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerCore.Utils;
using HabitTrackerServices.Models.Firestore;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabitTrackerServices.Services
{
    public class TaskGroupService : ITaskGroupService
    {
        private const string table_name = "task_group";
        private FirebaseConnector Connector { get; set; }

        public TaskGroupService(FirebaseConnector connector)
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
                        newTaskGroup.Id = document.Id;

                        return newTaskGroup.ToTaskGroup();
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
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return new List<TaskGroup>();
            }
        }

        public async Task<string> InsertGroupAsync(TaskGroup group)
        {
            try
            {
                // Check if task already exists
                bool alreadyExists = await CheckIfExistsAsync(group.Id);
                if (alreadyExists)
                {
                    Logger.Error("Group already exists : " + group.Id);
                    return null;
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
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
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
            catch (Exception ex)
            {
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
                return false;
            }
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
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
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
                Logger.Error($"Error in {System.Reflection.MethodBase.GetCurrentMethod().Name}", ex);
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
    }
}
