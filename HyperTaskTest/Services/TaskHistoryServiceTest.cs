using HyperTaskCore.Models;
using HyperTaskServices.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HyperTaskTest
{
    public partial class CalendarTaskApiTest
    {
        [TestMethod]
        public void InsertHistoryAsync_ShouldReturnId()
        {
            // ARRANGE
            var testTask = getTestTask();
            var result = this.calendarTaskService.InsertTaskAsync(testTask).Result;
            TaskHistory testHistory = getDoneTestTaskHistory(testTask);
            // testHistory.CalendarTaskId = result;
            // ACT
            var id = taskHistoryService.InsertHistoryAsync(testHistory).Result;

            // ASSERT
            Assert.IsTrue(id != null && id.Length > 0);
        }
        
        private static TaskHistory getDoneTestTaskHistory(CalendarTask testTask)
        {
            var testHistory = new TaskHistory(testTask);

            testHistory.DoneDate = DateTime.UtcNow;
            testHistory.TaskDone = true;
            testHistory.TaskResult = true;
            return testHistory;
        }

        private static void AssertValuesAreTheSame(ITaskHistory testHistory, ITaskHistory history)
        {
            Assert.AreEqual(testHistory.CalendarTaskId, history.CalendarTaskId);
            Assert.AreEqual(testHistory.DoneDate.Value.Date, history.DoneDate.Value.Date);
            Assert.AreEqual(testHistory.DoneDate.Value.Hour, history.DoneDate.Value.Hour);
            Assert.AreEqual(testHistory.DoneDate.Value.Minute, history.DoneDate.Value.Minute);
            Assert.AreEqual(testHistory.DoneDate.Value.Second, history.DoneDate.Value.Second);
            Assert.AreEqual(testHistory.TaskDone, history.TaskDone);
            Assert.AreEqual(testHistory.TaskDurationSeconds, history.TaskDurationSeconds);
            Assert.AreEqual(testHistory.TaskHistoryId, history.TaskHistoryId);
            Assert.AreEqual(testHistory.TaskResult, history.TaskResult);
            Assert.AreEqual(testHistory.TaskSkipped, history.TaskSkipped);
            Assert.AreEqual(testHistory.UserId, history.UserId);
            Assert.AreEqual(testHistory.Void, history.Void);
        }

        private static TaskHistory getTestTaskHistory(CalendarTask testTask)
        {
            var testHistory = new TaskHistory(testTask);

            testHistory.DoneDate = DateTime.UtcNow;
            testHistory.TaskDone = true;
            testHistory.TaskResult = true;
            testHistory.TaskDurationSeconds = 10;
            return testHistory;
        }

        private static CalendarTask getTestTask()
        {
            var testTask = new CalendarTask();

            testTask.Name = "Test Task";
            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;
            testTask.CalendarTaskId = Guid.NewGuid().ToString();
            return testTask;
        }
    }
}
