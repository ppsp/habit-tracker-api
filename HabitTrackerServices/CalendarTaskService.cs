using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HabitTrackerServices
{
    public class CalendarTaskService : ICalendarTaskService
    {
        public async Task<List<CalendarTask>> GetAsync(string userId)
        {
            try
            {
                Query taskCollection = FirestoreConnector.Instance.fireStoreDb
                                                                  .Collection("task_todo")
                                                                  .WhereEqualTo("UserId", userId);
                QuerySnapshot tasksQuerySnapshot = await taskCollection.GetSnapshotAsync();
                List<CalendarTask> tasks = new List<CalendarTask>();

                foreach (DocumentSnapshot documentSnapshot in tasksQuerySnapshot.Documents)
                {
                    if (documentSnapshot.Exists)
                    {
                        Dictionary<string, object> task = documentSnapshot.ToDictionary();
                        string json = JsonConvert.SerializeObject(task);
                        CalendarTask newTask = JsonConvert.DeserializeObject<CalendarTask>(json);
                        newTask.CalendarTaskId = documentSnapshot.Id;
                        tasks.Add(newTask);
                    }
                }
                return tasks;
            }
            catch (Exception ex)
            {
                return new List<CalendarTask>();
            }
        }

        public async Task<bool> InsertTaskAsync(CalendarTask task)
        {
            try
            {
                CollectionReference colRef = FirestoreConnector.Instance.fireStoreDb.Collection("task_todo");
                await colRef.AddAsync(task);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ReorderTasks(CalendarTask task, int initialAbsolutePosition)
        {
            try
            {
                int difference = task.AbsolutePosition - initialAbsolutePosition;

                var tasks = await GetAsync(task.UserId);

                foreach (var currentTask in tasks.Where(p => p.AbsolutePosition.IsBetween(task.AbsolutePosition, 
                                                                                          initialAbsolutePosition) &&
                                                             !p.Void && p.CalendarTaskId != task.CalendarTaskId))
                {
                    currentTask.AbsolutePosition = difference < 0 ?
                                                    currentTask.AbsolutePosition + 1 :
                                                    currentTask.AbsolutePosition - 1;

                    await UpdateTaskAsyncNoPositionCheck(currentTask);
                }

                return true;
            }
            catch (Exception ex)
            {
                //TODO : Logger
                
                return false;
            }
        }

        private async Task<bool> UpdateTaskAsyncNoPositionCheck(CalendarTask task)
        {
            try
            {
                DocumentReference taskRef = FirestoreConnector.Instance.fireStoreDb
                                                                       .Collection("task_todo")
                                                                       .Document(task.CalendarTaskId);

                var dictionnary = task.ToDictionary();

                await taskRef.UpdateAsync(dictionnary);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTaskAsync(CalendarTask task, int? initialAbsolutePosition = null)
        {
            try
            {
                if (initialAbsolutePosition != null)
                    await this.ReorderTasks(task, initialAbsolutePosition.Value);

                return await UpdateTaskAsyncNoPositionCheck(task);
            }
            catch (Exception ex)
            {
                return false;
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
                return false;
            }
        }
    }
}
