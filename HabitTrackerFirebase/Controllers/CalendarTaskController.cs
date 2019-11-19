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
            var tasks = await CalendarTaskService.GetAsync(userId);
            return tasks.Select(p => new DTOCalendarTask(p)).ToList();
        }

        // POST 
        [HttpPost]
        public async Task<bool> Post([FromBody]DTOCalendarTask task)
        {
            return await CalendarTaskService.InsertTaskAsync(new FireCalendarTask(task));
        }

        // PUT
        [HttpPut]
        public async Task<bool> Put([FromBody]DTOCalendarTask task)
        {
            // TODO: Move this logic in service layer
            int? initialAbsolutePosition = null;
            if (task.InitialAbsolutePosition != task.AbsolutePosition)
                initialAbsolutePosition = task.InitialAbsolutePosition;
            else if (task.Void)
            {
                initialAbsolutePosition = task.InitialAbsolutePosition;
                task.AbsolutePosition = 50000; // TODO: Refactor to remove the necessity of this arbitrary number
            }
                
            return await CalendarTaskService.UpdateTaskAsync(new FireCalendarTask(task), initialAbsolutePosition);
        }
    }
}