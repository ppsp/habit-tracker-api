using HabitTrackerCore.Models;
using HabitTrackerServices;
using HabitTrackerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HabitTrackerTest
{
    [TestClass]
    public class CalendarTaskServiceTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Logger.ConfigLogger();
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
            testTask.UserId = "testUser";
            testTask.AbsolutePosition = 1;

            // ACT
            var calendarTaskService = new CalendarTaskService();
            var id = calendarTaskService.InsertTaskAsync(testTask).Result;

            // ASSERT
            Assert.IsTrue(id != null && id.Length > 0);
        }
    }
}
