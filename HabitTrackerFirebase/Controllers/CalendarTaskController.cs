using FirebaseAdmin.Auth;
using HabitTrackerCore.Services;
using HabitTrackerServices;
using HabitTrackerServices.Models.DTO;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HabitTrackerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [CustomAuthorizeAttribute]
    public class CalendarTaskController : ControllerBase
    {
        private ICalendarTaskService CalendarTaskService { get; set; }

        public CalendarTaskController()
        {
            CalendarTaskService = new CalendarTaskService();
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get(string userId)
        {
            var tasks = await CalendarTaskService.GetTasksAsync(userId);
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