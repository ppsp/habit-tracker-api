using HyperTaskCore.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HyperTaskTest
{
    public partial class CalendarTaskApiTest
    {
        [TestMethod]
        public void Mongo_InsertTaskGroupAsync_ShouldReturnId()
        {
            MongoDeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            
            // ACT
            var id = this.mongoTaskGroupService.InsertGroupAsync(testGroup).Result;

            // ASSERT
            Assert.IsTrue(id != null && id.Length > 0);

            MongoDeleteAllGroups();
        }

        [TestMethod]
        public void Mongo_GetTaskGroupAsync_ShouldReturnSameValuesAsInsert()
        {
            MongoDeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            var firebaseId = this.mongoTaskGroupService.InsertGroupAsync(testGroup).Result;

            // ACT
            var retrievedGroup = this.mongoTaskGroupService.GetGroupAsync(testGroup.GroupId).Result;

            // ASSERT
            Assert.IsTrue(AssertValuesAreTheSame(testGroup, retrievedGroup));

            MongoDeleteAllGroups();
        }

        [TestMethod]
        public void Mongo_UpdateTaskGroupAsync_ShouldReturnTrue()
        {
            MongoDeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            var id = this.mongoTaskGroupService.InsertGroupAsync(testGroup).Result;

            // ACT
            var updatedGroup = this.mongoTaskGroupService.GetGroupAsync(testGroup.GroupId).Result;
            updatedGroup.Name = "NewName2";
            updatedGroup.Position = 32;
            updatedGroup.Void = true;
            updatedGroup.ColorHex = "ABBABC";

            var success1 = this.mongoTaskGroupService.UpdateGroupAsync(updatedGroup).Result;

            // ASSERT
            Assert.IsTrue(success1);

            MongoDeleteAllGroups();
        }

        [TestMethod]
        public void Mongo_UpdateTaskGroupAsync_ShouldUpdateValues()
        {
            MongoDeleteAllGroups();

            // ARRANGE
            var testGroup = getTestTaskGroup();
            var id = this.mongoTaskGroupService.InsertGroupAsync(testGroup).Result;

            // ACT
            var updatedGroup = this.mongoTaskGroupService.GetGroupAsync(testGroup.GroupId).Result;
            updatedGroup.Name = "NewName2";
            updatedGroup.Position = 32;
            updatedGroup.Void = true;
            updatedGroup.ColorHex = "ABBABC";

            var success1 = this.mongoTaskGroupService.UpdateGroupAsync(updatedGroup).Result;
            var retrievedGroup = this.mongoTaskGroupService.GetGroupAsync(testGroup.GroupId).Result;

            // ASSERT
            Assert.IsTrue(AssertValuesAreTheSame(updatedGroup, retrievedGroup));

            MongoDeleteAllGroups();
        }


        [TestMethod]
        public void Mongo_GetTaskGroupsAsync_ShouldReturnSameValuesAsInsert()
        {
            MongoDeleteAllGroups();

            // ARRANGE
            var testGroup1 = getTestTaskGroup();
            var testGroup2 = getTestTaskGroup();
            testGroup2.Position = 2;
            var testGroup3 = getTestTaskGroup();
            testGroup3.Position = 3;
            testGroup1.Id = this.fireTaskGroupService.InsertGroupAsync(testGroup1).Result;
            testGroup2.Id = this.fireTaskGroupService.InsertGroupAsync(testGroup2).Result;
            testGroup3.Id = this.fireTaskGroupService.InsertGroupAsync(testGroup3).Result;

            // ACT
            var retrievedGroups = this.fireTaskGroupService.GetGroupsAsync(testUserId, true).Result;

            // ASSERT
            Assert.IsTrue(retrievedGroups.Count == 3);
            Assert.IsTrue(AssertValuesAreTheSame(testGroup1, retrievedGroups.Single(p => p.GroupId == testGroup1.GroupId)));
            Assert.IsTrue(AssertValuesAreTheSame(testGroup2, retrievedGroups.Single(p => p.GroupId == testGroup2.GroupId)));
            Assert.IsTrue(AssertValuesAreTheSame(testGroup3, retrievedGroups.Single(p => p.GroupId == testGroup3.GroupId)));

            MongoDeleteAllGroups();
        }

        private void Mongo_DeleteAllGroups()
        {
            var groups = this.mongoTaskGroupService.GetGroupsAsync(testUserId, true).Result;

            foreach (var group in groups)
            {
                var result = this.mongoTaskGroupService.DeleteGroupAsync(group.GroupId).Result;
            }
        }
    }
}
