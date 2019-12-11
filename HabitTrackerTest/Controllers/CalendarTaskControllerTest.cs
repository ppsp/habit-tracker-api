﻿using HabitTrackerCore.DAL;
using HabitTrackerCore.Models;
using HabitTrackerServices.Caching;
using HabitTrackerServices.DAL;
using HabitTrackerServices.Models.DTO;
using HabitTrackerServices.Services;
using HabitTrackerTools;
using HabitTrackerWebApi;
using HabitTrackerWebApi.Controllers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HabitTrackerTest
{
    [TestClass]
    public partial class CalendarTaskApiTest
    {
        private DependencyResolverHelper ServiceProvider;
        private CalendarTaskService calendarTaskService;
        private TaskHistoryService taskHistoryService;
        private CalendarTaskController calendarTaskController;
        private IDALTaskHistory dalTaskHistory;
        private TaskHistoryCache taskHistoryCache;
        private static string testUserId = "testUser";

        public CalendarTaskApiTest()
        {
            Logger.ConfigLogger();

            var webHost = WebHost.CreateDefaultBuilder()
                                 .UseStartup<Startup>()
                                 .Build();

            ServiceProvider = new DependencyResolverHelper(webHost);
            var firebaseConnector = ServiceProvider.GetService<FirebaseConnector>();
            var cachingManager = ServiceProvider.GetService<CachingManager>();
            var dalTaskHistory = ServiceProvider.GetService<IDALTaskHistory>();
            var taskHistoryCache = ServiceProvider.GetService<TaskHistoryCache>();
            this.calendarTaskController = new CalendarTaskController(firebaseConnector, taskHistoryCache, dalTaskHistory);
            this.calendarTaskService = new CalendarTaskService(firebaseConnector);
            this.taskHistoryService = new TaskHistoryService(dalTaskHistory, taskHistoryCache);
            this.taskHistoryCache = taskHistoryCache;
            this.dalTaskHistory = dalTaskHistory;

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
            var response = calendarTaskController.Get(new DTOGetCalendarTaskRequest()
            {
                userId = testUserId
            }).Result;
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

        private void DeleteTests()
        {
            var tasks = calendarTaskService.GetTasksAsync(testUserId, true).Result;

            foreach (var task in tasks)
            {
                var result = calendarTaskService.DeleteTaskAsync(task.CalendarTaskId).Result;
            }
        }
    }
}
