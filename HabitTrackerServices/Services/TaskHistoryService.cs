using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerCore.Utils;
using HabitTrackerServices.Models.Firestore;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTrackerServices.Services
{
    public class TaskHistoryService : ITaskHistoryService
    {
        private FirebaseConnector Connector { get; set; }

        public TaskHistoryService(FirebaseConnector connector)
        {
            this.Connector = connector;
        }

        public async Task<List<ITaskHistory>> GetHistoriesAsync(string userId, 
                                                                bool includeVoid = false,
                                                                DateTime? dateStart = null,
                                                                DateTime? dateEnd = null)
        {
            try
            {
                return await getHistoriesAsync(userId, includeVoid, dateStart, dateEnd);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetAsync", ex);
                return new List<ITaskHistory>();
            }
        }

        private async Task<List<ITaskHistory>> getHistoriesAsync(string userId, 
                                                                 bool includeVoid,
                                                                 DateTime? dateStart = null,
                                                                 DateTime? dateEnd = null)
        {
            Query query = getGetHistoriesQuery(userId, includeVoid, dateStart, dateEnd);

            QuerySnapshot tasksQuerySnapshot = await query.GetSnapshotAsync();
            List<ITaskHistory> tasks = new List<ITaskHistory>();

            foreach (var document in tasksQuerySnapshot.Documents)
            {
                if (document.Exists)
                {
                    var task = document.ConvertTo<FireTaskHistory>();
                    var newTask = task.ToTaskHistory();
                    newTask.TaskHistoryId = document.Id;
                    tasks.Add(newTask);
                }
            }

            return tasks;
        }

        private Query getGetHistoriesQuery(string userId, 
                                                  bool includeVoid, 
                                                  DateTime? dateStart, 
                                                  DateTime? dateEnd)
        {
            if (dateStart == null)
                dateStart = DateTime.Today.ToUniversalTime();
            if (dateEnd == null)
                dateEnd = DateTime.Today.AddDays(1).ToUniversalTime();

            if (includeVoid)
            {
                return this.Connector.fireStoreDb
                                     .Collection("task_history")
                                     .WhereEqualTo("UserId", userId)
                                     .WhereGreaterThanOrEqualTo("DoneDate", dateStart.Value.ToUniversalTime())
                                     .WhereLessThanOrEqualTo("DoneDate", dateEnd.Value.ToUniversalTime());
            }
            else
            {
                return this.Connector.fireStoreDb
                                     .Collection("task_history")
                                     .WhereEqualTo("UserId", userId)
                                     .WhereEqualTo("Void", false)
                                     .WhereGreaterThanOrEqualTo("DoneDate", dateStart.Value.ToUniversalTime())
                                     .WhereLessThanOrEqualTo("DoneDate", dateEnd.Value.ToUniversalTime());
            }
        }

        public async Task<string> InsertHistoryAsync(ITaskHistory history)
        {
            try
            {
                return await insertHistoryAsync(history);
            }
            catch (Exception ex)
            {
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

            CollectionReference colRef = this.Connector.fireStoreDb.Collection("task_history");
            var reference = await colRef.AddAsync(new FireTaskHistory(history));

            return reference.Id;
        }

        public async Task<ITaskHistory> GetHistoryAsync(string taskHistoryId)
        {
            try
            {
                var reference = this.Connector.fireStoreDb
                                              .Collection("task_history")
                                              .Document(taskHistoryId);

                var snapshot = await reference.GetSnapshotAsync();

                var task = snapshot.ConvertTo<FireTaskHistory>();
                var newTask = task.ToTaskHistory();
                newTask.TaskHistoryId = snapshot.Id;

                return newTask;
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetAsync", ex);
                return null;
            }
        }

        public async Task<bool> DeleteHistoryAsync(string taskHistoryId)
        {
            try
            {
                DocumentReference taskRef = this.Connector.fireStoreDb
                                                          .Collection("task_history")
                                                          .Document(taskHistoryId);

                await taskRef.DeleteAsync();

                return true;
            }
            catch (Exception ex)
            {
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
                Logger.Error("Error in UpdateHistoryAsync", ex);
                return false;
            }
        }

        private async Task<bool> updateHistoryAsync(ITaskHistory history)
        {
            history.UpdateDate = DateTime.UtcNow;

            if (history.HasBeenVoided())
                history.VoidDate = DateTime.UtcNow;

            DocumentReference taskRef = this.Connector.fireStoreDb
                                                      .Collection("task_history")
                                                      .Document(history.TaskHistoryId);

            var dictionnary = history.ToDictionary();

            await taskRef.UpdateAsync(dictionnary);

            return true;
        }
    }
}
