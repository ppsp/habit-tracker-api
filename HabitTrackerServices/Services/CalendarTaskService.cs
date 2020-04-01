using Google.Cloud.Firestore;
using HabitTrackerCore.Exceptions;
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
        private FirebaseConnector Connector { get; set; }

        public CalendarTaskService(FirebaseConnector connector)
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
                                                             bool includeOnceDone = true)
        {
            try
            {
                return await getTasksAsync(userId, includeVoid, firstPosition, lastPosition, includeOnceDone);
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
                                                              bool includeOnceDone = true)
        {
            Query query = getGetTasksQuery(userId, includeVoid, firstPosition, lastPosition);
            try
            {
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

        private Query getGetTasksQuery(string userId, 
                                       bool includeVoid,
                                       int? firstPosition,
                                       int? lastPosition)
        {
            var query = this.Connector.fireStoreDb
                                      .Collection("task_todo")
                                      .WhereEqualTo("UserId", userId);

            if (!includeVoid)
                query = query.WhereEqualTo("Void", false);

            if (firstPosition != null)
                query = query.WhereGreaterThanOrEqualTo("AbsolutePosition", firstPosition.Value);

            if (lastPosition != null)
                query = query.WhereLessThanOrEqualTo("AbsolutePosition", lastPosition.Value);

            return query;
        }

        public async Task<string> InsertTaskAsync(ICalendarTask task)
        {
            try
            {
                // Check if AbsolutePosition already exists
                var existingTasks = await getTasksAsync(task.UserId, false, task.AbsolutePosition, task.AbsolutePosition);
                if (existingTasks.Count > 0)
                {
                    await reorderTasks(task);
                }

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
            if (task.InsertDate == null)
                task.InsertDate = DateTime.UtcNow;

            CollectionReference colRef = this.Connector.fireStoreDb.Collection("task_todo");
            var reference = await colRef.AddAsync(new FireCalendarTask(task));

            return reference.Id;
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
                Logger.Error("Error in ReorderTasks", ex);
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
                                            false);

            if (tasks.Count > Math.Abs(difference) + 1) // reorder all if 2 are the same
            {
                await reorderAllTasks(task);
            }
            else
            {
                // reorder only between current and new Id
                foreach (var currentTask in tasks.Where(p => p.AbsolutePosition.IsBetween(task.AbsolutePosition,
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

            int positionIterator = 1;
            foreach (var currentTask in tasks.Where(p => !p.Void &&
                                                         p.CalendarTaskId != task.CalendarTaskId &&
                                                         (IsPresentOrFuture(p)))
                                             .OrderBy(p => p.AbsolutePosition))
            {
                currentTask.AbsolutePosition = positionIterator++;
                await UpdateTaskAsyncNoPositionCheck(currentTask);
            }
        }

        private static bool IsPresentOrFuture(ICalendarTask p)
        {
            return p.Frequency.NotIn(eTaskFrequency.Once, eTaskFrequency.UntilDone) ||
                                     p.Histories.Any(p => p.TaskDone);
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

            if (task.AssignedDate != null)
                task.AssignedDate = task.AssignedDate.Value.ToUniversalTime();

            DocumentReference taskRef = this.Connector.fireStoreDb
                                                      .Collection("task_todo")
                                                      .Document(task.CalendarTaskId);

            var dictionnary = new FireCalendarTask(task).ToDictionary();

            await taskRef.UpdateAsync(dictionnary);
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
                var reference = this.Connector.fireStoreDb
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
                DocumentReference taskRef = this.Connector.fireStoreDb
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
