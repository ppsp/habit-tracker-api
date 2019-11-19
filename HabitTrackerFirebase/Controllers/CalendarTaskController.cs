using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HabitTrackerCore.Models;
using HabitTrackerCore.Services;
using HabitTrackerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return await CalendarTaskService.InsertTaskAsync(new CalendarTask(task));
        }

        // PUT
        [HttpPut]
        public async Task<bool> Put([FromBody]DTOCalendarTask task)
        {
            int? initialAbsolutePosition = task.InitialAbsolutePosition == task.AbsolutePosition ?
                                            (int?)null :
                                            task.InitialAbsolutePosition;

            return await CalendarTaskService.UpdateTaskAsync(new CalendarTask(task), initialAbsolutePosition);
        }

        // DELETE
        [HttpDelete]
        public async Task<bool> Delete(string calendarTaskId)
        {
            return await CalendarTaskService.DeleteTaskAsync(calendarTaskId);
        }
    }
}