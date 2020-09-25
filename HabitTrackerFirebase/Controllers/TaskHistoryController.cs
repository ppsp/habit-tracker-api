using HabitTrackerCore.Models;
using HabitTrackerServices.Services;
using HabitTrackerTools;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HabitTrackerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class TaskHistoryController : ControllerBase
    {
        private TaskHistoryService TaskHistoryService { get; set; }
        private UserService UserService { get; set; }

        public TaskHistoryController(FirebaseConnector connector,
                                     CalendarTaskService calendarTaskService)
        {
            TaskHistoryService = new TaskHistoryService(calendarTaskService);
            UserService = new UserService(connector, calendarTaskService);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TaskHistory task)
        {
            await UserService.UpdateLastActivityDate(task.UserId);

            var result = await TaskHistoryService.InsertHistoryAsync(task);
            return Ok(result);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]TaskHistory task)
        {
            await UserService.UpdateLastActivityDate(task.UserId);

            var result = await TaskHistoryService.UpdateHistoryAsync(task);
            return Ok(result);
        }
    }
}