using Google.Cloud.Firestore;
using HabitTrackerCore.DAL;
using HabitTrackerCore.Models;
using HabitTrackerCore.Utils;
using HabitTrackerServices.Models.Firestore;
using HabitTrackerTools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabitTrackerServices.DAL
{
    public class DALTaskHistory : IDALTaskHistory
    {
        private FirebaseConnector Connector { get; set; }

        public DALTaskHistory(FirebaseConnector connector)
        {
            this.Connector = connector;
        }

        public async Task<List<ITaskHistory>> GetHistoriesAsync(GetCalendarTaskRequest request)
        {
            Query query = getGetHistoriesQuery(request);

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

        private Query getGetHistoriesQuery(GetCalendarTaskRequest request)
        {
            var query = this.Connector.fireStoreDb.Collection("task_history")
                                                  .WhereEqualTo("UserId", request.UserId);

            if (!request.IncludeVoid)
                query = query.WhereEqualTo("Void", false);

            if (request.DateStart != null)
                query = query.WhereGreaterThanOrEqualTo("DoneDate", request.DateStart.Value.ToUniversalTime());

            if (request.DateEnd != null)
                query = query.WhereLessThanOrEqualTo("DoneDate", request.DateEnd.Value.ToUniversalTime());

            return query;
        }

        public async Task<ITaskHistory> GetHistoryAsync(string taskHistoryId)
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

        public async Task<bool> DeleteHistoryAsync(string taskHistoryId)
        {
            DocumentReference taskRef = this.Connector.fireStoreDb
                                                      .Collection("task_history")
                                                      .Document(taskHistoryId);

            await taskRef.DeleteAsync();

            return true;
        }

        public async Task<bool> UpdateHistoryAsync(ITaskHistory history)
        {
            DocumentReference taskRef = this.Connector.fireStoreDb
                                                      .Collection("task_history")
                                                      .Document(history.TaskHistoryId);

            var dictionnary = history.ToDictionary();
            await taskRef.UpdateAsync(dictionnary);

            return true;
        }

        public async Task<string> InsertHistoryAsync(ITaskHistory history)
        {
            CollectionReference colRef = this.Connector.fireStoreDb.Collection("task_history");
            var reference = await colRef.AddAsync(new FireTaskHistory(history));

            return reference.Id;
        }
    }
}
