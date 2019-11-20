using HabitTrackerCore.Services;
using HabitTrackerServices;
using HabitTrackerServices.Models.DTO;
using HabitTrackerServices.Models.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabitTrackerFirebase.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CalendarTaskController : ControllerBase
    {
        private ICalendarTaskService CalendarTaskService { get; set; }

        public CalendarTaskController()
        {
            CalendarTaskService = new CalendarTaskService();
        }

        // GET
        [HttpGet]
        public async Task<List<DTOCalendarTask>> Get(string userId)
        {
            var tasks = await CalendarTaskService.GetTasksAsync(userId);
            return tasks.Select(p => new DTOCalendarTask(p)).ToList();
        }

        // POST
        [HttpPost]
        public async Task<bool> Post([FromBody]DTOCalendarTask task)
        {
            var result = await CalendarTaskService.InsertTaskAsync(task);
            return result != null;
        }

        // PUT
        [HttpPut]
        public async Task<bool> Put([FromBody]DTOCalendarTask task)
        {
            return await CalendarTaskService.UpdateTaskAsync(task);
        }
    }
}