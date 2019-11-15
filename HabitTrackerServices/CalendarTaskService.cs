using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTrackerServices
{
    public class CalendarTaskService : ICalendarTaskService
    {
        public async Task<List<CalendarTask>> GetAsync(string userId)
        {
            try
            {
                Query taskCollection = FirestoreConnector.Instance.fireStoreDb.Collection("task_todo");
                QuerySnapshot citiesQuerySnapshot = await taskCollection.GetSnapshotAsync();
                List<CalendarTask> tasks = new List<CalendarTask>();

                foreach (DocumentSnapshot documentSnapshot in citiesQuerySnapshot.Documents)
                {
                    if (documentSnapshot.Exists)
                    {
                        Dictionary<string, object> task = documentSnapshot.ToDictionary();
                        string json = JsonConvert.SerializeObject(task);
                        CalendarTask newTask = JsonConvert.DeserializeObject<CalendarTask>(json);
                        tasks.Add(newTask);
                    }
                }
                return tasks;
            }
            catch (Exception ex)
            {

            }
        }

        public List<CalendarTask> Get(string userId)
        {
            throw new NotImplementedException();
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

        public async Task<bool> ReplaceTaskAsync(CalendarTask task)
        {
            throw new NotImplementedException();
        }
    }
}
