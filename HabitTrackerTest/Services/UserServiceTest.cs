using HabitTrackerCore.Models;
using HabitTrackerServices.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HabitTrackerTest
{
    public partial class CalendarTaskApiTest
    {
        [TestMethod]
        public void UpdateUserAsync_ShouldReturnTrue()
        {
            // ARRANGE
            var testUser = getTestUser();

            // ACT
            var success = userService.UpdateUserAsync(testUser).Result;

            // ASSERT
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void GetUserAsync_ShouldReturnSameValuesAsInsert()
        {
            // ARRANGE
            var testUser = getTestUser();
            var success = userService.UpdateUserAsync(testUser).Result;

            // ACT
            var user = userService.GetUserAsync(testUser.Id).Result;

            // ASSERT
            AssertValuesAreTheSame(testUser, user);
        }

        [TestMethod]
        public void UpdateUserAsync_ShouldReturnSameValues()
        {
            // ARRANGE
            var testUser = getTestUser();
            var success = userService.UpdateUserAsync(testUser).Result;
            var insertedUser = userService.GetUserAsync(testUser.Id).Result;

            // ACT
            insertedUser.PreferedLanguage = eLanguage.English;
            success = userService.UpdateUserAsync(testUser).Result;
            var updatedUser = userService.GetUserAsync(testUser.Id).Result;

            // ASSERT
            AssertValuesAreTheSame(insertedUser, updatedUser);
        }

        [TestMethod]
        public void DeleteUserAsync_ShouldReturnTrue()
        {
            // ARRANGE
            var testUser = getTestUser();
            var successInsert = userService.UpdateUserAsync(testUser).Result;

            // ACT
            var successDelete = userService.DeleteUserAsync(testUser.Id).Result;

            // ASSERT
            Assert.IsTrue(successDelete);
        }

        [TestMethod]
        public void DeleteUserAsync_ShouldDeleteUser()
        {
            // ARRANGE
            var testUser = getTestUser();
            var successInsert = userService.UpdateUserAsync(testUser).Result;

            // ACT
            var successDelete = userService.DeleteUserAsync(testUser.Id).Result;
            var insertedUser = userService.GetUserAsync(testUser.Id).Result;

            // ASSERT
            Assert.IsTrue(insertedUser == null);
        }

        private static void AssertValuesAreTheSame(IUser user1, IUser user2)
        {
            Assert.AreEqual(user1.Id, user2.Id);
            Assert.AreEqual(user1.PreferedLanguage, user2.PreferedLanguage);
        }

        private static User getTestUser()
        {
            var testUser = new User();

            testUser.Id = Guid.NewGuid().ToString();
            testUser.PreferedLanguage = eLanguage.French;
            
            return testUser;
        }
    }
}
