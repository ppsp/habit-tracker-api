using HabitTrackerCore.Models;
using HabitTrackerServices.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerTest
{
    public partial class CalendarTaskApiTest
    {

        /*[TestMethod]
        public void AddTaskHistoriesToCache_SavesToCache()
        {
            // ARRANGE
            CachedTaskHistories taskHistories = getTestTaskHistories();

            // ACT
            this.taskHistoryCache.AddToCache(taskHistories);

            // ASSERT
            var cachedHistories = this.taskHistoryCache.GetCachedHistories(taskHistories.Request);
            AssertValuesAreTheSame(taskHistories.Histories[0], cachedHistories.Histories[0]);
        }

        private static CachedTaskHistories getTestTaskHistories()
        {
            var testTask = getTestTask();
            var taskHistories = new CachedTaskHistories();
            taskHistories.Request = new GetCalendarTaskRequest()
            {
                UserId = testUserId,
                IncludeVoid = false,
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today.AddDays(1)
            };
            taskHistories.Histories = new List<ITaskHistory>();
            taskHistories.Histories.Add(getTestTaskHistory(testTask));
            return taskHistories;
        }*/
    }
}
