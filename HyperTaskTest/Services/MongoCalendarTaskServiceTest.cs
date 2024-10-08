using HyperTaskCore.Models;
using HyperTaskServices.Models.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperTaskTest
{
    public partial class CalendarTaskApiTest
    {
        [TestMethod]
        public void Mongo_InsertTaskAsync_ShouldReturnId()
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

            // ACT
            var id = mongoCalendarTaskService.InsertTaskAsync(testTask).Result;

            // ASSERT
            Assert.IsTrue(id != null && id.Length > 0);
        }


        [TestMethod]
        public void Mongo_GetTaskAsync_ShouldReturnSameValuesAsInsert()
        {
            // ARRANGE
            var testTask = new CalendarTask();

            testTask.Name = Guid.NewGuid().ToString();
            testTask.Frequency = eTaskFrequency.Monthly;
            testTask.ResultType = eResultType.Decimal;
            testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
            testTask.UserId = testUserId;
            testTask.AbsolutePosition = 1;
            testTask.CalendarTaskId = Guid.NewGuid().ToString();

            // ACT
            var id = mongoCalendarTaskService.InsertTaskAsync(testTask).Result;

            // ASSERT
            var task = mongoCalendarTaskService.GetTaskAsync(id).Result;
            Assert.AreEqual(testTask.Name, task.Name);
            CollectionAssert.AreEqual(testTask.RequiredDays, task.RequiredDays);
            Assert.AreEqual(testTask.ResultType, task.ResultType);
            Assert.AreEqual(testTask.UserId, task.UserId);
            Assert.AreEqual(testTask.AbsolutePosition, task.AbsolutePosition);
            Assert.AreEqual(testTask.Frequency, task.Frequency);
            Assert.IsTrue(task.InsertDate != null && task.InsertDate.Value > DateTime.MinValue);
        }

        [TestMethod]
        public void Mongo_UpdateAbsolutePositionToFirst_ShouldIncrementOtherTasks()
        {
            // ARRANGE
            var ids = new List<string>();
            var tasks = new List<DTOCalendarTask>();
            for (int i=1; i<5; i++)
            {
                var testTask = new DTOCalendarTask();

                testTask.Name = Guid.NewGuid().ToString();
                testTask.Frequency = eTaskFrequency.Monthly;
                testTask.ResultType = eResultType.Decimal;
                testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
                testTask.UserId = testUserId;
                testTask.AbsolutePosition = i;
                testTask.CalendarTaskId = Guid.NewGuid().ToString(); 
                var id = mongoCalendarTaskService.InsertTaskAsync(testTask).Result;

                tasks.Add(testTask);
            }

            Assert.AreEqual(1, tasks[0].AbsolutePosition);
            Assert.AreEqual(2, tasks[1].AbsolutePosition);
            Assert.AreEqual(3, tasks[2].AbsolutePosition);
            Assert.AreEqual(4, tasks[3].AbsolutePosition);

            // ACT (Take the last one, put it in first)
            var lastTask = tasks.Last();
            lastTask.AbsolutePosition = 1;
            lastTask.InitialAbsolutePosition = 4;
            var result = mongoCalendarTaskService.UpdateTaskAsync(lastTask).Result;

            // ASSERT
            var updatedTasks = mongoCalendarTaskService.GetTasksAsync(testUserId).Result;
            Assert.AreEqual(2, updatedTasks.First(p => p.CalendarTaskId == tasks[0].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(3, updatedTasks.First(p => p.CalendarTaskId == tasks[1].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(4, updatedTasks.First(p => p.CalendarTaskId == tasks[2].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(1, updatedTasks.First(p => p.CalendarTaskId == tasks[3].CalendarTaskId).AbsolutePosition);
        }

        [TestMethod]
        public void Mongo_InsertAbsolutePosition_ShouldIncrementOtherTasks()
        {
            // ARRANGE
            var ids = new List<string>();
            var tasks = new List<DTOCalendarTask>();
            for (int i = 1; i < 5; i++)
            {
                var testTask = new DTOCalendarTask();

                testTask.Name = Guid.NewGuid().ToString();
                testTask.Frequency = eTaskFrequency.Monthly;
                testTask.ResultType = eResultType.Decimal;
                testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
                testTask.UserId = testUserId;
                testTask.AbsolutePosition = i;
                testTask.CalendarTaskId = Guid.NewGuid().ToString(); 
                var id2 = mongoCalendarTaskService.InsertTaskAsync(testTask).Result;

                tasks.Add(testTask);
            }

            Assert.AreEqual(1, tasks[0].AbsolutePosition);
            Assert.AreEqual(2, tasks[1].AbsolutePosition);
            Assert.AreEqual(3, tasks[2].AbsolutePosition);
            Assert.AreEqual(4, tasks[3].AbsolutePosition);

            // ACT (Create a Fifth one, put it at position #2)
            var testTask2 = new DTOCalendarTask();

            testTask2.Name = Guid.NewGuid().ToString();
            testTask2.Frequency = eTaskFrequency.Monthly;
            testTask2.ResultType = eResultType.Decimal;
            testTask2.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
            testTask2.UserId = testUserId;
            testTask2.AbsolutePosition = 2;
            testTask2.CalendarTaskId = Guid.NewGuid().ToString(); 
            var id = mongoCalendarTaskService.InsertTaskAsync(testTask2).Result;

            tasks.Add(testTask2);

            // ASSERT
            var updatedTasks = mongoCalendarTaskService.GetTasksAsync(testUserId).Result;
            Assert.AreEqual(1, updatedTasks.First(p => p.CalendarTaskId == tasks[0].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(3, updatedTasks.First(p => p.CalendarTaskId == tasks[1].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(4, updatedTasks.First(p => p.CalendarTaskId == tasks[2].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(5, updatedTasks.First(p => p.CalendarTaskId == tasks[3].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(2, updatedTasks.First(p => p.CalendarTaskId == tasks[4].CalendarTaskId).AbsolutePosition);
        }


        [TestMethod]
        public void Mongo_UpdateAbsolutePositionToLast_ShouldDecrementOtherTasks()
        {
            // ARRANGE
            var ids = new List<string>();
            var tasks = new List<DTOCalendarTask>();
            for (int i = 1; i < 5; i++)
            {
                var testTask = new DTOCalendarTask();

                testTask.Name = Guid.NewGuid().ToString();
                testTask.Frequency = eTaskFrequency.Monthly;
                testTask.ResultType = eResultType.Decimal;
                testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
                testTask.UserId = testUserId;
                testTask.AbsolutePosition = i;
                testTask.CalendarTaskId = Guid.NewGuid().ToString(); 
                var id = mongoCalendarTaskService.InsertTaskAsync(testTask).Result;

                tasks.Add(testTask);
            }

            Assert.AreEqual(1, tasks[0].AbsolutePosition);
            Assert.AreEqual(2, tasks[1].AbsolutePosition);
            Assert.AreEqual(3, tasks[2].AbsolutePosition);
            Assert.AreEqual(4, tasks[3].AbsolutePosition);

            // ACT (Take the first one, put it in last)
            var lastTask = tasks.First();
            lastTask.AbsolutePosition = 4;
            lastTask.InitialAbsolutePosition = 1;
            var result = mongoCalendarTaskService.UpdateTaskAsync(lastTask).Result;

            // ASSERT
            var updatedTasks = mongoCalendarTaskService.GetTasksAsync(testUserId).Result;
            Assert.AreEqual(4, updatedTasks.First(p => p.CalendarTaskId == tasks[0].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(1, updatedTasks.First(p => p.CalendarTaskId == tasks[1].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(2, updatedTasks.First(p => p.CalendarTaskId == tasks[2].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(3, updatedTasks.First(p => p.CalendarTaskId == tasks[3].CalendarTaskId).AbsolutePosition);
        }

        [TestMethod]
        public void Mongo_InsertAtTheEnd_ShouldNotIncrementOtherTasks()
        {
            // ARRANGE + ACT
            var ids = new List<string>();
            var tasks = new List<DTOCalendarTask>();
            for (int i = 1; i < 3; i++)
            {
                var testTask = new DTOCalendarTask();

                testTask.Name = Guid.NewGuid().ToString();
                testTask.Frequency = eTaskFrequency.Monthly;
                testTask.ResultType = eResultType.Decimal;
                testTask.RequiredDays = new List<System.DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday };
                testTask.UserId = testUserId;
                testTask.AbsolutePosition = i;
                testTask.CalendarTaskId = Guid.NewGuid().ToString(); 
                var id = mongoCalendarTaskService.InsertTaskAsync(testTask).Result;

                tasks.Add(testTask);
            }

            Assert.AreEqual(1, tasks[0].AbsolutePosition);
            Assert.AreEqual(2, tasks[1].AbsolutePosition);

            // ASSERT
            var updatedTasks = mongoCalendarTaskService.GetTasksAsync(testUserId).Result;
            Assert.AreEqual(1, updatedTasks.First(p => p.CalendarTaskId == tasks[0].CalendarTaskId).AbsolutePosition);
            Assert.AreEqual(2, updatedTasks.First(p => p.CalendarTaskId == tasks[1].CalendarTaskId).AbsolutePosition);
        }
    }
}
