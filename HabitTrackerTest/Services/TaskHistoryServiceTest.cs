using HabitTrackerCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HabitTrackerTest
{
    public partial class CalendarTaskApiTest
    {
        [TestMethod]
        public void InsertHistoryAsync_ShouldReturnId()
        {
            // ARRANGE
            var testTask = new CalendarTask();

            testTask.Name = "Test Task";
            testTask.Description = "Test Task Description";
            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;
            testTask.CalendarTaskId = calendarTaskService.InsertTaskAsync(testTask).Result;

            var testHistory = new TaskHistory(testTask);

            testHistory.DoneDate = DateTime.UtcNow;
            testHistory.TaskDone = true;
            testHistory.TaskResult = true;

            // ACT
            var id = taskHistoryService.InsertHistoryAsync(testHistory).Result;

            // ASSERT
            Assert.IsTrue(id != null && id.Length > 0);
        }


        [TestMethod]
        public void GetHistoryAsync_ShouldReturnSameValuesAsInsert()
        {
            // ARRANGE
            var testTask = new CalendarTask();

            testTask.Name = "Test Task";
            testTask.Description = "Test Task Description";
            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;
            testTask.CalendarTaskId = calendarTaskService.InsertTaskAsync(testTask).Result;

            var testHistory = new TaskHistory(testTask);

            testHistory.DoneDate = DateTime.UtcNow;
            testHistory.TaskDone = true;
            testHistory.TaskResult = true;
            testHistory.TaskDurationSeconds = 10;
            testHistory.TaskHistoryId = taskHistoryService.InsertHistoryAsync(testHistory).Result;

            // ACT
            var history = taskHistoryService.GetHistoryAsync(testHistory.TaskHistoryId).Result;

            Assert.AreEqual(testHistory.CalendarTaskId, history.CalendarTaskId);
            Assert.AreEqual(testHistory.DoneDate.Date, history.DoneDate.Date);
            Assert.AreEqual(testHistory.DoneDate.Hour, history.DoneDate.Hour);
            Assert.AreEqual(testHistory.DoneDate.Minute, history.DoneDate.Minute);
            Assert.AreEqual(testHistory.DoneDate.Second, history.DoneDate.Second);
            Assert.AreEqual(testHistory.TaskDone, history.TaskDone);
            Assert.AreEqual(testHistory.TaskDurationSeconds, history.TaskDurationSeconds);
            Assert.AreEqual(testHistory.TaskHistoryId, history.TaskHistoryId);
            Assert.AreEqual(testHistory.TaskResult, history.TaskResult);
            Assert.AreEqual(testHistory.TaskSkipped, history.TaskSkipped);
            Assert.AreEqual(testHistory.UserId, history.UserId);
            Assert.AreEqual(testHistory.Void, history.Void);
        }
    }
}
