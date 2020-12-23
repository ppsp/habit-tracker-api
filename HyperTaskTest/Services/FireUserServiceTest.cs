using HyperTaskCore.Models;
using HyperTaskServices.Caching;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperTaskTest
{
    public partial class CalendarTaskApiTest
    {
        [TestMethod]
        public void UpdateUserAsync_ShouldReturnTrue()
        {
            // ARRANGE
            var testUser = getTestUser();

            // ACT
            var success = fireUserService.InsertUpdateUserAsync(testUser).Result;

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
            var successInsert = fireUserService.InsertUpdateUserAsync(testUser).Result;
            var insertedUser = fireUserService.GetUserAsync(testUser.UserId).Result;

            // ACT
            var successDelete = fireUserService.DeleteUserAsync(insertedUser.Id).Result;

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
            var successInsert = fireUserService.InsertUpdateUserAsync(testUser).Result;
            var insertedUser = fireUserService.GetUserAsync(testUser.UserId).Result;

            // ACT
            var successDelete = fireUserService.DeleteUserAsync(insertedUser.Id).Result;
            insertedUser = fireUserService.GetUserAsync(testUser.UserId).Result;

            // ASSERT
            Assert.IsTrue(insertedUser is NULLUser);

            // Cleanup
            DeleteUser(testUser);
        }

        /*[TestMethod]
        public void DeleteInactiveAccounts()
        {
            var users = userService.GetInactiveAccounts().Result;
            var minimumActivityDate = DateTime.Today.AddYears(-1);
            var originalPP = userService.GetUserAsync("NSm32K4BF6Y7NFc2kwqWeGmy6KG2").Result;
            var originalJoachim = userService.GetUserAsync("dO6TkLTit3WDMSOe3lH1gRiBQXF3").Result;

            foreach (var user in users)
            {
                var userId = user.UserId;

                if (user.LastActivityDate == null)
                {
                    if (user.UserId == "NSm32K4BF6Y7NFc2kwqWeGmy6KG2")
                    {
                        if (originalPP.Id == user.Id)
                        {
                            var tasks = calendarTaskService.GetTasksAsync(user.UserId, true).Result;

                            var dates = tasks.SelectMany(p => p.Histories.Count == 0 ?
                                     new List<DateTime>()
                                     {
                                                                     p.InsertDate == null ?
                                                                        DateTime.MinValue :
                                                                        p.InsertDate.Value
                                     } :
                                     p.Histories.Select(t => t.InsertDate == null ?
                                                                 DateTime.MinValue :
                                                                 t.InsertDate.Value));

                            var maxInsertDate = dates.Count() == 0 ? null : (DateTime?)dates.OrderByDescending(p => p).First();

                            user.LastActivityDate = maxInsertDate.Value.Date.ToUniversalTime();
                            var result = userService.InsertUpdateUserAsync(user).Result;
                        }
                        else
                        {
                            var result = userService.DeleteUserWithFireBaseIdAsync(user.Id).Result;
                        }
                    } 
                    else if (user.UserId == "dO6TkLTit3WDMSOe3lH1gRiBQXF3")
                    {
                        if (originalJoachim.Id == user.Id)
                        {
                            var tasks = calendarTaskService.GetTasksAsync(user.UserId, true).Result;

                            var dates = tasks.SelectMany(p => p.Histories.Count == 0 ?
                                     new List<DateTime>()
                                     {
                                                                     p.InsertDate == null ?
                                                                        DateTime.MinValue :
                                                                        p.InsertDate.Value
                                     } :
                                     p.Histories.Select(t => t.InsertDate == null ?
                                                                 DateTime.MinValue :
                                                                 t.InsertDate.Value));

                            var maxInsertDate = dates.Count() == 0 ? null : (DateTime?)dates.OrderByDescending(p => p).First();

                            if (maxInsertDate != null)
                            {
                                user.LastActivityDate = maxInsertDate.Value.Date.ToUniversalTime();
                                var result = userService.InsertUpdateUserAsync(user).Result;
                            }
                        }
                        else
                        {
                            var result = userService.DeleteUserWithFireBaseIdAsync(user.Id).Result;
                        }
                    }
                    else
                    {
                        var tasks = calendarTaskService.GetTasksAsync(user.UserId, true).Result;

                        if (tasks.Count == 0)
                        {
                            // DELETE ZERO TASK INSERTED
                            var result = userService.PermaDeleteUser(user).Result;
                        }
                        else
                        {
                            var dates = tasks.SelectMany(p => p.Histories.Count == 0 ?
                                                                 new List<DateTime>() 
                                                                 { 
                                                                     p.InsertDate == null ? 
                                                                        DateTime.MinValue : 
                                                                        p.InsertDate.Value 
                                                                 } :
                                                                 p.Histories.Select(t => t.InsertDate == null ?
                                                                                             DateTime.MinValue :
                                                                                             t.InsertDate.Value));

                            var maxInsertDate = dates.Count() == 0 ? null : (DateTime?)dates.OrderByDescending(p => p).First();

                            if (maxInsertDate < minimumActivityDate)
                            {
                                // DELETE EXPIRED
                                var result = userService.PermaDeleteUser(user).Result;
                            }
                            else
                            {
                                user.LastActivityDate = maxInsertDate.Value.Date.ToUniversalTime();
                                var result = userService.InsertUpdateUserAsync(user).Result;
                            }
                        }
                    }
                }
            }
        }*/

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
            var result = fireUserService.DeleteUserAsync(user.UserId).Result;
        }
    }
}
