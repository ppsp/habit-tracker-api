using HyperTaskServices.Models.DTO;
using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using HyperTaskWebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HyperTaskWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class TaskGroupController : ControllerBase
    {
        private MongoTaskGroupService _TaskGroupService { get; set; }
        private MongoUserService UserService { get; set; }

        public TaskGroupController(FirebaseConnector fireConnector,
                                   MongoConnector mongoConnector,
                                   MongoCalendarTaskService calendarTaskService)
        {
            _TaskGroupService = new MongoTaskGroupService(mongoConnector);
            UserService = new MongoUserService(mongoConnector, calendarTaskService, _TaskGroupService, fireConnector);
        }

        // GET
        [HttpGet]
        [RequestLimit("GetGroups", NoOfRequest = 2000, Seconds = 3600)]
        public async Task<IActionResult> Get(string userId)
        {
            await ValidateUserId(userId);

            var groups = await _TaskGroupService.GetGroupsAsync(userId);

            return Ok(groups.Select(p => DTOTaskGroup.FromTaskGroup(p)).ToList());
        }

        // POST
        [HttpPost]
        [RequestLimit("PostGroup", NoOfRequest = 5000, Seconds = 3600)]
        public async Task<IActionResult> Post([FromBody]DTOTaskGroup group)
        {
            await ValidateUserId(group.UserId);

            await UserService.UpdateLastActivityDate(group.UserId, group.UpdateDate ?? DateTime.Now);

            var result = await _TaskGroupService.InsertGroupAsync(group.ToTaskGroup());
            return Ok(result);
        }

        // PUT
        [HttpPut]
        [RequestLimit("PutGroup", NoOfRequest = 5000, Seconds = 3600)]
        public async Task<IActionResult> Put([FromBody] DTOTaskGroup group)
        {
            await ValidateUserId(group.UserId);

            await UserService.UpdateLastActivityDate(group.UserId, group.UpdateDate ?? DateTime.Now);

            var result = await _TaskGroupService.UpdateGroupAsync(group.ToTaskGroup());
            return Ok(result);
        }

        private async Task ValidateUserId(string userId)
        {
            if (!await this.UserService.ValidateUserId(userId, this.Request.GetJwt()))
                throw new UnauthorizedAccessException("userId does not correspond to authenticated user");
        }
    }
}