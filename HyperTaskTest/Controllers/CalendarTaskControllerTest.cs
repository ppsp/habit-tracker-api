using HyperTaskCore.Exceptions;
using HyperTaskCore.Models;
using HyperTaskServices.Caching;
using HyperTaskServices.Models.DTO;
using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi;
using HyperTaskWebApi.Controllers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace HyperTaskTest
{
    [TestClass]
    public partial class CalendarTaskApiTest
    {
        private DependencyResolverHelper ServiceProvider;
        private CalendarTaskService calendarTaskService;
        private TaskGroupService taskGroupService;
        private TaskHistoryService taskHistoryService;
        private UserService userService;
        private CalendarTaskController calendarTaskController;
        private TaskGroupController taskGroupController;
        private ReportService reportService;
        private static string testUserId = "testUser";

        private HttpClient TestClient;

        public CalendarTaskApiTest()
        {
            Logger.ConfigLogger();

            var webHost = WebHost.CreateDefaultBuilder()
                                 .UseStartup<Startup>()
                                 .Build();

            ServiceProvider = new DependencyResolverHelper(webHost);
            var firebaseConnector = ServiceProvider.GetService<FirebaseConnector>();
            this.calendarTaskController = new CalendarTaskController(firebaseConnector);
            this.calendarTaskService = new CalendarTaskService(firebaseConnector);
            this.taskHistoryService = new TaskHistoryService(calendarTaskService);
            this.userService = new UserService(firebaseConnector, calendarTaskService);
            this.taskGroupService = new TaskGroupService(firebaseConnector);
            this.taskGroupController = new TaskGroupController(firebaseConnector, calendarTaskService);
            this.reportService = new ReportService(calendarTaskService, taskGroupService);

            DeleteTests();
        }

        [TestMethod]
        public void Post_ShouldReturnIdAndCode200()
        {
            // ARRANGE
            var testTask = new DTOCalendarTask();

            testTask.Name = "Test Task";
            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;

            // ACT
            var response = calendarTaskController.Post(testTask).Result;
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsTrue(okResult.Value is string);
            Assert.IsTrue((okResult.Value as string).Length > 0);
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
                testTask.Frequency = eTaskFrequency.Monthly;
                testTask.ResultType = eResultType.Decimal;
                testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
                testTask.UserId = testUserId;
                testTask.AbsolutePosition = i;
                testTask.InitialAbsolutePosition = i;
                testTask.CalendarTaskId = Guid.NewGuid().ToString();
                var id = calendarTaskService.InsertTaskAsync(testTask).Result;
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
            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;
            testTask.CalendarTaskId = Guid.NewGuid().ToString(); 
            var id = calendarTaskService.InsertTaskAsync(testTask).Result;

            // ACT
            var task = calendarTaskService.GetTaskAsync(testTask.CalendarTaskId).Result;
            var DTOtask = new DTOCalendarTask(task);
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
            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;
            testTask.CalendarTaskId = Guid.NewGuid().ToString(); 
            var id = calendarTaskService.InsertTaskAsync(testTask).Result;

            // ACT
            var task = calendarTaskService.GetTaskAsync(testTask.CalendarTaskId).Result;
            var DTOtask = new DTOCalendarTask(task);
            DTOtask.Name = "new task name";
            DTOtask.AbsolutePosition = 2;
            DTOtask.Frequency = eTaskFrequency.Weekly;
            DTOtask.RequiredDays = new List<DayOfWeek>();
            DTOtask.ResultType = eResultType.Time;
            calendarTaskController.Put(DTOtask).Wait();

            var taskUpdated = calendarTaskService.GetTaskAsync(testTask.CalendarTaskId).Result;

            // ASSERT
            Assert.AreEqual(DTOtask.Name, taskUpdated.Name);
            Assert.AreEqual(DTOtask.AbsolutePosition, taskUpdated.AbsolutePosition);
            Assert.AreEqual(DTOtask.Frequency, taskUpdated.Frequency);
            Assert.AreEqual(DTOtask.ResultType, taskUpdated.ResultType);
            CollectionAssert.AreEqual(DTOtask.RequiredDays, taskUpdated.RequiredDays);
        }

        [TestMethod]
        public void Post_Name200chars_ShouldReturnError()
        {
            // ARRANGE
            var testTask = new DTOCalendarTask();

            testTask.Name = "TestTask" + Guid.NewGuid().ToString();

            for (int i = 0; i < 200; i++)
            {
                testTask.Name += "A";
            }

            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;

            Assert.ThrowsException<AggregateException>(() => calendarTaskController.Post(testTask).Result);
        }

        [TestMethod]
        public void Post_Comment1001chars_ShouldReturnError()
        {
            // ARRANGE
            var testTask = new DTOCalendarTask();

            testTask.Name = "TestTask" + Guid.NewGuid().ToString();
            testTask.CalendarTaskId = Guid.NewGuid().ToString();
            testTask.Frequency = eTaskFrequency.Daily;
            testTask.ResultType = eResultType.Binary;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;
            testTask.Histories.Add(new TaskHistory()
            {
                 CalendarTaskId = testTask.CalendarTaskId,
                 UserId = testTask.UserId,
                 TaskHistoryId = Guid.NewGuid().ToString(),
            });

            for (int i = 0; i < 2001; i++)
            {
                testTask.Histories[0].Comment += "A";
            }

            Assert.ThrowsException<AggregateException>(() => calendarTaskController.Post(testTask).Result);
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
