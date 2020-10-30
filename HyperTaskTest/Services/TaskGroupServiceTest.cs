using HyperTaskCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HyperTaskTest
{
    public partial class CalendarTaskApiTest
    {
        [TestMethod]
        public void InsertTaskGroupAsync_ShouldReturnId()
        {
            DeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            
            // ACT
            var id = this.taskGroupService.InsertGroupAsync(testGroup).Result;

            // ASSERT
            Assert.IsTrue(id != null && id.Length > 0);

            DeleteAllGroups();
        }

        [TestMethod]
        public void GetTaskGroupAsync_ShouldReturnSameValuesAsInsert()
        {
            DeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            var firebaseId = this.taskGroupService.InsertGroupAsync(testGroup).Result;

            // ACT
            var retrievedGroup = this.taskGroupService.GetGroupAsync(testGroup.GroupId).Result;

            // ASSERT
            Assert.IsTrue(AssertValuesAreTheSame(testGroup, retrievedGroup));

            DeleteAllGroups();
        }

        private static bool AssertValuesAreTheSame(TaskGroup taskGroup1, TaskGroup taskGroup2)
        {
            Assert.AreEqual(taskGroup1.ColorHex, taskGroup2.ColorHex);
            Assert.AreEqual(taskGroup1.GroupId, taskGroup2.GroupId);
            Assert.AreEqual(taskGroup1.Name, taskGroup2.Name);
            Assert.AreEqual(taskGroup1.Position, taskGroup2.Position);
            Assert.AreEqual(taskGroup1.Id, taskGroup2.Id);
            Assert.AreEqual(taskGroup1.UserId, taskGroup2.UserId);
            Assert.AreEqual(taskGroup1.Void, taskGroup2.Void);

            return true;
        }

        [TestMethod]
        public void UpdateTaskGroupAsync_ShouldReturnTrue()
        {
            DeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            var id = this.taskGroupService.InsertGroupAsync(testGroup).Result;

            // ACT
            var updatedGroup = this.taskGroupService.GetGroupAsync(testGroup.GroupId).Result;
            updatedGroup.Name = "NewName2";
            updatedGroup.Position = 32;
            updatedGroup.Void = true;
            updatedGroup.ColorHex = "ABBABC";

            var success1 = this.taskGroupService.UpdateGroupAsync(updatedGroup).Result;

            // ASSERT
            Assert.IsTrue(success1);

            DeleteAllGroups();
        }

        [TestMethod]
        public void UpdateTaskGroupAsync_ShouldUpdateValues()
        {
            DeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            var id = this.taskGroupService.InsertGroupAsync(testGroup).Result;

            // ACT
            var updatedGroup = this.taskGroupService.GetGroupAsync(testGroup.GroupId).Result;
            updatedGroup.Name = "NewName2";
            updatedGroup.Position = 32;
            updatedGroup.Void = true;
            updatedGroup.ColorHex = "ABBABC";

            var success1 = this.taskGroupService.UpdateGroupAsync(updatedGroup).Result;
            var retrievedGroup = this.taskGroupService.GetGroupAsync(testGroup.GroupId).Result;

            // ASSERT
            Assert.IsTrue(AssertValuesAreTheSame(updatedGroup, retrievedGroup));

            DeleteAllGroups();
        }


        [TestMethod]
        public void GetTaskGroupsAsync_ShouldReturnSameValuesAsInsert()
        {
            DeleteAllGroups();

            // ARRANGE
            var testGroup1 = getTestTaskGroup();
            var testGroup2 = getTestTaskGroup();
            testGroup2.Position = 2;
            var testGroup3 = getTestTaskGroup();
            testGroup3.Position = 3;
            testGroup1.Id = this.taskGroupService.InsertGroupAsync(testGroup1).Result;
            testGroup2.Id = this.taskGroupService.InsertGroupAsync(testGroup2).Result;
            testGroup3.Id = this.taskGroupService.InsertGroupAsync(testGroup3).Result;

            // ACT
            var retrievedGroups = this.taskGroupService.GetGroupsAsync(testUserId, true).Result;

            // ASSERT
            Assert.IsTrue(retrievedGroups.Count == 3);
            Assert.IsTrue(AssertValuesAreTheSame(testGroup1, retrievedGroups.Single(p => p.GroupId == testGroup1.GroupId)));
            Assert.IsTrue(AssertValuesAreTheSame(testGroup2, retrievedGroups.Single(p => p.GroupId == testGroup2.GroupId)));
            Assert.IsTrue(AssertValuesAreTheSame(testGroup3, retrievedGroups.Single(p => p.GroupId == testGroup3.GroupId)));

            DeleteAllGroups();
        }

        private void DeleteAllGroups()
        {
            var groups = this.taskGroupService.GetGroupsAsync(testUserId, true).Result;

            foreach (var group in groups)
            {
                var result = this.taskGroupService.DeleteGroupAsync(group.GroupId).Result;
            }
        }

        private static TaskGroup getTestTaskGroup()
        {
            var testGroup = new TaskGroup();

            testGroup.Name = Guid.NewGuid().ToString(); ;
            testGroup.GroupId = Guid.NewGuid().ToString();
            testGroup.ColorHex = "FFFFFF";
            testGroup.Position = 1;
            testGroup.UserId = testUserId;

            return testGroup;
        }
    }
}
