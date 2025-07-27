using HyperTaskCore.Exceptions;
using HyperTaskCore.Models;
using HyperTaskServices.Caching;
using HyperTaskServices.Models.DTO;
using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyperTaskTest
{
    [TestClass]
    public partial class CalendarTaskApiTest
    {
        private WebApplicationFactory<Program> _factory;
        private IServiceProvider _serviceProvider;
        private FireCalendarTaskService _fireCalendarTaskService;
        private MongoCalendarTaskService _mongoCalendarTaskService;
        private FireTaskGroupService _fireTaskGroupService;
        private MongoTaskGroupService _mongoTaskGroupService;
        private TaskHistoryService _taskHistoryService;
        private FireUserService _fireUserService;
        private MongoUserService _mongoUserService;
        private CalendarTaskController _calendarTaskController;
        private TaskGroupController _taskGroupController;
        private ReportService _reportService;
        private ILogger<CalendarTaskApiTest> _logger;

        private static string _testUserId = "testUser";

        public CalendarTaskApiTest()
        {
            // Initialize WebApplicationFactory with Program.cs
            _factory = new WebApplicationFactory<Program>();

            // Create a scope to resolve services
            var scope = _factory.Services.CreateScope();
            _serviceProvider = scope.ServiceProvider;

            // Resolve logger and IConnectorFactory
            _logger = _serviceProvider.GetRequiredService<ILogger<CalendarTaskApiTest>>();
            var factory = _serviceProvider.GetRequiredService<IConnectorFactory>();

            _logger.LogDebug("Initializing test services");

            // Initialize services asynchronously
            Task.Run(async () =>
            {
                var firebaseConnector = await factory.CreateFirebaseConnectorAsync(_serviceProvider);
                var mongoConnector = await factory.CreateMongoConnectorAsync(_serviceProvider);

                _fireTaskGroupService = new FireTaskGroupService(firebaseConnector);
                _mongoTaskGroupService = new MongoTaskGroupService(mongoConnector);
                _fireCalendarTaskService = new FireCalendarTaskService(firebaseConnector);
                _mongoCalendarTaskService = new MongoCalendarTaskService(mongoConnector);
                _calendarTaskController = new CalendarTaskController(firebaseConnector, mongoConnector, _mongoTaskGroupService);
                _taskHistoryService = new TaskHistoryService(_fireCalendarTaskService);
                _fireUserService = new FireUserService(firebaseConnector, _fireCalendarTaskService, _fireTaskGroupService);
                _mongoUserService = new MongoUserService(mongoConnector, _mongoCalendarTaskService, _mongoTaskGroupService, firebaseConnector);
                _taskGroupController = new TaskGroupController(firebaseConnector, mongoConnector, _mongoCalendarTaskService);
                _reportService = new ReportService(_fireCalendarTaskService, _fireTaskGroupService);

                // Clean up test data
                await DeleteTestsFirebaseAsync(firebaseConnector);
                await DeleteTestsMongoAsync(mongoConnector);

                _logger.LogDebug("Test services initialized");
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public async Task Post_ShouldReturnIdAndCode200()
        {
            // ARRANGE
            var testTask = new DTOCalendarTask
            {
                Name = "Test Task",
                Frequency = eTaskFrequency.Daily,
                ResultType = eResultType.Binary,
                RequiredDays = new List<DayOfWeek> { DayOfWeek.Monday },
                UserId = _testUserId,
                Position = 1
            };

            // ACT
            var response = await _calendarTaskController.Post(testTask);
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.IsTrue(okResult.Value is string, "Expected string value");
            Assert.IsTrue(!string.IsNullOrEmpty(okResult.Value as string), "Expected non-empty string ID");
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Get_ShouldReturnTaskListAndCode200()
        {
            // ARRANGE
            for (int i = 1; i < 3; i++)
            {
                var testTask = new DTOCalendarTask
                {
                    Name = Guid.NewGuid().ToString(),
                    Frequency = eTaskFrequency.Monthly,
                    ResultType = eResultType.Decimal,
                    RequiredDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday },
                    UserId = _testUserId,
                    Position = i,
                    InitialPosition = i,
                    CalendarTaskId = Guid.NewGuid().ToString()
                };
                await _mongoCalendarTaskService.InsertTaskAsync(testTask);
            }

            // ACT
            var response = await _calendarTaskController.Get(new DTOGetCalendarTaskRequest
            {
                userId = _testUserId
            });
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.IsTrue(okResult.Value is List<DTOCalendarTask>, "Expected List<DTOCalendarTask>");
            Assert.AreEqual(2, ((List<DTOCalendarTask>)okResult.Value).Count, "Expected 2 tasks");
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Put_ShouldReturnTrueAndCode200()
        {
            // ARRANGE
            var testTask = new CalendarTask
            {
                Name = "Test Task",
                Frequency = eTaskFrequency.Daily,
                ResultType = eResultType.Binary,
                RequiredDays = new List<DayOfWeek> { DayOfWeek.Monday },
                UserId = _testUserId,
                Position = 1,
                CalendarTaskId = Guid.NewGuid().ToString()
            };
            await _mongoCalendarTaskService.InsertTaskAsync(testTask);

            // ACT
            var task = await _mongoCalendarTaskService.GetTaskAsync(testTask.CalendarTaskId);
            var dtoTask = new DTOCalendarTask(task);
            var response = await _calendarTaskController.Put(dtoTask);
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.IsTrue(okResult.Value is bool, "Expected boolean value");
            Assert.IsTrue((bool)okResult.Value, "Expected true");
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Put_ShouldUpdateTask()
        {
            // ARRANGE
            var testTask = new CalendarTask
            {
                Name = "Test Task",
                Frequency = eTaskFrequency.Daily,
                ResultType = eResultType.Binary,
                RequiredDays = new List<DayOfWeek> { DayOfWeek.Monday },
                UserId = _testUserId,
                Position = 1,
                CalendarTaskId = Guid.NewGuid().ToString()
            };
            await _mongoCalendarTaskService.InsertTaskAsync(testTask);

            // ACT
            var task = await _mongoCalendarTaskService.GetTaskAsync(testTask.CalendarTaskId);
            var dtoTask = new DTOCalendarTask(task)
            {
                Name = "new task name",
                Position = 2,
                Frequency = eTaskFrequency.Weekly,
                RequiredDays = new List<DayOfWeek>(),
                ResultType = eResultType.Time
            };
            await _calendarTaskController.Put(dtoTask);
            var taskUpdated = await _mongoCalendarTaskService.GetTaskAsync(testTask.CalendarTaskId);

            // ASSERT
            Assert.AreEqual(dtoTask.Name, taskUpdated.Name);
            Assert.AreEqual(dtoTask.Position, taskUpdated.Position);
            Assert.AreEqual(dtoTask.Frequency, taskUpdated.Frequency);
            Assert.AreEqual(dtoTask.ResultType, taskUpdated.ResultType);
            CollectionAssert.AreEqual(dtoTask.RequiredDays, taskUpdated.RequiredDays);
        }

        [TestMethod]
        public async Task Post_Name200Chars_ShouldReturnError()
        {
            // ARRANGE
            var testTask = new DTOCalendarTask
            {
                Name = "TestTask" + Guid.NewGuid().ToString() + new string('A', 200),
                Frequency = eTaskFrequency.Daily,
                ResultType = eResultType.Binary,
                RequiredDays = new List<DayOfWeek> { DayOfWeek.Monday },
                UserId = _testUserId,
                Position = 1
            };

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<AggregateException>(async () =>
                await _calendarTaskController.Post(testTask));
        }

        [TestMethod]
        public async Task Post_Comment1001Chars_ShouldReturnError()
        {
            // ARRANGE
            var testTask = new DTOCalendarTask
            {
                Name = "TestTask" + Guid.NewGuid().ToString(),
                CalendarTaskId = Guid.NewGuid().ToString(),
                Frequency = eTaskFrequency.Daily,
                ResultType = eResultType.Binary,
                RequiredDays = new List<DayOfWeek> { DayOfWeek.Monday },
                UserId = _testUserId,
                Position = 1
            };

            // Initialize Histories after testTask is fully created
            testTask.Histories = new List<ITaskHistory>()
            {
                new TaskHistory
                {
                    CalendarTaskId = testTask.CalendarTaskId,
                    UserId = testTask.UserId,
                    TaskHistoryId = Guid.NewGuid().ToString(),
                    Comment = new string('A', 2001)
                }
            };

            // ACT & ASSERT
            await Assert.ThrowsExceptionAsync<AggregateException>(async () =>
                await _calendarTaskController.Post(testTask));
        }

        private async Task DeleteTestsFirebaseAsync(FirebaseConnector firebaseConnector)
        {
            var tasks = await _fireCalendarTaskService.GetTasksAsync(_testUserId, true);
            foreach (var task in tasks)
            {
                await _fireCalendarTaskService.DeleteTaskAsync(task.CalendarTaskId);
            }
        }

        private async Task DeleteTestsMongoAsync(MongoConnector mongoConnector)
        {
            var tasks = await _mongoCalendarTaskService.GetTasksAsync(_testUserId, true);
            foreach (var task in tasks)
            {
                await _mongoCalendarTaskService.DeleteTaskAsync(task.CalendarTaskId);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_factory != null)
            {
                await _factory.DisposeAsync();
            }
        }
    }

    public class TestSettings
    {
        public string KeyVaultName { get; set; }
        public string InstrumentationKeySecretName { get; set; }
        public string FirebaseSecretName { get; set; }
        public string MongoConnectionSecretName { get; set; }
        public string AzureDevopsTokenSecretName { get; set; }
        public string AzureDevopsUri { get; set; }
        public string AzureDevopsProjectName { get; set; }
    }
}