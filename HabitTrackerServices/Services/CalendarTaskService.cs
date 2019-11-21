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
    public class CalendarTaskService : ICalendarTaskService
    {
        /// <summary>
        /// Does not return Voided tasks
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<ICalendarTask>> GetTasksAsync(string userId, 
                                                             bool includeVoid = false)
        {
            try
            {
                return await getTasksAsync(userId, includeVoid);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetAsync", ex);
                return new List<ICalendarTask>();
            }
        }

        private async Task<List<ICalendarTask>> getTasksAsync(string userId, bool includeVoid)
        {
            Query query = getGetTasksQuery(userId, includeVoid);

            QuerySnapshot tasksQuerySnapshot = await query.GetSnapshotAsync();
            List<ICalendarTask> tasks = new List<ICalendarTask>();

            foreach (var document in tasksQuerySnapshot.Documents)
            {
                if (document.Exists)
                {
                    var newFireTask = document.ConvertTo<FireCalendarTask>();
                    var newTask = newFireTask.ToCalendarTask();
                    newTask.CalendarTaskId = document.Id;
                    tasks.Add(newTask);
                }
            }
            return tasks;
        }

        private static Query getGetTasksQuery(string userId, bool includeVoid)
        {
            return includeVoid ?
                                                FirestoreConnector.Instance.fireStoreDb
                                                                           .Collection("task_todo")
                                                                           .WhereEqualTo("UserId", userId) :
                                                FirestoreConnector.Instance.fireStoreDb
                                                                           .Collection("task_todo")
                                                                           .WhereEqualTo("UserId", userId)
                                                                           .WhereEqualTo("Void", false);
        }

        public async Task<string> InsertTaskAsync(ICalendarTask task)
        {
            try
            {
                return await insertTaskAsync(task);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in InsertTaskAsync", ex);
                return null;
            }
        }

        private async Task<string> insertTaskAsync(ICalendarTask task)
        {
            await ReorderTasks(task);

            task.InsertDate = DateTime.UtcNow;

            CollectionReference colRef = FirestoreConnector.Instance.fireStoreDb.Collection("task_todo");
            var reference = await colRef.AddAsync(new FireCalendarTask(task));

            return reference.Id;
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
                Logger.Error("Error in ReorderTasks", ex);
                return false;
            }
        }

        private async Task reorderTasks(ICalendarTask task)
        {
            int difference = task.AbsolutePosition - task.InitialAbsolutePosition;

            var tasks = await GetTasksAsync(task.UserId);

            foreach (var currentTask in tasks.Where(p => p.AbsolutePosition.IsBetween(task.AbsolutePosition,
                                                                                      task.InitialAbsolutePosition) &&
                                                         !p.Void && p.CalendarTaskId != task.CalendarTaskId))
            {
                currentTask.AbsolutePosition = difference < 0 ?
                                                currentTask.AbsolutePosition + 1 :
                                                currentTask.AbsolutePosition - 1;

                await UpdateTaskAsyncNoPositionCheck(currentTask);
            }
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
                Logger.Error("Error in UpdateTaskAsyncNoPositionCheck", ex);
                return false;
            }
        }

        private async Task updateTaskAsyncNoPositionCheck(ICalendarTask task)
        {
            task.UpdateDate = DateTime.UtcNow;

            if (task.HasBeenVoided())
                task.VoidDate = DateTime.UtcNow;

            DocumentReference taskRef = FirestoreConnector.Instance.fireStoreDb
                                                                   .Collection("task_todo")
                                                                   .Document(task.CalendarTaskId);

            var dictionnary = task.ToDictionary();

            await taskRef.UpdateAsync(dictionnary);
        }

        public async Task<bool> UpdateTaskAsync(ICalendarTask task)
        {
            try
            {
                return await updateTaskAsync(task);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in UpdateTaskAsyncNoPositionCheck", ex);
                return false;
            }
        }

        private async Task<bool> updateTaskAsync(ICalendarTask task)
        {
            if (task.HasBeenVoided())
            {
                task.AbsolutePosition = TaskPosition.MaxValue;
            }

            if (task.PositionHasBeenModified())
                await this.ReorderTasks(task);

            return await UpdateTaskAsyncNoPositionCheck(task);
        }

        public async Task<ICalendarTask> GetTaskAsync(string calendarTaskId)
        {
            try
            {
                var reference = FirestoreConnector.Instance.fireStoreDb
                                                           .Collection("task_todo")
                                                           .Document(calendarTaskId);

                var snapshot = await reference.GetSnapshotAsync();

                var task = snapshot.ConvertTo<FireCalendarTask>();
                task.CalendarTaskId = snapshot.Id;

                return task.ToCalendarTask();
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetAsync", ex);
                return null;
            }
        }


        public async Task<bool> DeleteTaskAsync(string calendarTaskId)
        {
            try
            {
                DocumentReference taskRef = FirestoreConnector.Instance.fireStoreDb
                                                                       .Collection("task_todo")
                                                                       .Document(calendarTaskId);

                await taskRef.DeleteAsync();

                //TODO: Delete the history (the delete history function has to be done first, or do we want to keep it and simply void the task to preserve the history ? or delete only if there is no history and void if there is one)

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
