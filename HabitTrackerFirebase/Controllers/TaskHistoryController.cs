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
    [AuthorizeJwt]
    public class TaskHistoryController : ControllerBase
    {
        private ITaskHistoryService TaskHistoryService { get; set; }

        public TaskHistoryController()
        {
            TaskHistoryService = new TaskHistoryService();
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get(string userId)
        {
            var tasks = await TaskHistoryService.GetHistoriesAsync(userId, false);
            return Ok(tasks);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TaskHistory task)
        {
            var result = await TaskHistoryService.InsertHistoryAsync(task);
            return Ok(result != null);
        }
    }
}