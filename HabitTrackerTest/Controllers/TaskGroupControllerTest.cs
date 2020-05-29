using HabitTrackerCore.Models;
using HabitTrackerServices.Caching;
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
    public partial class CalendarTaskApiTest
    {
        [TestMethod]
        public void TaskGroup_Post_ShouldReturnIdAndCode200()
        {
            DeleteAllGroups();

            // ARRANGE
            var testTaskGroup = DTOTaskGroup.FromTaskGroup(getTestTaskGroup());

            // ACT
            var response = taskGroupController.Post(testTaskGroup).Result;
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsTrue(okResult.Value is string);
            Assert.IsTrue((okResult.Value as string).Length > 0);
            Assert.AreEqual(200, okResult.StatusCode);
            
            DeleteAllGroups();
        }

        [TestMethod]
        public void TaskGroup_Get_ShouldReturnTaskGroup()
        {
            DeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            testGroup.Id = this.taskGroupService.InsertGroupAsync(testGroup).Result;

            // var retrievedGroup = this.taskGroupService.GetGroupAsync(testGroup.GroupId).Result;

            // ACT
            var response = taskGroupController.Get(testGroup.UserId).Result;
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsTrue(okResult.Value is List<DTOTaskGroup>);
            var dtoTaskGroup = okResult.Value as List<DTOTaskGroup>;
            Assert.IsTrue(AssertValuesAreTheSame(testGroup, dtoTaskGroup[0].ToTaskGroup()));
            Assert.AreEqual(200, okResult.StatusCode);

            DeleteAllGroups();
        }

        [TestMethod]
        public void TaskGroup_Put_ShouldReturnTrue()
        {
            DeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            testGroup.Id = this.taskGroupService.InsertGroupAsync(testGroup).Result;

            // var retrievedGroup = this.taskGroupService.GetGroupAsync(testGroup.GroupId).Result;

            // ACT
            var updatedGroup = this.taskGroupService.GetGroupAsync(testGroup.GroupId).Result;
            updatedGroup.GroupName = "NewName2";
            updatedGroup.GroupPosition = 32;
            updatedGroup.Void = true;
            updatedGroup.ColorHex = "ABBABC";

            var response = taskGroupController.Put(DTOTaskGroup.FromTaskGroup(updatedGroup)).Result;
            var okResult = response as OkObjectResult;

            // ASSERT
            Assert.IsTrue(okResult.Value is bool);
            Assert.IsTrue((bool)okResult.Value);
            Assert.AreEqual(200, okResult.StatusCode);

            DeleteAllGroups();
        }
    }
}
