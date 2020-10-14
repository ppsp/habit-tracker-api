using HyperTaskCore.Models;
using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HyperTaskWebApi.Controllers
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
                                     CalendarTaskService calendarTaskService,
                                     TaskGroupService taskGroupService)
        {
            TaskHistoryService = new TaskHistoryService(calendarTaskService);
            UserService = new UserService(connector, calendarTaskService, taskGroupService);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TaskHistory task)
        {
            await UserService.UpdateLastActivityDate(task.UserId, task.UpdateDate ?? DateTime.Now);

            var result = await TaskHistoryService.InsertHistoryAsync(task);
            return Ok(result);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]TaskHistory task)
        {
            await UserService.UpdateLastActivityDate(task.UserId, task.UpdateDate ?? DateTime.Now);

            var result = await TaskHistoryService.UpdateHistoryAsync(task);
            return Ok(result);
        }
    }
}