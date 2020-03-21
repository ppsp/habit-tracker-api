using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerServices.Services;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HabitTrackerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class TaskHistoryController : ControllerBase
    {
        private ITaskHistoryService TaskHistoryService { get; set; }

        public TaskHistoryController(CalendarTaskService calendarTaskService)
        {
            TaskHistoryService = new TaskHistoryService(calendarTaskService);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TaskHistory task)
        {
            var result = await TaskHistoryService.InsertHistoryAsync(task);
            return Ok(result);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]TaskHistory task)
        {
            var result = await TaskHistoryService.UpdateHistoryAsync(task);
            return Ok(result);
        }
    }
}