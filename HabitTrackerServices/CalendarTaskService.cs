using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerCore.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HabitTrackerTools;
using HabitTrackerServices.Models.DTO;
using System.Text.Json;
using Newtonsoft.Json;
using HabitTrackerServices.Models.Firestore;

namespace HabitTrackerServices
{
    public class CalendarTaskService : ICalendarTaskService
    {
        /// <summary>
        /// Does not return Voided tasks
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<ICalendarTask>> GetAsync(string userId)
        {
            try
            {
                return await getAsync(userId);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetAsync", ex);
                return new List<ICalendarTask>();
            }
        }

        private async Task<List<ICalendarTask>> getAsync(string userId)
        {
            Query taskCollection = FirestoreConnector.Instance.fireStoreDb
                                                                              .Collection("task_todo")
                                                                              .WhereEqualTo("UserId", userId)
                                                                              .WhereEqualTo("Void", false);
            QuerySnapshot tasksQuerySnapshot = await taskCollection.GetSnapshotAsync();
            List<ICalendarTask> tasks = new List<ICalendarTask>();

            foreach (DocumentSnapshot documentSnapshot in tasksQuerySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    var newFireTask = documentSnapshot.ConvertTo<FireCalendarTask>();
                    var newTask = newFireTask.ToCalendarTask();
                    newTask.CalendarTaskId = documentSnapshot.Id;
                    tasks.Add(newTask);
                }
            }
            return tasks;
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
            await ReorderTasks(new DTOCalendarTask(task, TaskPosition.MaxValue));

            task.InsertDate = DateTime.UtcNow;

            CollectionReference colRef = FirestoreConnector.Instance.fireStoreDb.Collection("task_todo");
            var reference = await colRef.AddAsync(new FireCalendarTask(task));

            return reference.Id;
        }

        public async Task<bool> ReorderTasks(DTOCalendarTask task)
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

        private async Task reorderTasks(DTOCalendarTask task)
        {
            int difference = task.AbsolutePosition - task.InitialAbsolutePosition;

            var tasks = await GetAsync(task.UserId);

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
                return await updateTaskAsync(new DTOCalendarTask(task));
            }
            catch (Exception ex)
            {
                Logger.Error("Error in UpdateTaskAsyncNoPositionCheck", ex);
                return false;
            }
        }

        private async Task<bool> updateTaskAsync(DTOCalendarTask task)
        {
            if (task.HasBeenVoided())
            {
                task.AbsolutePosition = TaskPosition.MaxValue;
            }

            if (task.PositionHasBeenModified())
                await this.ReorderTasks(task);

            return await UpdateTaskAsyncNoPositionCheck(task);
        }
    }
}
