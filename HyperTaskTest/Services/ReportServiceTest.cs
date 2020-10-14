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
        public void GetAllData_ShouldReturnAFile()
        {
            // ARRANGE
            // var testUser = getTestUser();

            // ACT
            // var success = userService.InsertUpdateUserAsync(testUser).Result;

            var file = this.reportService.GetTasksCsv("NSm32K4BF6Y7NFc2kwqWeGmy6KG2").Result;

            // ASSERT
            Assert.IsTrue(file.Length > 0);

            // Cleanup
            // DeleteUser(testUser);
        }
    }
}
