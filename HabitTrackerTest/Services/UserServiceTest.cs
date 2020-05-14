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
            var success = userService.InsertUpdateUserAsync(testUser).Result;

            // ASSERT
            Assert.IsTrue(success);

            // Cleanup
            DeleteUser(testUser);
        }

        /*[TestMethod]
        public void GetUserAsync_ShouldReturnSameValuesAsInsert()
        {
            // ARRANGE
            var testUser = getTestUser();
            var success = userService.InsertUpdateUserAsync(testUser).Result;

            // ACT
            var user = userService.GetUserAsync(testUser.UserId).Result;

            // ASSERT
            AssertValuesAreTheSame(testUser, user);

            // Cleanup
            DeleteUser(testUser);
        }*/

       /* [TestMethod]
        public void UpdateUserAsync_ShouldReturnSameValues()
        {
            // ARRANGE
            var testUser = getTestUser();
            var success = userService.InsertUpdateUserAsync(testUser).Result;
            var insertedUser = userService.GetUserAsync(testUser.UserId).Result;

            // ACT
            insertedUser.Config.PreferedLanguage = eLanguage.English;
            success = userService.InsertUpdateUserAsync(insertedUser).Result;
            var updatedUser = userService.GetUserAsync(testUser.UserId).Result;

            // ASSERT
            AssertValuesAreTheSame(insertedUser, updatedUser);

            // Cleanup
            DeleteUser(testUser);
        }*/

        [TestMethod]
        public void DeleteUserAsync_ShouldReturnTrue()
        {
            // ARRANGE
            var testUser = getTestUser();
            var successInsert = userService.InsertUpdateUserAsync(testUser).Result;
            var insertedUser = userService.GetUserAsync(testUser.UserId).Result;

            // ACT
            var successDelete = userService.DeleteUserAsync(insertedUser.Id).Result;

            // ASSERT
            Assert.IsTrue(successDelete);

            // Cleanup
            DeleteUser(testUser);
        }

        [TestMethod]
        public void DeleteUserAsync_ShouldDeleteUser()
        {
            // ARRANGE
            var testUser = getTestUser();
            var successInsert = userService.InsertUpdateUserAsync(testUser).Result;
            var insertedUser = userService.GetUserAsync(testUser.UserId).Result;

            // ACT
            var successDelete = userService.DeleteUserAsync(insertedUser.Id).Result;
            insertedUser = userService.GetUserAsync(testUser.UserId).Result;

            // ASSERT
            Assert.IsTrue(insertedUser is NULLUser);

            // Cleanup
            DeleteUser(testUser);
        }

        /*private static void AssertValuesAreTheSame(IUser user1, IUser user2)
        {
            Assert.AreEqual(user1.UserId, user2.UserId);
            Assert.AreEqual(user1.Config.PreferedLanguage, user2.Config.PreferedLanguage);
        }*/

        private static User getTestUser()
        {
            var testUser = new User();

            testUser.UserId = Guid.NewGuid().ToString();
            // testUser.Config.PreferedLanguage = eLanguage.French;
            
            return testUser;
        }

        private void DeleteUser(IUser user)
        {
            var result = userService.DeleteUserAsync(user.UserId).Result;
        }
    }
}
