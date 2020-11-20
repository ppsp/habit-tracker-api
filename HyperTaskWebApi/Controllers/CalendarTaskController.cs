using HyperTaskCore.Services;
using HyperTaskServices.Models.DTO;
using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using HyperTaskWebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HyperTaskWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class CalendarTaskController : ControllerBase
    {
        private MongoCalendarTaskService CalendarTaskService { get; set; }
        private MongoUserService UserService { get; set; }

        public CalendarTaskController(FirebaseConnector fireConnector,
                                      MongoConnector mongoConnector,
                                      MongoTaskGroupService taskGroupService)
        {
            CalendarTaskService = new MongoCalendarTaskService(mongoConnector);
            UserService = new MongoUserService(mongoConnector, CalendarTaskService, taskGroupService, fireConnector);
        }

        // GET
        [HttpGet]
        [RequestLimit("GetTasks", NoOfRequest = 2000, Seconds = 3600)]
        public async Task<IActionResult> Get([FromQuery]DTOGetCalendarTaskRequest dtoRequest)
        {
            await ValidateUserId(dtoRequest.userId);

            var tasks = await CalendarTaskService.GetTasksAsync(dtoRequest.userId);

            return Ok(tasks.Select(p => new DTOCalendarTask(p)).ToList());
        }

        // POST
        [HttpPost]
        [RequestLimit("PostTask", NoOfRequest = 10000, Seconds = 3600)]
        public async Task<IActionResult> Post([FromBody]DTOCalendarTask task)
        {
            var dateStart = DateTime.Now;
            await ValidateUserId(task.UserId);
            Logger.Debug("Post CalendarTask Validated User, seconds = " + (DateTime.Now - dateStart).TotalSeconds);

            task.Validate();
            Logger.Debug("Post CalendarTask Validated Task, seconds = " + (DateTime.Now - dateStart).TotalSeconds);

            await UserService.UpdateLastActivityDate(task.UserId, task.UpdateDate ?? DateTime.Now);
            Logger.Debug("Post CalendarTask Update Last ActivityDate, seconds = " + (DateTime.Now - dateStart).TotalSeconds);

            var result = await CalendarTaskService.InsertTaskAsync(task);
            Logger.Debug("Post CalendarTask Inserted Task, seconds = " + (DateTime.Now - dateStart).TotalSeconds);

            return Ok(result);
        }

        // PUT
        [HttpPut]
        [RequestLimit("PutTask", NoOfRequest = 5000, Seconds = 3600)]
        public async Task<IActionResult> Put([FromBody]DTOCalendarTask task)
        {
            await ValidateUserId(task.UserId);

            task.Validate();

            await UserService.UpdateLastActivityDate(task.UserId, task.UpdateDate ?? DateTime.Now);

            var result = await CalendarTaskService.UpdateTaskAsync(task);
            return Ok(result);
        }

        private async Task ValidateUserId(string userId)
        {
            if (!await this.UserService.ValidateUserId(userId, this.Request.GetJwt()))
                throw new UnauthorizedAccessException("userId does not correspond to authenticated user");
        }
    }
}