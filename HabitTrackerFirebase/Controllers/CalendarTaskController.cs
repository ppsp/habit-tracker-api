﻿using HabitTrackerCore.Services;
using HabitTrackerServices.Models.DTO;
using HabitTrackerServices.Services;
using HabitTrackerTools;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
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

        public CalendarTaskController(FirebaseConnector connector)
        {
            CalendarTaskService = new CalendarTaskService(connector);
            TaskHistoryService = new TaskHistoryService(CalendarTaskService);
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]DTOGetCalendarTaskRequest dtoRequest)
        {
            var tasks = await CalendarTaskService.GetTasksAsync(dtoRequest.userId);

            return Ok(tasks.Select(p => new DTOCalendarTask(p)).ToList());
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]DTOCalendarTask task)
        {
            task.Validate();

            var result = await CalendarTaskService.InsertTaskAsync(task);
            return Ok(result);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]DTOCalendarTask task)
        {
            task.Validate();

            var result = await CalendarTaskService.UpdateTaskAsync(task);
            return Ok(result);
        }
    }
}