using HabitTrackerCore.DAL;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerServices.DAL;
using HabitTrackerServices.Services;
using HabitTrackerTools;
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

        public TaskHistoryController(CachingManager cachingManager,
                                     DALTaskHistory dalTaskHistory)
        {
            TaskHistoryService = new TaskHistoryService(dalTaskHistory, cachingManager);
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get(string userId)
        {
            var tasks = await TaskHistoryService.GetHistoriesAsync(new GetCalendarTaskRequest()
            {
                UserId = userId,
                IncludeVoid = false
            });
            return Ok(tasks);
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