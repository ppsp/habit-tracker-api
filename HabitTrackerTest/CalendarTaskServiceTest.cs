using HabitTrackerCore.Models;
using HabitTrackerServices;
using HabitTrackerServices.Models.DTO;
using HabitTrackerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HabitTrackerTest
{
    [TestClass]
    public class CalendarTaskServiceTest
    {
        private static CalendarTaskService calendarTaskService = new CalendarTaskService();
        private static string testUserId = "testUser";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Logger.ConfigLogger();
            DeleteTests();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DeleteTests();
        }

        [TestMethod]
        public void HelloWorld()
        {
            // ASSERT
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void InsertTaskAsync_ShouldReturnId()
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

            // ACT
            var id = calendarTaskService.InsertTaskAsync(testTask).Result;

            // ASSERT
            Assert.IsTrue(id != null && id.Length > 0);
        }


        [TestMethod]
        public void GetTaskAsync_ShouldReturnSameValuesAsInsert()
        {
            // ARRANGE
            var testTask = new CalendarTask();

            testTask.Name = Guid.NewGuid().ToString();
            testTask.Description = Guid.NewGuid().ToString();
            testTask.Frequency = eTaskFrequency.Monthly;
            testTask.ResultType = eResultType.Decimal;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;
            testTask.Positive = false;

            // ACT
            var id = calendarTaskService.InsertTaskAsync(testTask).Result;

            // ASSERT
            var task = calendarTaskService.GetTaskAsync(id).Result;
            Assert.AreEqual(testTask.Name, task.Name);
            Assert.AreEqual(testTask.MinDuration, task.MinDuration);
            Assert.AreEqual(testTask.Positive, task.Positive);
            CollectionAssert.AreEqual(testTask.RequiredDays, task.RequiredDays);
            Assert.AreEqual(testTask.ResultType, task.ResultType);
            Assert.AreEqual(testTask.UserId, task.UserId);
            Assert.AreEqual(testTask.AbsolutePosition, task.AbsolutePosition);
            Assert.AreEqual(testTask.Description, task.Description);
            Assert.AreEqual(testTask.Frequency, task.Frequency);
            Assert.IsTrue(task.InsertDate != null && task.InsertDate.Value > DateTime.MinValue);
        }

        [TestMethod]
        public void UpdateAbsolutePositionToFirst_ShouldIncrementOtherTasks()
        {
            // ARRANGE
            var ids = new List<string>();
            var tasks = new List<DTOCalendarTask>();
            for (int i=1; i<5; i++)
            {
                var testTask = new DTOCalendarTask();

                testTask.Name = Guid.NewGuid().ToString();
                testTask.Description = Guid.NewGuid().ToString();
                testTask.Frequency = eTaskFrequency.Monthly;
                testTask.ResultType = eResultType.Decimal;
                testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
                testTask.UserId = testUserId;
                testTask.AbsolutePosition = i;
                testTask.Positive = false;
                testTask.CalendarTaskId = calendarTaskService.InsertTaskAsync(testTask).Result;

                tasks.Add(testTask);
            }

            Assert.AreEqual(1, tasks[0].AbsolutePosition);
            Assert.AreEqual(2, tasks[1].AbsolutePosition);
            Assert.AreEqual(3, tasks[2].AbsolutePosition);
            Assert.AreEqual(4, tasks[3].AbsolutePosition);

            // ACT (Take the last one, put it in first)
            var lastTask = tasks.Last();
            lastTask.AbsolutePosition = 1;
            var result = calendarTaskService.UpdateTaskAsync(lastTask).Result;

            // ASSERT
            var updatedTasks = calendarTaskService.GetTasksAsync(testUserId).Result;
            Assert.AreEqual(2, updatedTasks.First(p => p.CalendarTaskId == tasks[0].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(3, updatedTasks.First(p => p.CalendarTaskId == tasks[1].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(4, updatedTasks.First(p => p.CalendarTaskId == tasks[2].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(1, updatedTasks.First(p => p.CalendarTaskId == tasks[3].CalendarTaskId).AbsolutePosition);
        }


        [TestMethod]
        public void UpdateAbsolutePositionToLast_ShouldDecrementOtherTasks()
        {
            // ARRANGE
            var ids = new List<string>();
            var tasks = new List<DTOCalendarTask>();
            for (int i = 1; i < 5; i++)
            {
                var testTask = new DTOCalendarTask();

                testTask.Name = Guid.NewGuid().ToString();
                testTask.Description = Guid.NewGuid().ToString();
                testTask.Frequency = eTaskFrequency.Monthly;
                testTask.ResultType = eResultType.Decimal;
                testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
                testTask.UserId = testUserId;
                testTask.AbsolutePosition = i;
                testTask.Positive = false;
                testTask.CalendarTaskId = calendarTaskService.InsertTaskAsync(testTask).Result;

                tasks.Add(testTask);
            }

            Assert.AreEqual(1, tasks[0].AbsolutePosition);
            Assert.AreEqual(2, tasks[1].AbsolutePosition);
            Assert.AreEqual(3, tasks[2].AbsolutePosition);
            Assert.AreEqual(4, tasks[3].AbsolutePosition);

            // ACT (Take the first one, put it in last)
            var lastTask = tasks.First();
            lastTask.AbsolutePosition = 4;
            var result = calendarTaskService.UpdateTaskAsync(lastTask).Result;

            // ASSERT
            var updatedTasks = calendarTaskService.GetTasksAsync(testUserId).Result;
            Assert.AreEqual(4, updatedTasks.First(p => p.CalendarTaskId == tasks[0].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(1, updatedTasks.First(p => p.CalendarTaskId == tasks[1].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(2, updatedTasks.First(p => p.CalendarTaskId == tasks[2].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(3, updatedTasks.First(p => p.CalendarTaskId == tasks[3].CalendarTaskId).AbsolutePosition);
        }

        [TestMethod]
        public void InsertAtTheEnd_ShouldNotIncrementOtherTasks()
        {
            // ARRANGE + ACT
            var ids = new List<string>();
            var tasks = new List<DTOCalendarTask>();
            for (int i = 1; i < 3; i++)
            {
                var testTask = new DTOCalendarTask();

                testTask.Name = Guid.NewGuid().ToString();
                testTask.Description = Guid.NewGuid().ToString();
                testTask.Frequency = eTaskFrequency.Monthly;
                testTask.ResultType = eResultType.Decimal;
                testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
                testTask.UserId = testUserId;
                testTask.AbsolutePosition = i;
                testTask.Positive = false;
                testTask.CalendarTaskId = calendarTaskService.InsertTaskAsync(testTask).Result;

                tasks.Add(testTask);
            }

            Assert.AreEqual(1, tasks[0].AbsolutePosition);
            Assert.AreEqual(2, tasks[1].AbsolutePosition);

            // ASSERT
            var updatedTasks = calendarTaskService.GetTasksAsync(testUserId).Result;
            Assert.AreEqual(1, updatedTasks.First(p => p.CalendarTaskId == tasks[0].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(2, updatedTasks.First(p => p.CalendarTaskId == tasks[1].CalendarTaskId).AbsolutePosition);
        }

        private static void DeleteTests()
        {
            var tasks = calendarTaskService.GetTasksAsync(testUserId, true).Result;

            foreach (var task in tasks)
            {
                var result = calendarTaskService.DeleteTaskAsync(task.CalendarTaskId).Result;
            }
        }
    }
}
