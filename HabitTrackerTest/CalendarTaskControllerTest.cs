using HabitTrackerCore.Models;
using HabitTrackerServices;
using HabitTrackerServices.Models.DTO;
using HabitTrackerTools;
using HabitTrackerWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HabitTrackerTest
{
    [TestClass]
    public class CalendarTaskControllerTest
    {
        private static CalendarTaskService calendarTaskService = new CalendarTaskService();
        private static CalendarTaskController calendarTaskController = new CalendarTaskController();
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
        public void Post_ShouldReturnTrueAndCode200()
        {
            // ARRANGE
            var testTask = new DTOCalendarTask();

            testTask.Name = "Test Task";
            testTask.Description = "Test Task Description";
            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;

            // ACT
            var response = calendarTaskController.Post(testTask).Result;
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsTrue(okResult.Value is Boolean);
            Assert.AreEqual(true, (bool)okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public void Get_ShouldReturnTaskListAndCode200()
        {
            // ARRANGE
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
                testTask.InitialAbsolutePosition = i;
                testTask.Positive = false;
                testTask.CalendarTaskId = calendarTaskService.InsertTaskAsync(testTask).Result;
            }

            // ACT
            var response = calendarTaskController.Get(testUserId).Result;
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsTrue(okResult.Value is List<DTOCalendarTask>);
            Assert.AreEqual(2, ((List<DTOCalendarTask>)okResult.Value).Count);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public void Put_ShouldReturnTrueAndCode200()
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

            // ACT
            var task = calendarTaskService.GetTaskAsync(testTask.CalendarTaskId).Result;
            var DTOtask = new DTOCalendarTask(task);
            DTOtask.Description = "new description";
            var response = calendarTaskController.Put(DTOtask).Result;
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsTrue(okResult.Value is Boolean);
            Assert.AreEqual(true, (bool)okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public void Put_ShouldUpdateTask()
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

            // ACT
            var task = calendarTaskService.GetTaskAsync(testTask.CalendarTaskId).Result;
            var DTOtask = new DTOCalendarTask(task);
            DTOtask.Description = "new description";
            DTOtask.Name = "new task name";
            DTOtask.MinDuration = 3;
            DTOtask.AbsolutePosition = 2;
            DTOtask.Frequency = eTaskFrequency.Weekly;
            DTOtask.RequiredDays = new List<DayOfWeek>();
            DTOtask.ResultType = eResultType.Time;
            calendarTaskController.Put(DTOtask).Wait();

            var taskUpdated = calendarTaskService.GetTaskAsync(testTask.CalendarTaskId).Result;

            // ASSERT
            Assert.AreEqual(DTOtask.Name, taskUpdated.Name);
            Assert.AreEqual(DTOtask.Description, taskUpdated.Description);
            Assert.AreEqual(DTOtask.MinDuration, taskUpdated.MinDuration);
            Assert.AreEqual(DTOtask.AbsolutePosition, taskUpdated.AbsolutePosition);
            Assert.AreEqual(DTOtask.Frequency, taskUpdated.Frequency);
            Assert.AreEqual(DTOtask.ResultType, taskUpdated.ResultType);
            CollectionAssert.AreEqual(DTOtask.RequiredDays, taskUpdated.RequiredDays);
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
