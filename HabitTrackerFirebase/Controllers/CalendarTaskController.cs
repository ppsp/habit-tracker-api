using HabitTrackerCore.DAL;
using HabitTrackerCore.Services;
using HabitTrackerServices.DAL;
using HabitTrackerServices.Models.DTO;
using HabitTrackerServices.Services;
using HabitTrackerTools;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HabitTrackerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class CalendarTaskController : ControllerBase
    {
        private ICalendarTaskService CalendarTaskService { get; set; }
        private ITaskHistoryService TaskHistoryService { get; set; }

        public CalendarTaskController(FirebaseConnector connector,
                                      CachingManager cachingManager,
                                      IDALTaskHistory dalTaskHistory)
        {
            CalendarTaskService = new CalendarTaskService(connector);
            TaskHistoryService = new TaskHistoryService(dalTaskHistory, cachingManager);
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]DTOGetCalendarTaskRequest dtoRequest)
        {
            var tasks = await CalendarTaskService.GetTasksAsync(dtoRequest.userId);
            var histories = await TaskHistoryService.GetHistoriesAsync(dtoRequest.ToCalendarTaskRequest());

            foreach (var task in tasks)
                task.Histories = histories.Where(p => p.CalendarTaskId == task.CalendarTaskId);

            return Ok(tasks.Select(p => new DTOCalendarTask(p)).ToList());
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]DTOCalendarTask task)
        {
            var result = await CalendarTaskService.InsertTaskAsync(task);
            return Ok(result != null);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]DTOCalendarTask task)
        {
            var result = await CalendarTaskService.UpdateTaskAsync(task);
            return Ok(result);
        }
    }
}