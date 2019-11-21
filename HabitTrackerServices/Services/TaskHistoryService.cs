﻿using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerServices.Models.Firestore;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTrackerServices.Services
{
    public class TaskHistoryService : ITaskHistoryService
    {
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

        private static Query getGetHistoriesQuery(string userId, 
                                                  bool includeVoid, 
                                                  DateTime? dateStart, 
                                                  DateTime? dateEnd)
        {
            if (dateStart == null)
                dateStart = DateTime.Today;
            if (dateEnd == null)
                dateEnd = DateTime.Today.AddDays(1);

            if (includeVoid)
            {
                return FirestoreConnector.Instance.fireStoreDb
                                                  .Collection("task_history")
                                                  .WhereEqualTo("UserId", userId)
                                                  .WhereGreaterThanOrEqualTo("DoneDate", dateStart.Value.ToUniversalTime())
                                                  .WhereLessThanOrEqualTo("DoneDate", dateEnd.Value.ToUniversalTime());
            }
            else
            {
                return FirestoreConnector.Instance.fireStoreDb
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

            CollectionReference colRef = FirestoreConnector.Instance.fireStoreDb.Collection("task_history");
            var reference = await colRef.AddAsync(new FireTaskHistory(history));

            return reference.Id;
        }

        public async Task<ITaskHistory> GetHistoryAsync(string taskHistoryId)
        {
            try
            {
                var reference = FirestoreConnector.Instance.fireStoreDb
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

        public async Task<bool> DeleteTaskAsync(string taskHistoryId)
        {
            try
            {
                DocumentReference taskRef = FirestoreConnector.Instance.fireStoreDb
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
    }
}
